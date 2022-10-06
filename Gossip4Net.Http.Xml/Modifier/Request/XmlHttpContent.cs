using System.Net;
using System.Net.Http.Headers;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Request
{
    internal class XmlHttpContent : HttpContent
    {

        private readonly XmlSerializer xmlSerializer;
        private readonly object? value;

        public XmlHttpContent(
            XmlSerializer xmlSerializer,
            object? value)
        {
            this.xmlSerializer = xmlSerializer;
            this.value = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            xmlSerializer.Serialize(stream, value);
            return Task.CompletedTask;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
