using System;
using System.Collections.Specialized;

namespace Gossip4Net.Http
{
    internal static class UriTools
    {

        public static Uri Append(this Uri? currentUri, string url)
        {
            Uri.TryCreate(url, UriKind.Absolute, out Uri? absoluteUri);
            Uri.TryCreate(url, UriKind.Relative, out Uri? relativeUri);
            if (currentUri == null || absoluteUri != null)
            {
                return absoluteUri ?? relativeUri ?? throw new ConfigurationException($"Invalid uri '{url}'.");
            }
            if (relativeUri == null)
            {
                throw new ConfigurationException($"Invalid uri '{url}'.");
            }
            if (relativeUri.ToString().StartsWith("/") && !currentUri.ToString().EndsWith("/"))
            {
                return new Uri(currentUri.ToString() + url);
            }
            return new Uri(currentUri, relativeUri);
        }

        public static Uri ParseUri(string uri)
        {
            Uri? absoluteUri;
            if (Uri.TryCreate(uri, UriKind.Absolute, out absoluteUri))
            {
                return absoluteUri;
            }

            Uri? relativeUri;
            if (Uri.TryCreate(uri, UriKind.Relative, out relativeUri))
            {
                return relativeUri;
            }

            throw new ArgumentException($"Invalid uri '{uri}'.", nameof(uri));
        }


        public static Uri WithQuery(this Uri uri, NameValueCollection parameters)
        {
            List<string> items = new List<string>();

            foreach (string name in parameters)
                items.Add(string.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));

            string fullQuery = string.Join("&", items.ToArray());

            UriBuilder uriBuilder = new UriBuilder(uri);
            uriBuilder.Query = fullQuery;

            return uriBuilder.Uri;
        }

    }
}
