namespace Gossip4Net.Http.Modifier.Response
{
    public interface IResponseConstructor
    {
        public Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response);
    }

    public interface IResponseConstructor<T> : IResponseConstructor
    {
        public Task<ConstructedResponse<T>> ConstructResponseAsync(HttpResponseMessage responseMessage);
    }
}
