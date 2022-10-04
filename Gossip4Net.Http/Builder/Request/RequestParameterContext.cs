using System.Reflection;

namespace Gossip4Net.Http.Builder.Request
{
    public class RequestParameterContext
    {
        public RequestMethodContext MethodContext { get; set; }
        public ParameterInfo Parameter { get; }
        public int ParameterIndex { get; }

        public RequestParameterContext(
            RequestMethodContext methodContext,
            ParameterInfo parameter,
            int parameterIndex
        )
        {
            Parameter = parameter;
            ParameterIndex = parameterIndex;
            MethodContext = methodContext;
        }
    }
}
