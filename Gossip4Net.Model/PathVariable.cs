namespace Gossip4Net.Model
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathVariable : Attribute
    {
        public string? Name { get; set; }
        public bool EscapePath { get; set; } = true;
    }
}
