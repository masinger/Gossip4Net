using System.Reflection;
using System.Text.Json;
using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Modifier.Response;
using Gossip4Net.Http.Modifier.Response.Registration;

namespace Gossip4Net.Http.Builder.Response
{
    internal class ResponseImplementationBuilder
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly IList<IResponseAttributeRegistration> responseAttributeRegistrations;

        public ResponseImplementationBuilder(
            JsonSerializerOptions jsonSerializerOptions,
            IList<IResponseAttributeRegistration> responseAttributeRegistrations
        )
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
            this.responseAttributeRegistrations = responseAttributeRegistrations;
        }

        public IResponseConstructor CreateResponseBuilder(MethodInfo method)
        {
            List<IHttpResponseModifier> responseModifiers = new List<IHttpResponseModifier>();
            responseModifiers.Add(new ResponseSuccessModifier());

            Type returnType = method.ReturnType.TaskResultType() ?? method.ReturnType;

            ResponseMethodContext responseMethodContext = new ResponseMethodContext(returnType, method);
            IList<Attribute> allMethodAttributes = method.GetCustomAttributes<Attribute>().ToList();
            responseModifiers.AddRange(
                responseAttributeRegistrations
                    .Select(it => it.ForMethod(responseMethodContext, allMethodAttributes))
                    .Where(it => it != null)
                    .SelectMany(it => it!)
            );

            if (returnType.IsAssignableFrom(typeof(HttpResponseMessage)) || returnType.Equals(typeof(void)) || returnType.IsVoidTask())
            {
                return new CombinedResponseBuilder(responseModifiers, new NopResponseConstructor());
            }

            JsonResponseConstructor jsonResponseBuilder = new JsonResponseConstructor(returnType, jsonSerializerOptions);
            return new CombinedResponseBuilder(responseModifiers, jsonResponseBuilder);
        }
    }
}
