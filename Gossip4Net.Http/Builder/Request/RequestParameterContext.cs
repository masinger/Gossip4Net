using System.Reflection;

namespace Gossip4Net.Http.Builder.Request
{
    public class RequestParameterContext
    {
        public RequestMethodContext Method { get; set; }
        public ParameterInfo Parameter { get; }
        public int ParameterIndex { get; }

        public RequestParameterContext(
            RequestMethodContext method,
            ParameterInfo parameter,
            int parameterIndex
        )
        {
            Parameter = parameter;
            ParameterIndex = parameterIndex;
            Method = method;
        }
    }
}
