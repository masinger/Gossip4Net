namespace Gossip4Net.Http.Modifier.Response
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
