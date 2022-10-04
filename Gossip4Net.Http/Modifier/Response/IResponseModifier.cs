namespace Gossip4Net.Http.Modifier.Response
{
    internal interface IResponseModifier
    {
        HttpResponseMessage Modify(HttpResponseMessage response);
    }
}
