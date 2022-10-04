﻿using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Model;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    internal class HttpApiRegistration : RequestAttributeRegistration<HttpApi>
    {
        public override IList<IHttpRequestModifier>? ForType(RequestTypeContext typeContext, IList<HttpApi> attributes)
        {
            HttpApi httpApi = attributes.Single();
            if (httpApi.Url != null)
            {
                return new List<IHttpRequestModifier>() {
                    new RequestUriModifier(httpApi.Url)
                };
            }

            return null;
        }
    }
}
