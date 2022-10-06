using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Builder.Request;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    public class QueryValueRegistration : RequestModifierRegistration<QueryValue>
    {
        private readonly Func<object?, string> valueConverter;

        public QueryValueRegistration(Func<object?, string> valueConverter)
        {
            this.valueConverter = valueConverter;
        }

        public override IList<IHttpRequestModifier>? ForMethod(RequestMethodContext methodContext, IList<QueryValue> attributes)
        {
            return attributes
                .Select(it => new RequestStaticQueryModifier(it.Name, it.Value, valueConverter, it.EnumerateUsingMultipleParams))
                .ToList<IHttpRequestModifier>();
        }

        public override IList<IHttpRequestModifier>? ForType(RequestTypeContext typeContext, IList<QueryValue> attributes)
        {
            return attributes
                .Select(it => new RequestStaticQueryModifier(it.Name, it.Value, valueConverter, it.EnumerateUsingMultipleParams))
                .ToList<IHttpRequestModifier>();
        }
    }
}
