using Microsoft.Extensions.DependencyInjection;
using IdentityModel.Client;
using FluentAssertions;
using Gossip4Net.Http.DependencyInjection;

namespace Gossip4Net.Http.Test.Auth
{
    public class OidcAuthTest
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
        public void FullExampleShouldWord()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddAccessTokenManagement(options => // configuring the access token managenment
            {
                options.Client.Clients.Add("demo", new ClientCredentialsTokenRequest
                {
                    Address = "https://demo.duendesoftware.com/connect/token",
                    ClientId = "m2m",
                    ClientSecret = "secret"
                });
            }).ConfigureBackchannelHttpClient();

            services.AddGossipHttpClient<IOidcApi>()
                .AddClientAccessTokenHandler("demo"); // binding the access token handler to the http client used by Gossi4Net

            ServiceProvider sp = services.BuildServiceProvider();
            IOidcApi client = sp.GetRequiredService<IOidcApi>();

            // Act
            HttpResponseMessage result = client.Get();

            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
        }


        [Fact]
        public void RequestsWithAuthShouldWorkWhenSetupUsingExtensionMethods()
        {
            // Arrange
            ServiceCollection services = CreateServiceCollectionWithAccessTokenManagement();

            services.AddGossipHttpClient<IOidcApi>()
                .AddClientAccessTokenHandler("demo");

            ServiceProvider sp = services.BuildServiceProvider();
            IOidcApi client = sp.GetRequiredService<IOidcApi>();

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
                return new HttpGossipBuilder<IOidcApi>() { ClientProvider = clientProvider }.AddDefaultBehavior().Build();
            });

            ServiceProvider sp = services.BuildServiceProvider();
            IOidcApi client = sp.GetRequiredService<IOidcApi>();

            // Act
            HttpResponseMessage result = client.Get();

            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
