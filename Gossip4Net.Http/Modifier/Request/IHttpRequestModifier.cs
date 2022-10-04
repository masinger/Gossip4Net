namespace Gossip4Net.Http.Builder.Request
{
    public interface IHttpRequestModifier
    {
        Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args);
    }
}
