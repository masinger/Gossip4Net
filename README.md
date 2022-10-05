# Gossip4Net
Gossip4Net is an extensible http client middleware similar to Spring Feign. It allows developers to easily consume APIs using an interface contract, which gets automatically implemented.

1. [Getting started](#getting-started)
    1. [Get Gossip4Net](#0-get-gossip4net)
    1. [Define your API contract and models](#1-define-your-api-contract-and-models)
    1. [Obtain and configure a HttpGossipBuilder](#2-obtain-and-configure-a-httpgossipbuilder)
    1. [Let Gossip4Net implement your API interface](#3-let-gossip4net-implement-your-api-interface)
    1. [Use your API](#4-use-your-api)
1. [Feaures](#features)
    - [Url mapping](#url-mapping)
    - [Path variables](#path-variables)
    - [Query variables](#query-variables)
    - [Header variables](#header-variables)
    - [Static header values](#static-header-values)
    - [Raw http response](#raw-http-response)
    - [Async and synchronous requests](#async-and-synchronous-requests)
    - [Void methods](#void-methods)
    - [Request body](#request-body)
    - [Response body](#response-body)
    - [Json (de)serialization](#json-deserialization)
1. [Authentication](#authentication)
    - [Basic auth](#basic-auth)
    - [OpenID with client secret](#openid-with-client-secret)
1. [Testing](#testing)

## Getting started

### 0. Get Gossip4Net
For standalone usage:
```ps
Install-Package Gossip4Net.Http
```

For usage within ASP.NET core:
```ps
Install-Package Gossip4Net.Http.DependencyInjection
```


### 1. Define your API contract and models
```csharp
namespace MyDemo {
    public record HttpBinResponseDto(IDictionary<string, string> Headers);
    public record PersonRequestDto(string Fistname, string Lastname);

    [HttpApi("https://httpbin.org")]
    public interface HttpBinApi {

        [GetMapping("/get")]
        Task<HttpBinResponseDto> GetAsync();

        [GetMapping("/get")]
        HttpBinResponseDto GetSync();

        [PostMapping("/post")]
        Task PostAsync(Person person);
    }
}
```

### 2. Obtain and configure a HttpGossipBuilder
```csharp
using  Gossip4Net.Http;

namespace MyDemo {
    public class Demo {
        public async Task Startup() {
            IHttpGossipBuilder<HttpBinApi> builder = new HttpGossipBuilder<HttpBinApi>();
            builder.AddDefaultBehavior();
        }
    }
}
```


For ASP.NET core, dependency injection can be used:
```csharp
using  Microsoft.Extensions.DependencyInjection;

namespace AspNetDemo {

    public class Startup {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGossipHttpClient<HttpBinApi>();
        }
    }
}
```

### 3. Let Gossip4Net implement your API interface
```csharp
namespace MyDemo {
    public class Demo {
        public async Task Startup() {
            IHttpGossipBuilder<HttpBinApi> builder = new HttpGossipBuilder<HttpBinApi>();
            builder.AddDefaultBehavior();

            HttpBinApi api = builder.Build();
        }
    }
}
```

### 4. Use your API
```csharp
using  Gossip4Net.Http;

namespace MyDemo {
    public class Demo {
        public async Task Startup() {
            IHttpGossipBuilder<HttpBinApi> builder = new HttpGossipBuilder<HttpBinApi>();
            builder.AddDefaultBehavior();

            HttpBinApi api = builder.Build();
            HttpBinResponseDto response = await api.GetAync();
        }
    }
}
```

## Features

### Url mapping
The API-Url can be specified using the `[HttpApi]` attribute

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi {}
```

and/or using the method mapping attributes (e.g. `[GetMapping]`, `[PostMapping]`, `[PatchMapping]`):

```csharp
[HttpApi]
public interface MyApi {
    [GetMapping("https://httpbin.org/get")]
    Task<HttpResponseMessage> GetResponseAync();
}
```

If both are present and the url specified on the method mapping is relative, it will be resolved/appended to the url given in `[HttpApi]`.

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi {
    [GetMapping("/get")]
    Task<HttpResponseMessage> GetResponseAync();
}
```

The above example will result in a call to `https://httpbin.org/get`.

### Path variables
Parameter values can be interpolated into the request path using the `[PathVariable]` attribute.

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi {
    [GetMapping("/{method}")]
    Task<HttpResponseMessage> GetResponseAync([PathVariable] string method);

    [GetMapping("/{operation}")]
    Task<HttpResponseMessage> GetResponseWithExplicitPathVariableNameAync([PathVariable("operation")] string method);
}
```

By default, the placeholder name is determined by the parameter name (e.g. "method").
It can also be specified manually.

*Available properties*
| Property | Description | Default |
|----------|-------------|---------|
| `Name` | The path variable name. | The annotated parameter's name. |
| `EscapePath` | If `false`, special characters (like `/` and `?`) are not escaped. | `true` |

### Query variables
Parameter values can be send as a query parameter using the `[QueryVariable]` attribute.

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi {
    [GetMapping("/get")]
    HttpResponseMessage GetWithQuery([QueryVariable] string testParam);

    [GetMapping("/get")]
    HttpResponseMessage GetWithExplicitlyNamedQuery([QueryVariable("testParam")] string myValue);
}
```
By default, the query variable name is determined by the parameter name (e.g. "method").
It can also be specified manually.

*Available properties*
| Property | Description | Default |
|----------|-------------|---------|
| `Name` | The query parameter name. | The annotated parameter's name. |
| `OmitEmpty` | If `true`, the query parameter will be omitted for given `null` values. | `true` |
| `EnumerateUsingMultipleParams` | If the parameter type is an `IEnumerable` and this is set to `true`, the query parameter name will be repeated for each entry. | `true` |

### Header variables
Parameter values can be sent as request headers using the `[HeaderVariable]` attribute.

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi {
    [DeleteMapping("/delete")]
    Task<HttpResponseMessage> DeleteAsyncWithHeader([HeaderVariable] string actor);

    [DeleteMapping("/delete")]
    Task<HttpResponseMessage> DeleteAsyncWithExplicitHeader([HeaderVariable("actor")] string myValue);
}
```
By default, the header name is determined by the parameter name (e.g. "method").
It can also be specified manually.

*Available properties*
| Property | Description | Default |
|----------|-------------|---------|
| `Name` | The header name to be used. | The annotated parameter's name. |
| `OmitEmpty` | If `true`, the header will be omitted for given `null` values. | `true` |

### Static header values
In order to always send a specific header, the `[HeaderValue]` attribute can be applied to a method or to the entire interface.

```csharp
[HttpApi("https://httpbin.org")]
[HeaderValue("Interface-Header", "interface")]
public interface MyApi {
    [GetMapping("/get")]
    [HeaderValue("Method-Header", "method")]
    Task<HttpResponseMessage> GetAyncWithStaticHeader();
}
```

### Raw http response
If a method's return type is `HttpResponseMessage` or `Task<HttpResponseMessage>` the raw `HttpResponseMessage` will be returned.

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi {
    [GetMapping("/get")]
    Task<HttpResponseMessage> GetRawResponseAsnc();
}
```

> **Note**: Even though the response body will not be parsed and the entire http response body is getting returned, other response processing (e.g. checking the response status) still applies.

### Async and synchronous requests
You can send both asynchronous and synchronous requests.
If a request should be performed asynchronously is determined be the method's return type (being `Task<>` or `Task`) only.
Aync methods do not have to end with the `Async`-suffix.

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi
{
    [GetMapping("/get")]
    Task<HttpResponseMessage> GetAsync();

    [GetMapping("/get")]
    HttpResponseMessage Get();
}
```


### Void methods
Methods returning either `void` or `Task`, will not return anything.
Nevertheless, the response will still be received and processed (e.g. checking the response code) by the library.
A call will be blocked and a returned Task will not complete until a response is received.

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi
{
    [GetMapping("/get")]
    Task GetAsync();

    [GetMapping("/get")]
    void Get();
}
```

### Request body
Parameter values without an attribute will be serialized and sent using the request body.

```csharp
public class Person {
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public int Age { get; set; }
}

[HttpApi("https://httpbin.org")]
public interface MyApi
{
    [PostMapping("/post")]
    HttpResponseMessage Post(Person p);

    [PostMapping("/post")]
    Task<HttpResponseMessage> PostAsync(Person p);
}
```

The serialization format depends on the current configuration and can be customized using the `HttpGossipBuilder`.
The default is JSON.

**See also**:
- [Json (de)serialization](#json-deserialization)


### Response body
Gossip4Net will attempt to return an instance of the method's specified return type.
It will be deserialized using the response body.
The deserialization format depends on the current configuration and the received response headers.
It can be customized using the `HttpGossipBuilder`.

```csharp
public record HttpBinResponse(IDictionary<string, string> Headers, string Origin, string Url, IDictionary<string, string> Args);

[HttpApi("https://httpbin.org")]
public interface MyApi
{
    [GetMapping("/get")]
    Task<HttpBinResponse> GetAsync();

    [GetMapping("/get")]
    HttpBinResponse Get();
}
```

### Json (de)serialization
By default, JSON is used to serialize request and response bodies.

JSON serialization is added by the `JsonRequestBodyRegistration` and `JsonResponseConstructorRegistration`. 


You can configure the JSON serializer using one of the following extension/helper methods.

*Constructing a builder and adding default behavior*
```csharp
IHttpGossipBuilder<MyApi> builder = new HttpGossipBuilder<MyApi>()
    .AddDefaultBehavior(new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    });
```

*Creating a builder using a builder helper method*
```csharp
IHttpGossipBuilder<MyApi> builder = HttpGossipBuilder<MyApi>.NewDefaultBuilder(
    new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    });
```

*While using dependency injection*
```csharp
IServiceCollection services = new ServiceCollection();
services.AddGossipHttpClient<MyApi>(
    builder => builder.AddDefaultBehavior(new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    })
);
```


If you don't intend to use the helper/extension methods, you can add JSON support like this:
```csharp
IHttpGossipBuilder<MyApi> builder = new HttpGossipBuilder<MyApi>();
JsonSerializerOptions options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};
builder.Registrations.RequestAttributes.Add(new JsonRequestBodyRegistration(options));
builder.Registrations.ResponseConstructors.Add(new JsonResponseConstructorRegistration(options));
```

## Authentication
Authentication can be implemented using third-party `HttpClient` middleware tools (like the common [IdentityModel project](https://github.com/IdentityModel/IdentityModel) or another library of your choice).
The main entry point for configuring authentication is always the `IHttpGossipBuilder<>.ClientProvider` property, which holds a factory method returning a fresh `HttpClient` instance.

Refer to the examples below for suggestions on how certain authentication methods can be configured. If not stated otherwise, [IdentityModel project](https://github.com/IdentityModel/IdentityModel) will be used.

> **Warning**
> The examples are written to be as concise and minimal as possible. 
Common and required security measures (e.g. obtaining passwords and secrets from secure sources) are omitted for clearity. 
Please consult the documentation relevant to your setup and tooling, in order to learn about safe ways to handle secrets. 
>
>A good starting point might be the following MSDN documentation:
>-  [Safe storage of app secrets in development in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)


### Basic Auth

*API definition*
```csharp
[HttpApi("https://httpbin.org")]
public interface IHttpBinStaticBasicAuthApi
{
    [GetMapping("/basic-auth/myuser/mypassword")]
    HttpResponseMessage GetWithBasicAuth();
}
```

*Example usage with basic auth*
```csharp
public class BasicAuthTest
{
    [Fact]
    public void HttpClientWithBasicAuthShouldWork()
    {
        // Arrange
        IHttpGossipBuilder<IHttpBinStaticBasicAuthApi> gossipBuilder = HttpGossipBuilder<IHttpBinStaticBasicAuthApi>.NewDefaultBuilder();
        gossipBuilder.ClientProvider = () =>
        {
            HttpClient httpClient = new HttpClient();
            httpClient.SetBasicAuthentication("myuser", "mypassword");
            return httpClient;
        };
        IHttpBinStaticBasicAuthApi api = gossipBuilder.Build();

        // Act
        HttpResponseMessage response = api.GetWithBasicAuth();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
```

### OpenID with client secret
This example uses a client id and secret in order to authenticate against an OIDC identity provider.
The token retrival and management is done automatically by IdentityModel. 

*API definition*
```csharp
[HttpApi("https://demo.duendesoftware.com/api/test")]
public interface IOidcApi
{
    [GetMapping]
    HttpResponseMessage Get();
}
```

*Example usage with ASP.NET core and IdentityModel.AspNetCore*
```csharp
public class OidcAuthTest
{
    [Fact]
    public void FullExampleShouldWord()
    {
        // Arrange
        ServiceCollection services = new ServiceCollection();
        services.AddAccessTokenManagement(options => // configuring the access token managenment
        {
            options.Client.Clients.Add("demo", new ClientCredentialsTokenRequest
            {
                Address = "https://demo.duendesoftware.com/connect/token",
                ClientId = "m2m",
                ClientSecret = "secret"
            });
        }).ConfigureBackchannelHttpClient();

        services.AddGossipHttpClient<IOidcApi>()
            .AddClientAccessTokenHandler("demo"); // binding the access token handler to the http client used by Gossi4Net

        ServiceProvider sp = services.BuildServiceProvider();
        IOidcApi client = sp.GetRequiredService<IOidcApi>();

        // Act
        HttpResponseMessage result = client.Get();

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
    }
}
```

## Testing
Testing a component that relies on the API is as easy as just implementing/mocking the API interface.

Assuming your API definition, model and service are looking like these:
```csharp
namespace Demo
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
```

Then your service test could look tike that (using Moq and FluentAssertions):

```csharp
public class DemoMockTest
{
    [Fact]
    public async Task CountHeadersShouldReturnNumberOfReceivedHeaders()
    {
        // Arrange
        Mock<IExampleApi> apiMock = new Mock<IExampleApi>();
        apiMock.Setup(api => api.Get())
        .ReturnsAsync(new ExampleResponse(
            Headers: new Dictionary<string, string> { { "Content-Type", "Example" }, { "Foo", "Bar" } },
            Origin: "a string",
            Url: "a url",
            Args: new Dictionary<string, string>()
        ));
        
        IExampleApi exampleApi = apiMock.Object;
        ExampleService serviceUnderTest = new ExampleService(exampleApi);

        // Act
        int headerCount = await serviceUnderTest.CountHeaders();

        // Assert
        headerCount.Should().Be(2);
    }
}
```