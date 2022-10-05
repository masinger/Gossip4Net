namespace Gossip4Net.Http.Attributes.Mappings
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PatchMapping : HttpMapping
    {
        public PatchMapping() : base(HttpMethod.Put)
        {
        }

        public PatchMapping(string path) : this()
        {
            Path = path;
        }
    }
}
