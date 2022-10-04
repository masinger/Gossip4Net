using Microsoft.Extensions.DependencyInjection;
using IdentityModel.Client;
using Gossip4Net.Http.Test.Clients;
using FluentAssertions;
using Gossip4Net.Http.DependencyInjection;

namespace Gossip4Net.Http.Test
{
    public class OIDCAuthTest
    {
        private ServiceCollection CreateServiceCollectionWithAccessTokenManagement()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("demo", new ClientCredentialsTokenRequest
                {
                    Address = "https://demo.duendesoftware.com/connect/token",
                    ClientId = "m2m",
                    ClientSecret = "secret"
                });
            }).ConfigureBackchannelHttpClient();

            return services;
        }

        [Fact]
        public void RequestsWithAuthShouldWorkWhenSetupUsingExtensionMethods()
        {
            // Arrange
            ServiceCollection services = CreateServiceCollectionWithAccessTokenManagement();

            services.AddGossipHttpClient<IDuendeDemo>()
                .AddClientAccessTokenHandler("demo");

            ServiceProvider sp = services.BuildServiceProvider();
            IDuendeDemo client = sp.GetRequiredService<IDuendeDemo>();

            // Act
            HttpResponseMessage result = client.Get();

            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public void RequestsWithAuthShouldWorkWhenSetupManual()
        {
            // Arrange
            ServiceCollection services = CreateServiceCollectionWithAccessTokenManagement();

            services.AddHttpClient("demo")
                    .AddClientAccessTokenHandler("demo");

            services.AddSingleton(prov =>
            {
                Func<HttpClient> clientProvider = () => prov.GetRequiredService<IHttpClientFactory>().CreateClient("demo");
                return new HttpGossipBuilder<IDuendeDemo>() {  ClientProvider = clientProvider }.AddDefaultBehavior().Build();
            });

            ServiceProvider sp = services.BuildServiceProvider();
            IDuendeDemo client = sp.GetRequiredService<IDuendeDemo>();

            // Act
            HttpResponseMessage result = client.Get();

            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
