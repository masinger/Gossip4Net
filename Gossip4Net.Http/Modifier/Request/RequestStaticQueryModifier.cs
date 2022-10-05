using System.Collections;
using System.Collections.Specialized;
using System.Web;

namespace Gossip4Net.Http.Modifier.Request
{
    internal class RequestStaticQueryModifier : IHttpRequestModifier
    {
        private readonly string name;
        private readonly object? value;
        private readonly Func<object?, string> valueConverter;
        private readonly bool enumerateUsingMultipleParams;

        public RequestStaticQueryModifier(
            string name,
            object? value,
            Func<object?, string> valueConverter,
            bool enumerateUsingMultipleParams)
        {
            this.name = name;
            this.value = value;
            this.valueConverter = valueConverter;
            this.enumerateUsingMultipleParams = enumerateUsingMultipleParams;
        }

        public Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args)
        {
            if (requestMessage.RequestUri == null)
            {
                throw new ConfigurationException("The http request does not declare a request uri.");
            }

            NameValueCollection currentQuery = HttpUtility.ParseQueryString(requestMessage.RequestUri.Query);
            if (enumerateUsingMultipleParams && value is IEnumerable enumerableValues && value is not string)
            {
                foreach (var entry in enumerableValues)
                {
                    currentQuery.Add(name, valueConverter(entry));
                }
            }
            else
            {
                currentQuery.Set(name, valueConverter(value));
            }

            requestMessage.RequestUri = requestMessage.RequestUri.WithQuery(currentQuery);

            return Task.FromResult(requestMessage);
        }
    }
}
