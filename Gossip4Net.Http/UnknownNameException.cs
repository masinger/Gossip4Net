using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
