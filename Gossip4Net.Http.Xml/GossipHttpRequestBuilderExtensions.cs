using Gossip4Net.Http.Xml.Modifier.Request.Registration;
using Gossip4Net.Http.Xml.Modifier.Response.Registration;
using System.Xml.Serialization;

namespace Gossip4Net.Http
{
    public static class HttpGossipBuilderExtensions
    {

        public static IHttpGossipBuilder<T> AddXmlBehavior<T>(
            this IHttpGossipBuilder<T> instance,
            bool applyByDefault,
            Func<Type, XmlSerializer> serializerProvider)
        {
            return instance
                .WithRegistrations(r => r
                    .With(new XmlRequestBodyRegistration(serializerProvider))
                    .With(new XmlResponseConstructorRegistration(serializerProvider, applyByDefault))
                );
        }

        public static IHttpGossipBuilder<T> AddXmlBehavior<T>(
            this IHttpGossipBuilder<T> instance,
            bool applyByDefault)
        {
            return instance.AddXmlBehavior(
                applyByDefault,
                t => new XmlSerializer(t));
        }

        public static IHttpGossipBuilder<T> AddXmlBehavior<T>(
            this IHttpGossipBuilder<T> instance)
        {
            return instance.AddXmlBehavior(false);
        }

    }
}
