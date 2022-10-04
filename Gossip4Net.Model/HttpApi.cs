namespace Gossip4Net.Model
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class HttpApi : Attribute
    {
        public string? Url { get; set; }

        public HttpApi()
        {
        }

        public HttpApi(string url)
        {
            Url = url;
        }
    }
}
