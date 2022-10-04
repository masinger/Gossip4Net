using Gossip4Net.Model;
using System.Text.Json;

namespace Gossip4Net.Http
{
    public interface IHttpGossipBuilder<T> : IGossipBuilder<T>
    {
        public Func<HttpClient> ClientProvider { get; set; }
        public JsonSerializerOptions JsonOptions { get; set; }
    }
}