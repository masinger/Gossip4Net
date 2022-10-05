using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;

namespace Gossip4Net.Http.Test.Clients
{
    [HttpApi(
        Url = "https://demo.duendesoftware.com/api/test"
    )]
    public interface IDuendeDemo
    {
        [GetMapping]
        HttpResponseMessage Get();
    }
}
