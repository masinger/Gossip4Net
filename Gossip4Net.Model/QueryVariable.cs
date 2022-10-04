namespace Gossip4Net.Model
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class QueryVariable : Attribute
    {
        public string? Name { get; set; }
        public bool OmitEmpty { get; set; } = true;
        public bool EnumerateUsingMultipleParams { get; set; } = true;
    }
}
