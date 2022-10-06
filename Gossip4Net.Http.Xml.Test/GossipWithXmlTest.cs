using FluentAssertions;
using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;
using Gossip4Net.Http.Xml.Modifier.Request.Registration;
using Gossip4Net.Http.Xml.Modifier.Response.Registration;
using System.Text.Json;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Test
{
    [XmlRoot("slideshow")]
    public record Slideshow([property: XmlAttribute("title")] string Title, [property: XmlAttribute("author")] string Author)
    {
        // Default constructor required by XmlSerializer
        public Slideshow() : this(string.Empty, string.Empty) { }
    }

    public record HttpBinResponse(string Data);

    [HttpApi("https://httpbin.org")]
    public interface IXmlHttpBinClient {

        [XmlResponse]
        [GetMapping("/xml")]
        Task<Slideshow> GetSlideshow();

        [PostMapping("/post")]
        Task<HttpBinResponse> PostXml([XmlBody] Slideshow slideshow);
    }


    
    public class GossipWithXmlTest
    {

        [Fact]
        public async Task XmlResponseBodiesShouldGetDeserializedWhenUsingExtensionMethods()
        {
            // Arrange
            IHttpGossipBuilder<IXmlHttpBinClient> builder = new HttpGossipBuilder<IXmlHttpBinClient>()
                .AddXmlBehavior()
                .AddDefaultBehavior();

            IXmlHttpBinClient client = builder.Build();

            // Act
            Slideshow slideshow = await client.GetSlideshow();

            // Assert
            slideshow.Title.Should().Be("Sample Slide Show");
            slideshow.Author.Should().Be("Yours Truly");
        }

        [Fact]
        public async Task XmlResponseBodiesShouldGetDeserialized()
        {
            // Arrange
            IHttpGossipBuilder<IXmlHttpBinClient> builder = new HttpGossipBuilder<IXmlHttpBinClient>()
                .WithRegistrations(reg => reg.With(new XmlResponseConstructorRegistration(t => new XmlSerializer(t), false)))
                .AddDefaultBehavior();

            IXmlHttpBinClient client = builder.Build();
            
            // Act
            Slideshow slideshow = await client.GetSlideshow();

            // Assert
            slideshow.Title.Should().Be("Sample Slide Show");
            slideshow.Author.Should().Be("Yours Truly");
        }

        [Fact]
        public async Task XmlRequestBodyShouldGetSerialized()
        {
            // Arrange
            IHttpGossipBuilder<IXmlHttpBinClient> builder = new HttpGossipBuilder<IXmlHttpBinClient>()
                .WithRegistrations(reg => reg.With(new XmlRequestBodyRegistration(t => new XmlSerializer(t))))
                .AddDefaultBehavior(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            IXmlHttpBinClient client = builder.Build();

            // Act
            HttpBinResponse response = await client.PostXml(new Slideshow("Test title", "Test author"));

            // Assert
            response.Data.Should().Contain("title=\"Test title\"").And.Contain("author=\"Test author\"");
        }
    }
}