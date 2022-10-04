using Gossip4Net.Model;

namespace Gossip4Net.Http.Modifier.Request
{
    internal class RequestPathVariableModifier : IHttpRequestModifier
    {
        private readonly Func<object?, string> valueConverter;
        private readonly IEnumerable<PathVariable> pathVariables;
        private readonly string? defaultName;
        private readonly int argIndex;

        public RequestPathVariableModifier(
            Func<object?, string> valueConverter,
            IEnumerable<PathVariable> pathVariables,
            string? defaultName,
            int argIndex
        )
        {
            this.valueConverter = valueConverter;
            this.pathVariables = pathVariables;
            this.defaultName = defaultName;
            this.argIndex = argIndex;
        }

        public Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args)
        {
            if (requestMessage.RequestUri == null)
            {
                throw new ArgumentException("The http request does not declare a request uri.");
            }

            object? arg = args[argIndex];
            string unescapedArgValue = valueConverter(arg);
            string escapedArgValue = Uri.EscapeDataString(unescapedArgValue);
            string currentUri = requestMessage.RequestUri.ToString();


            foreach (PathVariable pathVariable in pathVariables)
            {
                string variableName = (pathVariable.Name ?? defaultName) ?? throw new UnknownNameException($"Unknown path variable name for parameter at index {argIndex}.");
                string replacement = pathVariable.EscapePath ? escapedArgValue : unescapedArgValue;

                currentUri = currentUri.Replace("{" + variableName + "}", replacement);
            }

            requestMessage.RequestUri = UriTools.ParseUri(currentUri);

            return Task.FromResult(requestMessage);
        }
    }
}
