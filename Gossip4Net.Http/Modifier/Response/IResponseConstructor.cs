namespace Gossip4Net.Http.Modifier.Response
{
    public interface IResponseConstructor
    {
        Task<object?> ConstructResponseAsync(HttpResponseMessage response);
    }

    public interface IResponseConstructor<T> : IResponseConstructor
    {
        new Task<T> ConstructResponseAsync(HttpResponseMessage responseMessage);
    }
}
