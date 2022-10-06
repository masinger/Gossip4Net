using Gossip4Net.Http.Modifier.Response;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Response
{
    internal class XmlResponseConstructor : IResponseConstructor
    {
        private readonly XmlSerializer xmlSerializer;

        public XmlResponseConstructor(XmlSerializer xmlSerializer)
        {
            this.xmlSerializer = xmlSerializer;
        }

        private static readonly ISet<string> SupportedMediaTypes = new HashSet<string>()
        {
            "application/xml",
            "text/xml"
        };

        public async Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response)
        {
            string? mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType == null || !SupportedMediaTypes.Contains(mediaType))
            {
                return ConstructedResponse.Empty;
            }
            object? responseObject = xmlSerializer.Deserialize(await response.Content.ReadAsStreamAsync());
            return new ConstructedResponse(responseObject);
        }
    }
}
