namespace Gossip4Net.Http.Builder.Request
{
    internal interface IHttpRequestModifier
    {
        Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args);
    }
}
