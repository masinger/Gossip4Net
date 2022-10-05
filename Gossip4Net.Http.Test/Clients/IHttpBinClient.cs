using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;

namespace Gossip4Net.Http.Test.Clients
{
    public record HttpBinResponse(IDictionary<string, string> headers, string origin, string url, IDictionary<string, string> args);
    public record HttpBinPostResponse<T>(IDictionary<string, string> headers, string origin, string url, IDictionary<string, string> args, string data, T json);

    public class Person {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public int Age { get; set; }
    }

    [HttpApi("https://httpbin.org")]
    [HeaderValue("Interface-Header", "interface")]
    [QueryValue("interface", true)]
    [QueryValue("interface-index", 3)]
    public interface IHttpBinClient
    {
        [GetMapping("/get")]
        Task<HttpBinResponse> GetAsync();

        [GetMapping("/get")]
        HttpBinResponse Get();

        [GetMapping("/get")]
        HttpResponseMessage GetResponse();

        [GetMapping("/get")]
        void MakeGetRequest();

        [GetMapping("/{operation}")]
        HttpBinResponse GetWithPathVariable([PathVariable] string operation);

        [GetMapping("/get")]
        HttpBinResponse GetWithQuery([QueryVariable] string testParam);

        [GetMapping("/get")]
        HttpBinResponse GetWithEnumerableQuery([QueryVariable] List<string> testParams);

        [PostMapping("/post")]
        HttpBinPostResponse<Person> Post(Person p);

        [PostMapping("/post")]
        Task<HttpBinPostResponse<Person>> PostAsync(Person p);

        [PostMapping("/post")]
        Task PostAsyncWithoutResult(Person p);

        [DeleteMapping("/delete")]
        Task<HttpBinResponse> DeleteAsyncWithHeader([HeaderVariable] string actor);

        [GetMapping("/get")]
        [HeaderValue("Method-Header", "method")]
        Task<HttpBinResponse> GetAyncWithStaticHeader();

        [GetMapping("/get")]
        [QueryValue("method", "present")]
        [QueryValue("method2", new string[] { "a", "b", "c" })]
        Task<HttpBinResponse> GetAsyncWithStaticQuery();
    }
}
