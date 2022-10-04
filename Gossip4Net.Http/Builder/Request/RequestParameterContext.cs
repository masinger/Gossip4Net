using System.Reflection;

namespace Gossip4Net.Http.Builder.Request
{
    public class RequestParameterContext
    {
        public Type TypeInfo { get; }
        public ParameterInfo Parameter { get; }
        public int ParameterIndex { get; }

        public RequestParameterContext(
            Type typeInfo,
            ParameterInfo parameter,
            int parameterIndex
        )
        {
            Parameter = parameter;
            ParameterIndex = parameterIndex;
            TypeInfo = typeInfo;
        }
    }
}
