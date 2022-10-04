using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Builder.Response;
using Gossip4Net.Http.Client;
using Gossip4Net.Http.Modifier.Request.Registration;
using Gossip4Net.Http.Modifier.Response;
using Gossip4Net.Http.Modifier.Response.Registration;
using Gossip4Net.Model.Mappings;
using System.Reflection;
using System.Text.Json;

namespace Gossip4Net.Http.Builder
{
    internal class MethodImplemantationBuilder
    {

        private delegate Task<object?> PerformRequestDelegate(object?[] arguments);

        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ICollection<IHttpRequestModifier> globalRequestModifiers;
        private readonly Func<HttpClient> clientProvider;
        private readonly IList<IRequestAttributeRegistration> requestAttributeRegistrations;
        private readonly IList<IResponseAttributeRegistration> responseAttributeRegistrations;


        public MethodImplemantationBuilder(
            JsonSerializerOptions jsonSerializerOptions,
            ICollection<IHttpRequestModifier> globalRequestModifiers,
            Func<HttpClient> clientProvider,
            IList<IRequestAttributeRegistration> requestAttributeRegistrations,
            IList<IResponseAttributeRegistration> responseAttributeRegistrations
        )
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
            this.globalRequestModifiers = globalRequestModifiers;
            this.clientProvider = clientProvider;
            this.requestAttributeRegistrations = requestAttributeRegistrations;
            this.responseAttributeRegistrations = responseAttributeRegistrations;
        }


        private IList<IHttpRequestModifier> BuildRequestModifiers(
            RequestMethodContext requestMethodContext
        )
        {
            List<IHttpRequestModifier> requestModifiers = new List<IHttpRequestModifier>();

            IList<Attribute> allMethodAttributes = requestMethodContext.MethodInfo.GetCustomAttributes<Attribute>().ToList();
            requestModifiers.AddRange(
                requestAttributeRegistrations
                    .Select(it => it.ForMethod(requestMethodContext, allMethodAttributes))
                    .Where(it => it != null)
                    .SelectMany(it => it!)
            );

            ParameterInfo[] methodParams = requestMethodContext.MethodInfo.GetParameters();
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

        private PerformRequestDelegate BuildRequestDelegate(RequestMethodContext requestMethodContext)
        {
            HttpMapping? mapping = requestMethodContext.MethodInfo.GetCustomAttributes().Where(a => a is HttpMapping).Select(a => (HttpMapping)a).SingleOrDefault();
            if (mapping == null)
            {
                throw new ArgumentException($"No mapping specified for method '{requestMethodContext.MethodInfo}'.", nameof(requestMethodContext));
            }

            IList<IHttpRequestModifier> requestModifiers = BuildRequestModifiers(requestMethodContext);
            CombinedHttpRequestBuilder requestBuilder = new CombinedHttpRequestBuilder(globalRequestModifiers.Concat(requestModifiers).ToList());

            ResponseImplementationBuilder responseImplementationBuilder = new ResponseImplementationBuilder(jsonSerializerOptions, responseAttributeRegistrations);
            IResponseConstructor responseBuilder = responseImplementationBuilder.CreateResponseBuilder(requestMethodContext.MethodInfo);

            return async (args) =>
            {
                HttpRequestMessage request = await requestBuilder.CreateRequestAsync(args);

                using (HttpClient client = clientProvider())
                {
                    HttpResponseMessage response = await client.SendAsync(request);
                    object? result = await responseBuilder.ConstructResponseAsync(response);
                    return result;
                }
            };

        }

        public KeyValuePair<ClientRegistration, RequestMethodImplementation> BuildImplementation(RequestMethodContext requestMethodContext)
        {
            PerformRequestDelegate asyncImplementation = BuildRequestDelegate(requestMethodContext);

            RequestMethodImplementation requestMethodImplementation;
            Type returnType = requestMethodContext.MethodInfo.ReturnType;

            if (returnType.IsAssignableTo(typeof(Task)))
            {
                Func<Task<object?>, object> castToReturn = a => a;
                if (returnType.IsGenericType)
                {
                    Type genericTaskType = returnType.GenericTypeArguments[0];
                    castToReturn = t => t.WithType(genericTaskType);
                }
                requestMethodImplementation = (args) =>
                {
                    return castToReturn(asyncImplementation(args));
                };
            }
            else
            {
                requestMethodImplementation = (args) =>
                {
                    return asyncImplementation(args).GetAwaiter().GetResult();
                };
            }

            ClientRegistration registration = new ClientRegistration(
                requestMethodContext.MethodInfo.Name,
                requestMethodContext.MethodInfo.GetParameters().Length
            );

            return new KeyValuePair<ClientRegistration, RequestMethodImplementation>(
                   registration,
                   requestMethodImplementation
            );
        }

    }
}
