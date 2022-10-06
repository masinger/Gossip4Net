using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Http.Test.Clients;
using FluentAssertions;
using Gossip4Net.Http.Attributes.Mappings;
using Gossip4Net.Http.Attributes;
using System.Text.Json;
using Gossip4Net.Http.Modifier.Request.Registration;

namespace Gossip4Net.Http.Test
{
    [HttpApi("/get")]
    public interface IRelativeHttpBinClient
    {
        [GetMapping]
        Task<HttpBinResponse> Get();
    }

    public class GossipBuilderCustomRegistrationsTest
    {
        [Fact]
        public async Task RequestModifiersShouldBeAddedByHelperMethods()
        {
            // Arrange
            IHttpGossipBuilder<IRelativeHttpBinClient> builder = new HttpGossipBuilder<IRelativeHttpBinClient>();
            IRelativeHttpBinClient client = builder
                .WithRegistrations(r => r.With(new RequestUriModifier("https://httpbin.org")))
                .AddDefaultBehavior(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
                .Build();

            // Act
            HttpBinResponse response = await client.Get();

            // Assert
            response.Url.Should().Be("https://httpbin.org/get");
        }

        [Fact]
        public async Task RequestModifierReqistrationsShouldBeAddedByHelperMethods()
        {
            // Arrange
            IHttpGossipBuilder<IRelativeHttpBinClient> builder = new HttpGossipBuilder<IRelativeHttpBinClient>();
            IRelativeHttpBinClient client = builder
                .WithRegistrations(r => r.With(
                    new GlobalRequestRegistration(new RequestUriModifier("https://httpbin.org"))
                ))
                .AddDefaultBehavior(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
                .Build();

            // Act
            HttpBinResponse response = await client.Get();

            // Assert
            response.Url.Should().Be("https://httpbin.org/get");
        }
    }
}
