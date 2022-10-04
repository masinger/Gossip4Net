﻿using Gossip4Net.Model;
using System.Web;
using System.Collections.Specialized;
using System.Collections;

namespace Gossip4Net.Http.Builder.Request
{
    internal class RequestQueryModifier : IHttpRequestModifier
    {
        private readonly Func<object?, string> valueConverter;
        private readonly IEnumerable<QueryVariable> queryVariables;
        private readonly string defaultName;
        private readonly int argIndex;

        public RequestQueryModifier(Func<object?, string> valueConverter, IEnumerable<QueryVariable> queryVariables, string defaultName, int argIndex)
        {
            this.valueConverter = valueConverter;
            this.queryVariables = queryVariables;
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

            NameValueCollection currentQuery = HttpUtility.ParseQueryString(requestMessage.RequestUri.Query);
           
            foreach (QueryVariable queryVariable in queryVariables)
            {
                if (queryVariable.OmitEmpty && arg == null)
                {
                    continue;
                }
                if (arg is IEnumerable enumberableArgs && arg is not string && queryVariable.EnumerateUsingMultipleParams)
                {
                    foreach (var enumerableArg in enumberableArgs)
                    {
                        currentQuery.Add(queryVariable.Name ?? defaultName, valueConverter(enumerableArg));
                    }
                }
                else
                {
                    currentQuery.Set(queryVariable.Name ?? defaultName, unescapedArgValue);
                }
            }

            requestMessage.RequestUri = requestMessage.RequestUri.WithQuery(currentQuery);

            return Task.FromResult(requestMessage);
        }
    }
}
