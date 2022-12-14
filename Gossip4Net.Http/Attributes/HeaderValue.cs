namespace Gossip4Net.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
    public class HeaderValue : Attribute
    {
        public HeaderValue(
            string name,
            string value
        )
        {
            Name = name;
            Value = value;
        }

        public string Name { get; init; }
        public string Value { get; init; }
    }
}
