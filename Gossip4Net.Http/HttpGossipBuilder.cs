using Gossip4Net.Http.Builder.Implementation;
using Gossip4Net.Http.Client;
using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Model;
using ImpromptuInterface;
using System.Reflection;
using System.Text.Json;

namespace Gossip4Net.Http
{
    public class HttpGossipBuilder<T> : IHttpGossipBuilder<T> where T:class
    {
        public Func<HttpClient> ClientProvider { get; set; } = () => new HttpClient();
        public JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions();

        public T Build()
        {
            Type t = typeof(T);
            List<IHttpRequestModifier> globalRequestModifiers = new List<IHttpRequestModifier>();
            HttpApi? apiAttribute = t.GetCustomAttribute<HttpApi>();
            if (apiAttribute != null)
            {
                if (apiAttribute.Url != null)
                {
                    globalRequestModifiers.Add(new RequestUriModifier(apiAttribute.Url));
                }
            }

            foreach (HeaderValue headerValue in t.GetCustomAttributes<HeaderValue>())
            {
                globalRequestModifiers.Add(new RequestStaticHeaderModifier(headerValue.Name, headerValue.Value));
            }

            IDictionary<ClientRegistration, RequestMethodImplementation> registrations = new Dictionary<ClientRegistration, RequestMethodImplementation>();
            MethodImplemantationBuilder methodImplemantationBuilder = new MethodImplemantationBuilder(JsonOptions, globalRequestModifiers, ClientProvider);

            foreach (var method in t.GetMethods())
            {
                KeyValuePair<ClientRegistration, RequestMethodImplementation> implementation = methodImplemantationBuilder.BuildImplementation(method);
                registrations[implementation.Key] = implementation.Value;
            }

            GossipHttpClient gossipHttpClient = new GossipHttpClient(registrations);
            return Impromptu.ActLike<T>(gossipHttpClient);
        }
    }
}
