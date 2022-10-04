using Microsoft.Extensions.DependencyInjection;

namespace Gossip4Net.Http.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddGossipHttpClient<T>(
            this IServiceCollection services,
            Action<IHttpGossipBuilder<T>> gossipConfig
        ) where T : class
        {
            return services.AddGossipHttpClient(null, gossipConfig);
        }

        public static IHttpClientBuilder AddGossipHttpClient<T>(
            this IServiceCollection services,
            Action<IServiceProvider, HttpClient>? httpConfig = null,
            Action<IHttpGossipBuilder<T>>? gossipConfig = null
        ) where T : class
        {
            if (httpConfig == null)
            {
                httpConfig = (a, b) => { };
            }
            string typeName = typeof(T).AssemblyQualifiedName!;

            return services
                .AddSingleton<IHttpGossipBuilder<T>>(sp =>
                {
                    IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                    HttpGossipBuilder<T> gossipBuilder = new HttpGossipBuilder<T>
                    {
                        ClientProvider = () =>
                        {
                            return httpClientFactory.CreateClient(typeName);
                        }
                    };

                    if (gossipConfig != null)
                    {
                        gossipConfig(gossipBuilder);
                    }

                    return gossipBuilder;
                })
                .AddSingleton<T>(sp => sp.GetRequiredService<IHttpGossipBuilder<T>>().Build())
                .AddHttpClient(typeName, httpConfig);
        }
    }
}