using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Builder.Request;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    public class HeaderVariableRegistration : RequestModifierRegistration<HeaderVariable>
    {

        private readonly Func<object?, string> valueConverter;

        public HeaderVariableRegistration(Func<object?, string> valueConverter)
        {
            this.valueConverter = valueConverter;
        }

        public override IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IList<HeaderVariable> attributes)
        {
            return new List<IHttpRequestModifier>()
            {
                new RequestHeaderModifier(valueConverter, attributes, parameterContext.Parameter.Name, parameterContext.ParameterIndex),
            };
        }
    }
}
