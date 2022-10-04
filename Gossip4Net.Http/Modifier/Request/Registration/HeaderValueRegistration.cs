﻿using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Model;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    internal class HeaderValueRegistration : RequestAttributeRegistration<HeaderValue>
    {
        public override IList<IHttpRequestModifier>? ForMethod(RequestMethodContext methodContext, IList<HeaderValue> atttributes)
        {
            return atttributes
                .Select(a => new RequestStaticHeaderModifier(a.Name, a.Value))
                .ToList<IHttpRequestModifier>();
        }
    }
}