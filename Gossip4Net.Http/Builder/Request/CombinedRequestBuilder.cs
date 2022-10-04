using Gossip4Net.Http.Modifier.Request;

namespace Gossip4Net.Http.Builder.Request
{
    internal class CombinedHttpRequestBuilder : IHttpRequestBuilder
    {

        private readonly List<IHttpRequestModifier> modifiers;

        public CombinedHttpRequestBuilder(List<IHttpRequestModifier> modifiers)
        {
            this.modifiers = modifiers;
        }

        public async Task<HttpRequestMessage> CreateRequestAsync(object?[] args)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            foreach (var modifier in modifiers)
            {
                request = await modifier.ApplyAsync(request, args);
            }
            return request;
        }
    }
}
