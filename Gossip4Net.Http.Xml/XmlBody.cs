namespace Gossip4Net.Http.Xml
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class XmlBody : Attribute {

        public bool OmitEmpty { get; init; } = true;

    }
}
