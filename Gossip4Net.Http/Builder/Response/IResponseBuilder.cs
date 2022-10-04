namespace Gossip4Net.Http.Builder.Response
{
    public interface IResponseBuilder
    {
        Task<object?> ConstructResponseAsync(HttpResponseMessage response);
    }

    public interface IResponseBuilder<T> : IResponseBuilder
    {
        new Task<T> ConstructResponseAsync(HttpResponseMessage responseMessage);
    }
}
