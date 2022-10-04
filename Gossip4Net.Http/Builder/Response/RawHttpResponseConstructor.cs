using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Builder.Response
{
    internal class RawHttpResponseConstructor : IResponseConstructor<HttpResponseMessage>
    {
        public Task<ConstructedResponse<HttpResponseMessage>> ConstructResponseAsync(HttpResponseMessage responseMessage)
        {
            return Task.FromResult(ConstructedResponse.Of(responseMessage));
        }

        async Task<ConstructedResponse> IResponseConstructor.ConstructResponseAsync(HttpResponseMessage response)
        {
            return await ConstructResponseAsync(response);
        }
    }
}
