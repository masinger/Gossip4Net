using Gossip4Net.Http.Modifier.Request;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Request
{
    internal class XmlRequestBodyModifier : IHttpRequestModifier
    {
        private readonly XmlSerializer xmlSerializer;
        private readonly bool omitEmpty;
        private readonly int argumentIndex;

        public XmlRequestBodyModifier(
            XmlSerializer xmlSerializer,
            bool omitEmpty,
            int argumentIndex)
        {
            this.xmlSerializer = xmlSerializer;
            this.omitEmpty = omitEmpty;
            this.argumentIndex = argumentIndex;
        }

        public Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args)
        {
            if (requestMessage.Content != null)
            {
                return Task.FromResult(requestMessage);
            }

            object? arg = args[argumentIndex];

            if (arg == null && omitEmpty)
            {
                return Task.FromResult(requestMessage);
            }

            requestMessage.Content = new XmlHttpContent(xmlSerializer, arg);
            return Task.FromResult(requestMessage);
        }
    }
}
