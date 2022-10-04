using System.Reflection;

namespace Gossip4Net.Http.Builder.Request
{
    public class RequestMethodContext
    {
        public Type TypeInfo { get; }
        public MethodInfo MethodInfo { get; }

        public RequestMethodContext(
            Type typeInfo,
            MethodInfo methodInfo
        )
        {
            TypeInfo = typeInfo;
            MethodInfo = methodInfo;
        }
    }
}
