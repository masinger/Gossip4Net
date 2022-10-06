using Gossip4Net.Http.Builder.Response;

namespace Gossip4Net.Http.Modifier.Response.Registration
{
    public interface IResponseModifierRegistration
    {
        public IList<IHttpResponseModifier>? ForMethod(ResponseMethodContext methodContext, IEnumerable<Attribute> attributes);
    }
}
