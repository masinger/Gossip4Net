using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;

namespace Gossip4Net.Http.Test.Auth
{
    [HttpApi("https://httpbin.org")]
    public interface IHttpBinStaticBasicAuthApi
    {
        [GetMapping("/basic-auth/myuser/mypassword")]
        HttpResponseMessage GetWithBasicAuth();
    }
}
