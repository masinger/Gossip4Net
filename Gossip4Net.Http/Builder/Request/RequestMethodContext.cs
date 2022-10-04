using System.Reflection;

namespace Gossip4Net.Http.Builder.Request
{
    public class RequestMethodContext
    {
        public RequestTypeContext TypeContext { get; }
        public MethodInfo MethodInfo { get; }

        public RequestMethodContext(
            RequestTypeContext typeContext,
            MethodInfo methodInfo
        )
        {
            TypeContext = typeContext;
            MethodInfo = methodInfo;
        }
    }
}
