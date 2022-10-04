namespace Gossip4Net.Http.Builder.Response
{
    internal class ResponseSuccessModifier : IResponseModifier
    {
        public HttpResponseMessage Modify(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
