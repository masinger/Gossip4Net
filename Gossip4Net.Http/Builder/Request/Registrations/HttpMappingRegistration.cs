﻿using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Model.Mappings;

namespace Gossip4Net.Http.Builder.Request.Registrations
{
    internal class HttpMappingRegistration : RequestAttributeRegistration<HttpMapping>
    {

        public override IList<IHttpRequestModifier>? ForMethod(RequestMethodContext methodContext, IList<HttpMapping> atttributes)
        {
            HttpMapping mapping = atttributes.Single();
            IList<IHttpRequestModifier> modifiers = new List<IHttpRequestModifier>()
            {
                new RequestMethodModifier(mapping.Method)
            };

            if (mapping.Path != null)
            {
                modifiers.Add(new RequestUriModifier(mapping.Path));
            }

            return modifiers;
        }

    }
}