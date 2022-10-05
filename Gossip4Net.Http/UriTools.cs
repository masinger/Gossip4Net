using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace Gossip4Net.Http
{
    internal static class UriTools
    {

        private static readonly Uri HelperUri = new Uri("http://localhost");

        internal static Uri AppendNonNull([NotNull] Uri baseUri, [NotNull] Uri nextUri)
        {
            if (nextUri.IsAbsoluteUri)
            {
                return nextUri;
            }
            Uri absoluteNextUri = new Uri(HelperUri, nextUri);
            Uri absoluteBaseUri = baseUri.IsAbsoluteUri ? baseUri : new Uri(HelperUri, baseUri);

            UriBuilder builder = new UriBuilder(absoluteBaseUri);
            if (
                !absoluteBaseUri.AbsolutePath.EndsWith("/") && nextUri.OriginalString.StartsWith("/")
            )
            {
                builder.Path = $"{absoluteBaseUri.AbsolutePath}{absoluteNextUri.AbsolutePath}";
            }
            else if (absoluteBaseUri.AbsolutePath.EndsWith("/") && nextUri.OriginalString != string.Empty && !nextUri.OriginalString.StartsWith("/"))
            {
                builder.Path = $"{absoluteBaseUri.AbsolutePath}{absoluteNextUri.AbsolutePath.Substring(1)}";
            }
            else
            {
                builder.Path = absoluteNextUri.AbsolutePath;
            }

            builder.Query = string.Join(
                '&',
                new List<string>
                {
                    absoluteBaseUri.Query,
                    absoluteNextUri.Query
                }
                .Where(it => !string.IsNullOrEmpty(it)));

            if (baseUri.IsAbsoluteUri)
            {
                return builder.Uri;
            }

            return HelperUri.MakeRelativeUri(builder.Uri);
        }

        public static Uri Append(this Uri? currentUri, string url)
        {
            Uri nextUri = ParseUri(url);
            if (currentUri == null)
            {
                return nextUri;
            }
            return AppendNonNull(currentUri, nextUri);
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
                items.Add(string.Concat(name, "=", HttpUtility.UrlEncode(parameters[name])));

            string fullQuery = string.Join("&", items.ToArray());

            UriBuilder uriBuilder = new UriBuilder(uri);
            uriBuilder.Query = fullQuery;

            return uriBuilder.Uri;
        }

    }
}
