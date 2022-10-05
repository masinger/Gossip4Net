using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Builder;

namespace Gossip4Net.Http
{
    public interface IHttpGossipBuilder<T> : IGossipBuilder<T>
    {
        public Func<HttpClient> ClientProvider { get; set; }
        public Registrations Registrations { get; }

    }
}