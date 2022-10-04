using Gossip4Net.Http.Builder.Response;
using System.Text.Json;

namespace Gossip4Net.Http.Modifier.Response.Registration
{
    public class JsonResponseConstructorRegistration : IResponseConstructorRegistration
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public JsonResponseConstructorRegistration(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public IList<IResponseConstructor>? ForMethod(ResponseMethodContext responseMethod)
        {
            if (responseMethod.IsVoid)
            {
                return null;
            }

            return new List<IResponseConstructor>() {
                new JsonResponseConstructor(responseMethod.ProxyReturnType, jsonSerializerOptions)
            };
        }
    }
}
