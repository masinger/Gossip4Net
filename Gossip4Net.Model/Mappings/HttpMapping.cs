namespace Gossip4Net.Model.Mappings
{
    public class HttpMapping : Attribute
    {
        public HttpMethod Method { get; }
        public string? Path { get; set; }

        public HttpMapping(HttpMethod method)
        {
            Method = method;
        }

    }
}
