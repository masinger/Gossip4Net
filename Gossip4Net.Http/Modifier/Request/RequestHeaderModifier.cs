using Gossip4Net.Http.Builder.Request;
using Gossip4Net.Model;

namespace Gossip4Net.Http.Modifier.Request
{
    internal class RequestHeaderModifier : IHttpRequestModifier
    {
        private readonly Func<object?, string> valueConverter;
        private readonly IEnumerable<HeaderVariable> headerVariables;
        private readonly string defaultName;
        private readonly int argIndex;

        public RequestHeaderModifier(Func<object?, string> valueConverter, IEnumerable<HeaderVariable> queryVariables, string defaultName, int argIndex)
        {
            this.valueConverter = valueConverter;
            headerVariables = queryVariables;
            this.defaultName = defaultName;
            this.argIndex = argIndex;
        }

        public Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args)
        {
            object? arg = args[argIndex];
            string unescapedArgValue = valueConverter(arg);

            foreach (HeaderVariable variable in headerVariables)
            {
                if (variable.OmitEmpty && args == null)
                {
                    continue;
                }
                requestMessage.Headers.Add(variable.Name ?? defaultName, unescapedArgValue);
            }

            return Task.FromResult(requestMessage);
        }
    }
}