using System.Net.Http.Json;
using System.Text.Json;

namespace Gossip4Net.Http.Builder.Response
{
    internal class JsonResponseBuilder : IResponseBuilder
    {
        private readonly Type targetType;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public JsonResponseBuilder(
            Type targetType,
            JsonSerializerOptions jsonSerializerOptions
        )
        {
            this.targetType = targetType;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task<object?> ConstructResponseAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync(targetType, jsonSerializerOptions);
        }
    }
}
