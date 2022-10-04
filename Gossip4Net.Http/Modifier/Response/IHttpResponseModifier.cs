namespace Gossip4Net.Http.Modifier.Response
{
    public interface IHttpResponseModifier
    {
        HttpResponseMessage Modify(HttpResponseMessage response);
    }
}
