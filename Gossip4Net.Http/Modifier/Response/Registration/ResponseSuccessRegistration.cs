using Gossip4Net.Http.Attributes.Mappings;
using Gossip4Net.Http.Builder.Response;

namespace Gossip4Net.Http.Modifier.Response.Registration
{
    internal class ResponseSuccessRegistration : ResponseAttributeRegistration<HttpMapping>
    {

        public override IList<IHttpResponseModifier>? ForMethod(ResponseMethodContext methodContext, IList<HttpMapping> attributes)
        {
            return new List<IHttpResponseModifier>()
            {
                new ResponseSuccessModifier()
            };
        }

    }
}
