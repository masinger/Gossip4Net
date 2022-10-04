using System.Reflection;
using System.Text.Json;
using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Builder.Response
{
    internal class ResponseImplementationBuilder
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public ResponseImplementationBuilder(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public IResponseBuilder CreateResponseBuilder(MethodInfo method)
        {
            List<IResponseModifier> responseModifiers = new List<IResponseModifier>();
            responseModifiers.Add(new ResponseSuccessModifier());

            Type returnType = method.ReturnType.TaskResultType() ?? method.ReturnType;


            if (returnType.IsAssignableFrom(typeof(HttpResponseMessage)) || returnType.Equals(typeof(void)) || returnType.IsVoidTask())
            {
                return new CombinedResponseBuilder(responseModifiers, new NopResponseBuilder());
            }

            JsonResponseBuilder jsonResponseBuilder = new JsonResponseBuilder(returnType, jsonSerializerOptions);
            return new CombinedResponseBuilder(responseModifiers, jsonResponseBuilder);
        }
    }
}
