namespace Gossip4Net.Http.Builder.Response
{
    internal class NopResponseBuilder : IResponseBuilder<HttpResponseMessage>
    {
        public Task<HttpResponseMessage> ConstructResponseAsync(HttpResponseMessage responseMessage)
        {
            return Task.FromResult(responseMessage);
        }

        async Task<object?> IResponseBuilder.ConstructResponseAsync(HttpResponseMessage response)
        {
            return await ConstructResponseAsync(response);
        }
    }
}
