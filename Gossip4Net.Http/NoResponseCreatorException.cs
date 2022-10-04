namespace Gossip4Net.Http
{
    public class NoResponseCreatorException : Exception
    {
        public NoResponseCreatorException() : this("There has been no response constructor that was able to process the received response.")
        {
        }

        public NoResponseCreatorException(string? message) : base(message)
        {
        }
    }
}
