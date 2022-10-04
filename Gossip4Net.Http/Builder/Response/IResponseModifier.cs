namespace Gossip4Net.Http.Builder.Response
{
    internal interface IResponseModifier
    {
        HttpResponseMessage Modify(HttpResponseMessage response);
    }
}
