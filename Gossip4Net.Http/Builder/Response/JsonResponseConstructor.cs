using System.Net.Http.Json;
using System.Text.Json;
using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Builder.Response
{
    internal class JsonResponseConstructor : IResponseConstructor
    {
        private readonly Type targetType;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public JsonResponseConstructor(
            Type targetType,
            JsonSerializerOptions jsonSerializerOptions
        )
        {
            this.targetType = targetType;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response)
        {
            return new ConstructedResponse(await response.Content.ReadFromJsonAsync(targetType, jsonSerializerOptions));
        }
    }
}
