using System.Runtime.Serialization;

namespace Gossip4Net.Http
{
    public class UnknownNameException : Exception
    {
        public UnknownNameException()
        {
        }

        public UnknownNameException(string? message) : base(message)
        {
        }

        public UnknownNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnknownNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
