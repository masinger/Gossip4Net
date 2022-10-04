namespace Gossip4Net.Http.Builder.Request
{
    public interface IHttpRequestBuilder
    {
        Task<HttpRequestMessage> CreateRequestAsync(params object[] args);
    }
}
