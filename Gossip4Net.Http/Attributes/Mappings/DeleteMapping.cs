namespace Gossip4Net.Http.Attributes.Mappings
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DeleteMapping : HttpMapping
    {
        public DeleteMapping() : base(HttpMethod.Delete)
        {
        }

        public DeleteMapping(string path) : this()
        {
            Path = path;
        }
    }
}
