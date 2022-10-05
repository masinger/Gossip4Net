namespace Gossip4Net.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class QueryVariable : Attribute
    {
        public QueryVariable()
        {
        }

        public QueryVariable(string name)
        {
            Name = name;
        }

        public string? Name { get; init; }
        public bool OmitEmpty { get; init; } = true;
        public bool EnumerateUsingMultipleParams { get; init; } = true;
    }
}
