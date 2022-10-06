using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Http.Modifier.Request.Registration;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Request.Registration
{
    public class XmlRequestBodyRegistration : RequestModifierRegistration<XmlBody>
    {
        private readonly Func<Type, XmlSerializer> serializerProvider;

        public XmlRequestBodyRegistration(Func<Type, XmlSerializer> serializerProvider)
        {
            this.serializerProvider = serializerProvider;
        }

        public override IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IList<XmlBody> attributes)
        {
            XmlBody xmlBody = attributes.Single();

            return new List<IHttpRequestModifier>()
            {
                new XmlRequestBodyModifier(
                    serializerProvider(parameterContext.Parameter.ParameterType),
                    xmlBody.OmitEmpty,
                    parameterContext.ParameterIndex
                )
            };
        }
    }
}
