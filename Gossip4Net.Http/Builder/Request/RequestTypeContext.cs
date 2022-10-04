namespace Gossip4Net.Http.Builder.Request
{
    public class RequestTypeContext
    {
        public Type Type { get; }

        public RequestTypeContext(
            Type type
        )
        {
            Type = type;
        }
    }
}
