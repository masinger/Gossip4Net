namespace Gossip4Net.Http.Modifier.Request
{
    public class RequestBodyModifier : IHttpRequestModifier
    {

        private readonly int argIndex;
        private readonly Func<object?, Task<HttpContent>> contentBuilder;
        private readonly bool omitNull;

        public RequestBodyModifier(int argIndex, Func<object?, Task<HttpContent>> contentBuilder, bool omitNull)
        {
            this.argIndex = argIndex;
            this.contentBuilder = contentBuilder;
            this.omitNull = omitNull;
        }

        public async Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args)
        {
            object? arg = args[argIndex];

            if (arg == null && omitNull)
            {
                return requestMessage;
            }

            requestMessage.Content = await contentBuilder(arg);
            return requestMessage;
        }
    }
}
