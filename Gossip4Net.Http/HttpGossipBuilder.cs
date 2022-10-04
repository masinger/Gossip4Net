using Gossip4Net.Http.Builder;
using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Http.Client;
using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Http.Modifier.Request.Registration;
using Gossip4Net.Http.Modifier.Response.Registration;
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
            RequestTypeContext requestTypeContext = new RequestTypeContext(t);

            IList<IRequestAttributeRegistration> requestAttributeRegistrations = new List<IRequestAttributeRegistration>() // TODO: Inject
            {
                new HttpApiRegistration(),
                new HttpMappingRegistration(),
                new HeaderValueRegistration(),
                new PathVariableRegistration(DefaultValueConverter),
                new QueryVariableRegistration(DefaultValueConverter),
                new HeaderVariableRegistration(DefaultValueConverter),
                new RequestBodyRegistration(JsonOptions),
            };

            IList<Attribute> allTypeAttributes = t.GetCustomAttributes<Attribute>().ToList();
            List<IHttpRequestModifier> globalRequestModifiers = requestAttributeRegistrations
                .Select(it => it.ForType(requestTypeContext, allTypeAttributes))
                .Where(it => it != null)
                .SelectMany(it => it!)
                .ToList();

            IList<IResponseConstructorRegistration> responseConstructorRegistrations = new List<IResponseConstructorRegistration>()
            {
                new RawResponseConstructorRegistration(),
                new JsonResponseConstructorRegistration(JsonOptions)
            };

            IList<IResponseAttributeRegistration> responseAttributeRegistrations = new List<IResponseAttributeRegistration>() // TODO: Inject
            {
                new ResponseSuccessRegistration()
            };

            MethodImplemantationBuilder methodImplemantationBuilder = new MethodImplemantationBuilder(
                ClientProvider,
                globalRequestModifiers,
                requestAttributeRegistrations,
                responseAttributeRegistrations,
                responseConstructorRegistrations
            );

            IDictionary<ClientRegistration, RequestMethodImplementation> registrations = new Dictionary<ClientRegistration, RequestMethodImplementation>();
            foreach (MethodInfo method in t.GetMethods())
            {
                RequestMethodContext requestMethodContext = new RequestMethodContext(requestTypeContext, method);
                KeyValuePair<ClientRegistration, RequestMethodImplementation> implementation = methodImplemantationBuilder.BuildImplementation(requestMethodContext);
                registrations[implementation.Key] = implementation.Value;
            }

            GossipHttpClient gossipHttpClient = new GossipHttpClient(registrations);
            return Impromptu.ActLike<T>(gossipHttpClient);
        }
    }
}
