using System.Reflection;

namespace Gossip4Net.Http.Builder.Response
{
    public class ResponseMethodContext
    {

        public Type ProxyReturnType { get; }
        public MethodInfo MethodInfo { get; }
        public bool IsVoid => ProxyReturnType.Equals(typeof(void)) || ProxyReturnType.IsVoidTask();

        public ResponseMethodContext(
            Type proxyReturnType,
            MethodInfo methodInfo
        )
        {
            ProxyReturnType = proxyReturnType;
            MethodInfo = methodInfo;
        }

    }
}
