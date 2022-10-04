namespace Gossip4Net.Http.Modifier.Request
{
    public interface IHttpRequestModifier
    {
        Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args);
    }
}
