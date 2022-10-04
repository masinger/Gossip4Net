namespace Gossip4Net.Http.Builder.Response
{

    internal interface IResponseBuilder
    {

        Task<object?> ConstructResponseAsync(HttpResponseMessage response);

    }

    internal interface IResponseBuilder<T> : IResponseBuilder
    {
        new Task<T> ConstructResponseAsync(HttpResponseMessage responseMessage);
    }
}
