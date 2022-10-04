namespace Gossip4Net.Http.Builder.Request
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
            Uri? currentUri = requestMessage.RequestUri;

            Uri? absoluteUri;
            Uri.TryCreate(url, UriKind.Absolute, out absoluteUri);

            Uri? relativeUri;
            Uri.TryCreate(url, UriKind.Relative, out relativeUri);

            if (currentUri == null || absoluteUri != null)
            {
                requestMessage.RequestUri = absoluteUri;
                return Task.FromResult(requestMessage);
            }

            requestMessage.RequestUri = new Uri(currentUri, relativeUri ?? throw new ArgumentException($"Invalid uri '{url}'."));
            return Task.FromResult(requestMessage);
        }
    }
}
