using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Builder.Response;
using Gossip4Net.Http.Client;
using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Http.Modifier.Response;
using Gossip4Net.Model.Mappings;
using System.Reflection;

namespace Gossip4Net.Http.Builder
{
    internal class MethodImplemantationBuilder
    {
        private delegate Task<object?> PerformRequestDelegate(object?[] arguments);

        private readonly ICollection<IHttpRequestModifier> globalRequestModifiers;
        private readonly Func<HttpClient> clientProvider;
        private readonly Registrations registrations;

        public MethodImplemantationBuilder(
            Func<HttpClient> clientProvider,
            ICollection<IHttpRequestModifier> globalRequestModifiers,
            Registrations registrations
        )
        {
            this.globalRequestModifiers = globalRequestModifiers;
            this.clientProvider = clientProvider;
            this.registrations = registrations;
        }
      
        public KeyValuePair<MethodSignature, RequestMethodImplementation> BuildImplementation(RequestMethodContext requestMethodContext)
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

            MethodSignature registration = new MethodSignature(
                requestMethodContext.MethodInfo.Name,
                requestMethodContext.MethodInfo.GetParameters().Length
            );

            return new KeyValuePair<MethodSignature, RequestMethodImplementation>(
                   registration,
                   requestMethodImplementation
            );
        }

        private IList<IHttpRequestModifier> BuildRequestModifiers(
          RequestMethodContext requestMethodContext
        )
        {
            List<IHttpRequestModifier> requestModifiers = new List<IHttpRequestModifier>();

            IList<Attribute> allMethodAttributes = requestMethodContext.MethodInfo.GetCustomAttributes<Attribute>().ToList();
            requestModifiers.AddRange(
                registrations.RequestAttributes
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
                    registrations.RequestAttributes
                        .Select(r => r.ForParameter(requestParameterContext, allParameterAttributes))
                        .Where(it => it != null)
                        .SelectMany(it => it!)
                );
            }

            return requestModifiers;
        }

        private IResponseConstructor CreateResponseBuilder(MethodInfo method)
        {
            Type returnType = method.ReturnType.TaskResultType() ?? method.ReturnType;
            ResponseMethodContext responseMethodContext = new ResponseMethodContext(returnType, method);

            IList<Attribute> allMethodAttributes = method.GetCustomAttributes<Attribute>().ToList();
            List<IHttpResponseModifier> responseModifiers = registrations.ResponseAttributes
                    .Select(it => it.ForMethod(responseMethodContext, allMethodAttributes))
                    .Where(it => it != null)
                    .SelectMany(it => it!)
            .ToList();

            List<IResponseConstructor> responseConstructors = registrations.ResponseConstructors
                    .Select(it => it.ForMethod(responseMethodContext))
                    .Where(it => it != null)
                    .SelectMany(it => it!)
                    .ToList();

            return new CombinedResponseConstructor(
                responseModifiers,
                responseConstructors
            );
        }

        private PerformRequestDelegate BuildRequestDelegate(RequestMethodContext requestMethodContext)
        {
            HttpMapping? mapping = requestMethodContext.MethodInfo.GetCustomAttributes().Where(a => a is HttpMapping).Select(a => (HttpMapping)a).SingleOrDefault();
            if (mapping == null)
            {
                throw new ConfigurationException($"No mapping specified for method '{requestMethodContext.MethodInfo}'.");
            }

            IList<IHttpRequestModifier> requestModifiers = BuildRequestModifiers(requestMethodContext);
            CombinedHttpRequestBuilder requestBuilder = new CombinedHttpRequestBuilder(globalRequestModifiers.Concat(requestModifiers).ToList());

            IResponseConstructor responseConstructor = CreateResponseBuilder(requestMethodContext.MethodInfo);
            return async (args) =>
            {
                HttpRequestMessage request = await requestBuilder.CreateRequestAsync(args);

                using (HttpClient client = clientProvider())
                {
                    HttpResponseMessage response = await client.SendAsync(request);
                    ConstructedResponse constructedResponse = await responseConstructor.ConstructResponseAsync(response);
                    if (constructedResponse.IsEmpty)
                    {
                        throw new NoResponseCreatorException();
                    }
                    return constructedResponse.Response;
                }
            };

        }
    }
}
