using Gossip4Net.Http.Builder.Implementation;
using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Client;
using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Http.Modifier.Request.Registrations;
using Gossip4Net.Model;
using ImpromptuInterface;
using System.Reflection;
using System.Text.Json;

namespace Gossip4Net.Http
{
    public class HttpGossipBuilder<T> : IHttpGossipBuilder<T> where T:class
    {
        private static readonly Func<object?, string> DefaultValueConverter = o => "" + o;

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

            List<IRequestAttributeRegistration> requestAttributeRegistrations = new List<IRequestAttributeRegistration>() // TODO: Inject
            {
                new HttpMappingRegistration(),
                new HeaderValueRegistration(),
                new PathVariableRegistration(DefaultValueConverter),
                new QueryVariableRegistration(DefaultValueConverter),
                new HeaderVariableRegistration(DefaultValueConverter),
                new RequestBodyRegistration(JsonOptions),
            };

            MethodImplemantationBuilder methodImplemantationBuilder = new MethodImplemantationBuilder(JsonOptions, globalRequestModifiers, ClientProvider, requestAttributeRegistrations);

            foreach (MethodInfo method in t.GetMethods())
            {
                RequestMethodContext requestMethodContext = new RequestMethodContext(t, method);
                KeyValuePair<ClientRegistration, RequestMethodImplementation> implementation = methodImplemantationBuilder.BuildImplementation(requestMethodContext);
                registrations[implementation.Key] = implementation.Value;
            }

            GossipHttpClient gossipHttpClient = new GossipHttpClient(registrations);
            return Impromptu.ActLike<T>(gossipHttpClient);
        }
    }
}
