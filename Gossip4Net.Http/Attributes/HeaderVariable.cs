namespace Gossip4Net.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class HeaderVariable : Attribute
    {
        public string? Name { get; set; }
        public bool OmitEmpty { get; set; } = true;

        public HeaderVariable()
        {
        }

        public HeaderVariable(string name)
        {
            Name = name;
        }

    }
}
