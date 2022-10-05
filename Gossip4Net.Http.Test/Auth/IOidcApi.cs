using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;

namespace Gossip4Net.Http.Test.Auth
{
    [HttpApi("https://demo.duendesoftware.com/api/test")]
    public interface IOidcApi
    {
        [GetMapping]
        HttpResponseMessage Get();
    }
}
