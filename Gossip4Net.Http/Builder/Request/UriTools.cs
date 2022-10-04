using System.Collections.Specialized;

namespace Gossip4Net.Http.Builder.Request
{
    internal static class UriTools
    {
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
