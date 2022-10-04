using Gossip4Net.Http.Builder.Request;

namespace Gossip4Net.Http.Modifier.Request
{
    internal class RequestMethodModifier : IHttpRequestModifier
    {
        private readonly HttpMethod method;

        public RequestMethodModifier(HttpMethod method)
        {
            this.method = method;
        }

        public Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args)
        {
            requestMessage.Method = method;
            return Task.FromResult(requestMessage);
        }
    }
}
