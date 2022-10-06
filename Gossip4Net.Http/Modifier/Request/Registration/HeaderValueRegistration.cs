using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Builder.Request;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    public class HeaderValueRegistration : RequestAttributeRegistration<HeaderValue>
    {
        public override IList<IHttpRequestModifier>? ForMethod(RequestMethodContext methodContext, IList<HeaderValue> atttributes)
        {
            return atttributes
                .Select(a => new RequestStaticHeaderModifier(a.Name, a.Value))
                .ToList<IHttpRequestModifier>();
        }

        public override IList<IHttpRequestModifier>? ForType(RequestTypeContext typeContext, IList<HeaderValue> attributes)
        {
            return attributes
                .Select(it => new RequestStaticHeaderModifier(it.Name, it.Value))
                .ToList<IHttpRequestModifier>();
        }
    }
}
