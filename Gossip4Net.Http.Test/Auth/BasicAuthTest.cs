using FluentAssertions;
using IdentityModel.Client;
using System.Net;

namespace Gossip4Net.Http.Test.Auth
{
    public class BasicAuthTest
    {
        [Fact]
        public void HttpClientWithBasicAuthShouldWork()
        {
            // Arrange
            IHttpGossipBuilder<IHttpBinStaticBasicAuthApi> gossipBuilder = HttpGossipBuilder<IHttpBinStaticBasicAuthApi>.NewDefaultBuilder();
            gossipBuilder.ClientProvider = () =>
            {
                HttpClient httpClient = new HttpClient();
                httpClient.SetBasicAuthentication("myuser", "mypassword");
                return httpClient;
            };
            IHttpBinStaticBasicAuthApi api = gossipBuilder.Build();

            // Act
            HttpResponseMessage response = api.GetWithBasicAuth();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public void HttpClientWithoutBasicAuthShouldNotWork()
        {
            // Arrange
            IHttpGossipBuilder<IHttpBinStaticBasicAuthApi> gossipBuilder = HttpGossipBuilder<IHttpBinStaticBasicAuthApi>.NewDefaultBuilder();
            IHttpBinStaticBasicAuthApi api = gossipBuilder.Build();

            // Act & Assert
            api.Invoking(a => a.GetWithBasicAuth())
                .Should().Throw<HttpRequestException>()
                .Which.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
