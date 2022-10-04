using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Builder.Request.Registrations;
using Gossip4Net.Http.Builder.Response;
using Gossip4Net.Http.Client;
using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Model;
using Gossip4Net.Model.Mappings;
using System.Reflection;
using System.Text.Json;

namespace Gossip4Net.Http.Builder.Implementation
{
    internal class MethodImplemantationBuilder
    {

        private static readonly Func<object?, string> DefaultValueConverter = o => "" + o;

        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ICollection<IHttpRequestModifier> globalRequestModifiers;
        private readonly Func<HttpClient> clientProvider;
        private readonly IList<IRequestAttributeRegistration> requestAttributeRegistrations;


        public MethodImplemantationBuilder(
            JsonSerializerOptions jsonSerializerOptions,
            ICollection<IHttpRequestModifier> globalRequestModifiers,
            Func<HttpClient> clientProvider
        )
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
            this.globalRequestModifiers = globalRequestModifiers;
            this.clientProvider = clientProvider;
            requestAttributeRegistrations = new List<IRequestAttributeRegistration>() // TODO: Inject
            {
                new PathVariableRegistration(DefaultValueConverter),
                new QueryVariableRegistration(DefaultValueConverter),
                new HeaderVariableRegistration(DefaultValueConverter),
                new RequestBodyRegistration(jsonSerializerOptions),
            };
        }


        private IList<IHttpRequestModifier> BuildRequestModifiers(
            Type typeInfo,
            MethodInfo method,
            HttpMapping mapping
        )
        {
            List<IHttpRequestModifier> requestModifiers = new List<IHttpRequestModifier>();
            requestModifiers.Add(new RequestMethodModifier(mapping.Method));
            if (mapping.Path != null)
            {
                requestModifiers.Add(new RequestUriModifier(mapping.Path));
            }

            foreach (HeaderValue headerValue in method.GetCustomAttributes<HeaderValue>())
            {
                requestModifiers.Add(new RequestStaticHeaderModifier(headerValue.Name, headerValue.Value));
            }

            ParameterInfo[] methodParams = method.GetParameters();

            for (int i = 0; i < methodParams.Length; i++)
            {
                ParameterInfo parameterInfo = methodParams[i];

                RequestParameterContext requestParameterContext = new RequestParameterContext(
                    typeInfo,
                    parameterInfo,
                    i
                );

                IList<Attribute> allAttributes = parameterInfo.GetCustomAttributes<Attribute>().ToList();
                requestModifiers.AddRange(
                    requestAttributeRegistrations
                        .Select(r => r.ForParameter(requestParameterContext, allAttributes))
                        .Where(it => it != null)
                        .SelectMany(it => it!)
                );
            }


            return requestModifiers;

        }

        public KeyValuePair<ClientRegistration, RequestMethodImplementation> BuildImplementation(Type typeInfo, MethodInfo method)
        {
            HttpMapping? mapping = method.GetCustomAttributes().Where(a => a is HttpMapping).Select(a => (HttpMapping)a).SingleOrDefault();
            if (mapping == null)
            {
                throw new ArgumentException("No mapping specified for method.", nameof(method));
            }

            ParameterInfo[] parameters = method.GetParameters();
            Type returnType = method.ReturnType;
            ClientRegistration registration = new ClientRegistration(method.Name, parameters.Length);


            IList<IHttpRequestModifier> requestModifiers = BuildRequestModifiers(typeInfo, method, mapping);

            CombinedHttpRequestBuilder requestBuilder = new CombinedHttpRequestBuilder(globalRequestModifiers.Concat(requestModifiers).ToList());
            ResponseImplementationBuilder responseImplementationBuilder = new ResponseImplementationBuilder(jsonSerializerOptions);
            var responseBuilder = responseImplementationBuilder.CreateResponseBuilder(method);

            RequestMethodImplementation requestMethodImplementation;

            var asyncImplementation = async (object?[] args) =>
            {
                var request = await requestBuilder.CreateRequestAsync(args);

                using (HttpClient client = clientProvider())
                {
                    var response = await client.SendAsync(request);
                    var result = await responseBuilder.ConstructResponseAsync(response);
                    return result;
                }
            };

            if (returnType.IsAssignableTo(typeof(Task)))
            {
                Func<Task<object?>, object> castToReturn = a => a;
                if (returnType.IsGenericType)
                {
                    Type genericTaskType = returnType.GenericTypeArguments[0];
                    castToReturn = t => t.WithType(genericTaskType);
                }
                requestMethodImplementation = (object?[] args) =>
                {
                    return castToReturn(asyncImplementation(args));
                };
            }
            else
            {
                requestMethodImplementation = (object?[] args) =>
                {
                    return asyncImplementation(args).GetAwaiter().GetResult();
                };
            }


            return new KeyValuePair<ClientRegistration, RequestMethodImplementation>(
                   registration,
                   requestMethodImplementation
            );
        }

    }
}
