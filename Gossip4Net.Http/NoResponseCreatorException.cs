namespace Gossip4Net.Http
{
    public class NoResponseCreatorException : Exception
    {
        public NoResponseCreatorException(HttpResponseMessage responseMessage)
            : base($"There has been no response constructor that was able to process the received response (see {nameof(Response)} property for further details).")
        {
            Response = responseMessage;
        }

        public HttpResponseMessage Response { get; }
    }
}
