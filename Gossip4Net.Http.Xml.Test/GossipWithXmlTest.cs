using FluentAssertions;
using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;
using Gossip4Net.Http.Xml.Modifier.Response.Registration;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Test
{
    [XmlRoot("slideshow")]
    public record Slideshow([property: XmlAttribute("title")] string Title, [property: XmlAttribute("author")] string Author)
    {
        // Default constructor required by XmlSerializer
        public Slideshow() : this(string.Empty, string.Empty) { }
    }

    [HttpApi("https://httpbin.org")]
    public interface IXmlHttpBinClient {

        [GetMapping("/xml")]
        Task<Slideshow> GetSlideshow();
    }
    
    public class GossipWithXmlTest
    {
        [Fact]
        public async Task XmlResponseBodiesShouldGetDeserialized()
        {
            // Arrange
            IHttpGossipBuilder<IXmlHttpBinClient> builder = new HttpGossipBuilder<IXmlHttpBinClient>()
                .WithRegistrations(reg => reg.With(new XmlResponseConstructorRegistration()))
                .AddDefaultBehavior();

            IXmlHttpBinClient client = builder.Build();

            // Act
            Slideshow slideshow = await client.GetSlideshow();

            // Assert
            slideshow.Title.Should().Be("Sample Slide Show");
            slideshow.Author.Should().Be("Yours Truly");
        }
    }
}