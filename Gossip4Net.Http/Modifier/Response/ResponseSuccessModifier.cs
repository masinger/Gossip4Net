namespace Gossip4Net.Http.Modifier.Response
{
    public class ResponseSuccessModifier : IHttpResponseModifier
    {
        public HttpResponseMessage Modify(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
