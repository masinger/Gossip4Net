namespace Gossip4Net.Http.Modifier.Request
{
    internal class RequestUriModifier : IHttpRequestModifier
    {
        private readonly string url;

        public RequestUriModifier(string url)
        {
            this.url = url;
        }

        public Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args)
        {
            requestMessage.RequestUri = requestMessage.RequestUri.Append(url);
            return Task.FromResult(requestMessage);
        }
    }
}
