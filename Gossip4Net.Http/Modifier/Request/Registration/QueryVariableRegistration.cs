using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Builder.Request;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    internal class QueryVariableRegistration : RequestAttributeRegistration<QueryVariable>
    {
        private readonly Func<object?, string> valueConverter;

        public QueryVariableRegistration(Func<object?, string> valueConverter)
        {
            this.valueConverter = valueConverter;
        }

        public override IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IList<QueryVariable> attributes)
        {
            return new List<IHttpRequestModifier>()
            {
                new RequestQueryModifier(valueConverter, attributes, parameterContext.Parameter.Name, parameterContext.ParameterIndex)
            };
        }
    }
}
