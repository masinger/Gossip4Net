namespace Gossip4Net.Http.Attributes.Mappings
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
