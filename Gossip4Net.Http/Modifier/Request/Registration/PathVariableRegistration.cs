using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Builder.Request;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    public class PathVariableRegistration : RequestModifierRegistration<PathVariable>
    {
        private readonly Func<object?, string> valueConverter;

        public PathVariableRegistration(Func<object?, string> valueConverter)
        {
            this.valueConverter = valueConverter;
        }

        public override IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IList<PathVariable> attributes)
        {
            return new List<IHttpRequestModifier>() {
                new RequestPathVariableModifier(valueConverter, attributes, parameterContext.Parameter.Name, parameterContext.ParameterIndex)
            };
        }
    }
}
