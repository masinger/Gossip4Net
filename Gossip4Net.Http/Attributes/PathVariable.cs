namespace Gossip4Net.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathVariable : Attribute
    {
        public string? Name { get; set; }
        public bool EscapePath { get; set; } = true;

        public PathVariable()
        {
        }

        public PathVariable(string name)
        {
            Name = name;
        }

    }
}
