namespace Gossip4Net.Model
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
    public class HeaderValue : Attribute
    {
        public string Name { get; }
        public string Value { get; }

        public HeaderValue(
            string name,
            string value
        )
        {
            Name = name;
            Value = value;
        }
    }
}
