using Gossip4Net.Http.Builder;
using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Client;
using Gossip4Net.Http.Modifier.Request;
using ImpromptuInterface;
using System.Reflection;
using System.Text.Json;

namespace Gossip4Net.Http
{
    public class HttpGossipBuilder<T> : IHttpGossipBuilder<T> where T:class
    {
        public static IHttpGossipBuilder<T> NewDefaultBuilder()
        {
            return new HttpGossipBuilder<T>().AddDefaultBehavior();
        }

        public static IHttpGossipBuilder<T> NewDefaultBuilder(JsonSerializerOptions jsonSerializerOptions)
        {
            return new HttpGossipBuilder<T>().AddDefaultBehavior(jsonSerializerOptions);
        }

        public HttpGossipBuilder()
        {
        }

        public Func<HttpClient> ClientProvider { get; set; } = () => new HttpClient();
        public Registrations Registrations { get; } = new Registrations();

        public T Build()
        {
            Type t = typeof(T);
            RequestTypeContext requestTypeContext = new RequestTypeContext(t);
            
            IList<Attribute> allTypeAttributes = t.GetCustomAttributes<Attribute>().ToList();
            List<IHttpRequestModifier> globalRequestModifiers = Registrations.RequestModifiers
                .Select(it => it.ForType(requestTypeContext, allTypeAttributes))
                .Where(it => it != null)
                .SelectMany(it => it!)
                .ToList();

            MethodImplemantationBuilder methodImplemantationBuilder = new MethodImplemantationBuilder(
                ClientProvider,
                globalRequestModifiers,
                Registrations
            );

            IDictionary<MethodSignature, RequestMethodImplementation> methodImplementations = new Dictionary<MethodSignature, RequestMethodImplementation>();
            foreach (MethodInfo method in t.GetMethods())
            {
                RequestMethodContext requestMethodContext = new RequestMethodContext(requestTypeContext, method);
                KeyValuePair<MethodSignature, RequestMethodImplementation> implementation = methodImplemantationBuilder.BuildImplementation(requestMethodContext);
                methodImplementations[implementation.Key] = implementation.Value;
            }

            GossipHttpClient gossipHttpClient = new GossipHttpClient(methodImplementations);
            return Impromptu.ActLike<T>(gossipHttpClient);
        }
    }
}
