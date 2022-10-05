namespace Gossip4Net.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
    public class QueryValue : Attribute
    {
        public QueryValue(
            string name,
            object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; init; }
        public object Value { get; init; }
        public bool EnumerateUsingMultipleParams { get; init; } = true;
    }
}
