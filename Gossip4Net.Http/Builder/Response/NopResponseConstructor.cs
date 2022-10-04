using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Builder.Response
{
    internal class NopResponseConstructor : IResponseConstructor<HttpResponseMessage>
    {
        public Task<HttpResponseMessage> ConstructResponseAsync(HttpResponseMessage responseMessage)
        {
            return Task.FromResult(responseMessage);
        }

        async Task<object?> IResponseConstructor.ConstructResponseAsync(HttpResponseMessage response)
        {
            return await ConstructResponseAsync(response);
        }
    }
}
