namespace Gossip4Net.Http.Attributes.Mappings
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PutMapping : HttpMapping
    {
        public PutMapping() : base(HttpMethod.Put)
        {
        }

        public PutMapping(string path) : this()
        {
            Path = path;
        }
    }
}
