using Gossip4Net.Model;
using Gossip4Net.Model.Mappings;

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
