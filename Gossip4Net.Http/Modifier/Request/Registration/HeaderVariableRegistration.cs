﻿using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Model;

namespace Gossip4Net.Http.Modifier.Request.Registration
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