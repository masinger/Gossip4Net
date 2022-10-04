using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Builder.Request.Registrations;
using Gossip4Net.Http.Builder.Response;
using Gossip4Net.Http.Client;
using Gossip4Net.Model.Mappings;
using System.Reflection;
using System.Text.Json;

namespace Gossip4Net.Http.Builder.Implementation
{
    internal class MethodImplemantationBuilder
    {

        private delegate Task<object?> PerformRequestDelegate(object?[] arguments);

        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ICollection<IHttpRequestModifier> globalRequestModifiers;
        private readonly Func<HttpClient> clientProvider;
        private readonly IList<IRequestAttributeRegistration> requestAttributeRegistrations;


        public MethodImplemantationBuilder(
            JsonSerializerOptions jsonSerializerOptions,
            ICollection<IHttpRequestModifier> globalRequestModifiers,
            Func<HttpClient> clientProvider,
            IList<IRequestAttributeRegistration> requestAttributeRegistrations
        )
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
            this.globalRequestModifiers = globalRequestModifiers;
            this.clientProvider = clientProvider;
            this.requestAttributeRegistrations = requestAttributeRegistrations;
        }


        private IList<IHttpRequestModifier> BuildRequestModifiers(
            Type typeInfo,
            MethodInfo method
        )
        {
            RequestMethodContext requestMethodContext = new RequestMethodContext(
                typeInfo,
                method
            );
            List<IHttpRequestModifier> requestModifiers = new List<IHttpRequestModifier>();

            IList<Attribute> allMethodAttributes = method.GetCustomAttributes<Attribute>().ToList();
            requestModifiers.AddRange(
                requestAttributeRegistrations
                    .Select(it => it.ForMethod(requestMethodContext, allMethodAttributes))
                    .Where(it => it != null)
                    .SelectMany(it => it!)
            );

            ParameterInfo[] methodParams = method.GetParameters();
            for (int paremeterIndex = 0; paremeterIndex < methodParams.Length; paremeterIndex++)
            {
                ParameterInfo parameterInfo = methodParams[paremeterIndex];

                RequestParameterContext requestParameterContext = new RequestParameterContext(
                    requestMethodContext,
                    parameterInfo,
                    paremeterIndex
                );

                IList<Attribute> allParameterAttributes = parameterInfo.GetCustomAttributes<Attribute>().ToList();
                requestModifiers.AddRange(
                    requestAttributeRegistrations
                        .Select(r => r.ForParameter(requestParameterContext, allParameterAttributes))
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

            IList<IHttpRequestModifier> requestModifiers = BuildRequestModifiers(typeInfo, method);
            CombinedHttpRequestBuilder requestBuilder = new CombinedHttpRequestBuilder(globalRequestModifiers.Concat(requestModifiers).ToList());
            
            ResponseImplementationBuilder responseImplementationBuilder = new ResponseImplementationBuilder(jsonSerializerOptions);
            IResponseBuilder responseBuilder = responseImplementationBuilder.CreateResponseBuilder(method);

            RequestMethodImplementation requestMethodImplementation;

            PerformRequestDelegate asyncImplementation = async (object?[] args) =>
            {
                HttpRequestMessage request = await requestBuilder.CreateRequestAsync(args);

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
