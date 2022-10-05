using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;

namespace Gossip4Net.Http.Test.Clients
{
    public record ExampleResponse(
        IDictionary<string, string> Headers,
        string Origin,
        string Url,
        IDictionary<string, string> Args);

    [HttpApi("https://httpbin.org")]
    public interface IExampleApi
    {
        [GetMapping("/get")]
        Task<ExampleResponse> Get();
    }

    public class ExampleService
    {
        private readonly IExampleApi exampleApi;

        public ExampleService(IExampleApi exampleApi)
        {
            this.exampleApi = exampleApi;
        }

        public async Task<int> CountHeaders()
        {
            return (await exampleApi.Get()).Headers.Count;
        }
    }
}
