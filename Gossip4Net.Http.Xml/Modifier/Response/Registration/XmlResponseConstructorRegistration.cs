using Gossip4Net.Http.Builder.Response;
using Gossip4Net.Http.Modifier.Response;
using Gossip4Net.Http.Modifier.Response.Registration;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Response.Registration
{
    public class XmlResponseConstructorRegistration : IResponseConstructorRegistration
    {
        public IList<IResponseConstructor>? ForMethod(ResponseMethodContext responseMethod)
        {
            if (responseMethod.IsVoid)
            {
                return null;
            }
            XmlSerializer xmlSerializer = new XmlSerializer(responseMethod.ProxyReturnType);
            return new List<IResponseConstructor>
            {
                new XmlResponseConstructor(xmlSerializer)
            };
        }
    }
}
