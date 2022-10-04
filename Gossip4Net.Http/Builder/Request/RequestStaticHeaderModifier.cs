namespace Gossip4Net.Http.Builder.Request
{
    internal class RequestStaticHeaderModifier : IHttpRequestModifier
    {
        private readonly string name;
        private readonly string value;

        public RequestStaticHeaderModifier(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args)
        {
            requestMessage.Headers.Add(name, value);
            return Task.FromResult(requestMessage);
        }
    }
}
