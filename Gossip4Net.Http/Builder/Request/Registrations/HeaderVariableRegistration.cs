using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip4Net.Http.Builder.Request.Registrations
{
    internal class HeaderVariableRegistration : RequestAttributeRegistration<HeaderVariable>
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
