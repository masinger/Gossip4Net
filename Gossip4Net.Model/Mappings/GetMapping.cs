namespace Gossip4Net.Model.Mappings
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GetMapping : HttpMapping
    {
        public GetMapping() : base(HttpMethod.Get)
        {
        }

        public GetMapping(string path) : this()
        {
            Path = path;
        }
    }
}
