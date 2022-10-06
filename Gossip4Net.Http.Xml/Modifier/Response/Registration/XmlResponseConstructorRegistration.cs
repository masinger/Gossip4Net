using Gossip4Net.Http.Builder.Response;
using Gossip4Net.Http.Modifier.Response;
using Gossip4Net.Http.Modifier.Response.Registration;
using System.Reflection;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Response.Registration
{
    public class XmlResponseConstructorRegistration : IResponseConstructorRegistration
    {
        private readonly Func<Type, XmlSerializer> serializerProvider;
        private readonly bool applyByDefault;

        public XmlResponseConstructorRegistration(
            Func<Type, XmlSerializer> serializerProvider,
            bool applyByDefault)
        {
            this.serializerProvider = serializerProvider;
            this.applyByDefault = applyByDefault;
        }

        public IList<IResponseConstructor>? ForMethod(ResponseMethodContext responseMethod)
        {
            if (responseMethod.IsVoid)
            {
                return null;
            }

            if (!applyByDefault && !responseMethod.MethodInfo.GetCustomAttributes<XmlResponse>().Any())
            {
                return null;
            }

            if (responseMethod.MethodInfo.GetCustomAttributes<NoXmlResponse>().Any())
            {
                return null;
            }
            XmlSerializer xmlSerializer = serializerProvider(responseMethod.ProxyReturnType);
            return new List<IResponseConstructor>
            {
                new XmlResponseConstructor(xmlSerializer)
            };
        }
    }
}
