# Gossip4Net
Gossip4Net is an extensible http client middleware similar to Spring Feign. It allows developers to easily consume APIs using an interface contract, which gets automatically implemented.

> **Note**
> Please read the [Not invented here](#not-invented-here) section, to learn if this is the right library for your use case.

1. [Getting started](#getting-started)
    1. [Get Gossip4Net](#0-get-gossip4net)
    1. [Define your API contract and models](#1-define-your-api-contract-and-models)
    1. [Obtain and configure a HttpGossipBuilder](#2-obtain-and-configure-a-httpgossipbuilder)
    1. [Let Gossip4Net implement your API interface](#3-let-gossip4net-implement-your-api-interface)
    1. [Use your API](#4-use-your-api)
1. [Not invented here](#not-invented-here)
1. [Feaures](#features)
    - [Url mapping](#url-mapping)
    - [Url mapping from code](#url-mapping-from-code)
    - [Path variables](#path-variables)
    - [Query variables](#query-variables)
    - [Static query values](#static-query-values)
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
1. [Extensibility](#extensibility)
    1. [The internals](#the-internals)
        1. [Big picture](#big-picture)
        1. [Registrations](#registrations)
        1. [Request modifiers](#request-modifiers)
        1. [Response modifiers](#response-modifiers)
        1. [Response constructors](#response-constructors)
    1. [Example (adding XML support) ](#example-adding-xml-support)
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

## Not invented here
This is (by far) not the first project enabling developers to consume an API by automatically implementing an interface. 
In fact, there is an awesome library called [Refit](https://github.com/reactiveui/refit), doing just that.

> **Note**
> For the sake of completeness, it has to be mentioned that there is also [DHaven.Faux](https://github.com/D-Haven/DHaven.Faux) and [feign.net](https://github.com/daixinkai/feign.net) heading in the same general direction (although I don't have firsthand experience with them).

### Why you should use Refit
[Refit](https://github.com/reactiveui/refit) is a widespread, stable and fast library, providing similar functionality. If you only intend to use "standard" features, it might well be the better choice for your use case - so go check it out!

### Why you should use Gossip4Net
The "issue" with auto-generating libraries is (IMHO), that you are somewhat limited to the functionalities intended by the project's maintainers.
Most of the time they will do just fine, but you are probably going to run into trouble the first you need to do something "special". 

What do you do now? Getting rid of the library and implementing everything manually (even the "standard" cases)?

In order to avoid having to ask these questions, Gossip4Net is focused on [extensibility](#extensibility). Even though this library is also providing some [standard features](#features), all of them are added dynamically and in the same way new behaviors could be added by any depending project.

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

If both are present and the url specified on the method mapping is relative, it will be combined with the url given in `[HttpApi]`.

```csharp
[HttpApi("https://httpbin.org")]
public interface MyApi {
    [GetMapping("/get")]
    Task<HttpResponseMessage> GetResponseAync();
}
```

The above example will result in a call to `https://httpbin.org/get`.

*Example path combinations*
| Base path                 | Extension | Result                            |
| ---------                 | --------- | ------                            |
| `https://localhost`       | `/api`    | `https://localhost/api`           |
| `https://localhost/api`   | `/person` | `https://localhost/api/person`    |
| `https://localhost/api`   | `person`  | `https://localhost/person`        |
| `https://localhost/api/`  | `person`  | `https://localhost/api/person`    |
| `https://localhost/api/`  | `/person` | `https://localhost/person`        | 

### Url mapping from code
You can also modify/specify urls directly from your code.
This can come in handy, if you for example need to use a different API version depending on the current environment (e.g test, staging, prod).

All following examples will refer to this API definition.
```csharp
[HttpApi("/get")]
public interface IRelativeHttpBinClient
{
    [GetMapping]
    Task<HttpBinResponse> Get();
}
```

To archive this, you can choose one of the following methods. 
#### By providing a custom http client
You can specify a custom `HttpClient` provider by setting the builder's `.ClientProvider` property.
This provider could then set an appropriate `BaseAddress`.

```csharp
IHttpGossipBuilder<IRelativeHttpBinClient> builder = new HttpGossipBuilder<IRelativeHttpBinClient>().AddDefaultBehavior(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
builder.ClientProvider = () => new HttpClient { BaseAddress = new Uri("https://httpbin.org") };

IRelativeHttpBinClient client = builder.Build();
```

#### By adding a custom behavior 
Gossip4Net focuses on extensibility, which enables you to freely add and remove different behaviors. To get and in-depth insight, refer to [Extensibility](#extensibility).

Request urls are handled by the `RequestUriModifier`, which takes an url and appends it to the currently processed http request.
By injecting a custom instance into the request chain, you can modify the url as you like.

*Adding a global request modifier using the helper method* 
```csharp
IRelativeHttpBinClient client = builder
    .WithRegistrations(r => r.With(new RequestUriModifier("https://httpbin.org")))
    .AddDefaultBehavior(new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    })
    .Build();
```

> **Note**
> The order of operation (adding the `RequestUriModifier` first, then applying the default behavior) is essential. 
Request modifiers are executed sequentially in order of their registration. If the `RequestUriModifier` was to be registered last,
the url `https://httpbin.org` would be appended to `/get` (set by the `[HttpApi]` attribute).

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

### Static query values
Similar to `[QueryVariable]`, the `[QueryValue(string name, object? value)]` attribute can be used to send a static header with every request.

```csharp
[HttpApi("https://httpbin.org")]
[QueryValue("json", "true")]
public interface MyApi {    
    [QueryValue("max", 100)]
    HttpResponseMessage Get();
}
```

*Available properties*
| Property | Description | Default |
|----------|-------------|---------|
| `Name` | The query parameter name. | none |
| `Value` | The value to be sent. | none |
| `EnumerateUsingMultipleParams`| If the parameter type is an `IEnumerable` and this is set to `true`, the query parameter name will be repeated for each entry. | `true` |

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

## Extensibility

### The internals

#### Big picture
A request/response-cycle always follows the process outlined below.

```text
actual paremeters are received
            |
            V
   create a plain request
            |
            V
   apply request modifiers
            |
            V
 obtain a HttpClient instance
            |
            V
       send request
            |
            V
  apply response modifiers
            |
            V
  apply response constructor
  (creates an instance of the 
    specified return type)
            |
            V
     return response
```

The entire behavior is therefore determined by the set of registered request and response modifiers.
In order to deterine the behaviors to be applied, the `HttpGossipBuilder<>` uses different modifier registrations.
These are responsible for registering intended behaviors based on certain criteria (e.g. an attribute is present).

```text
    plain HttpGossipBuilder<> is createad
                    |
                    V
    registrations are added to the builder
                    |
                    V
        .Build() method is invoked
                    |
                    V
    the builder scans the API interface
                    |
                    V
    registrations are invoked for each
        present interface member
                    |
                    V
    if applicable, behaviors are returned
        by the invoked registration
                    |
                    V
HttpGossipBuilder adds the returned behavior
        to the request/response cycle
                    |
                    V
 instance of the API interface is returned
```

#### Registrations
As outlined [above](#big-picture), a registration is responsible for adding specific behaviors based on the API interface to be implemented.

The interface to be implemented in order to register request modifiers, looks like that:

```csharp
public interface IRequestModifierRegistration
{
    IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IEnumerable<Attribute> attributes);
    IList<IHttpRequestModifier>? ForMethod(RequestMethodContext methodContext, IEnumerable<Attribute> attributes);
    IList<IHttpRequestModifier>? ForType(RequestTypeContext typeContext, IEnumerable<Attribute> attributes);
}
```

During the "implementation phase", the `ForType`, `ForMethod` and `ForParameter` methods are invoked by the builder.
This is done for the interface type itself, each declared method and for each of their parameters.
The registration's implementation can now decide, if the discovered member (or one of its attributes) justifies new behaviors to be applied. If so, a list of them are returned.

The `IResponseModiferRegistration` and `IResponseConstructorRegistration` are following the same general principle.

If a registration only intends to add behaviors based on an attribute of a given type, the `RequestModifierRegistration<TAttribute>` class can be inherited.

#### Request modifiers
A request modifier is responsible for manipulating a request before it gets send.
Note that it is only invoked during the request/response cycle following a call to an API interface method.

```csharp
public interface IHttpRequestModifier
{
    Task<HttpRequestMessage> ApplyAsync(HttpRequestMessage requestMessage, object?[] args);
}
```
It receivs the request to be modified as well as the actual parameters that have been passed into the interface method.
After the relevant modifications are done, it must return the manipulated request message.

#### Response modifiers
A response modifier is responsible for manipulating a received response, before they are converted in the API interface method's return type.

```csharp
public interface IHttpResponseModifier
{
    HttpResponseMessage Modify(HttpResponseMessage response);
}
```

### Response constructors
A response constructor is the last instance to be invoked, before returning the final result to the caller.
It is responsible for converting the manipulated http response into an object of the return type declared on the API's method.

```csharp
public interface IResponseConstructor
{
    public Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response);
}
```

If the implementation has been able to process the response, it should return `ConstructuredResponse.Of(...)`. 
If it has ben unable, it should usually return `ConstructedResponse.Empty` instead of throwing an exception (e.g. the `JsonResponseConstructor` receiving an "application/xml" response).

An exception should only be thrown, if it should have definitely been able to process it (e.g. if the `JsonResponseConstructor` receivs an "application/json" response, but the body contains syntax errors).

### Example (adding XML support) 
In this example we will add support for XML request and response bodies. This feature is actually availble with `Gossip4Net.Http.Xml`, but you could also implement it yourself using the steps outlined below.

#### Creating a project and adding dependencies
At first, we need to make sure to install all required dependencies.
If you want to separate Gossip4Net extensions from your "regular" code, you might as well create a new project within your solution.

In this this example, we are creating a new C# library project called `Gossip4Net.Http.Xml` and target .NET 6.

Next we need to be sure to install the following NuGet packages:
- System.Runtime.Serialization.Xml (providing XML serialization)
- Gossip4Net.Http

#### Creating a custom ResponseConstructor
The component that converts a http response into a model type is called "ResponseConstructor". 
In our example, it should check if the response contains XML and then deserialize it into the requested return type.

1. Create a new class and implement the IResponseConstructorInterface
```csharp
using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Xml.Modifier.Response
{
    internal class XmlResponseConstructor : IResponseConstructor
    {
        public async Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response)
        {
            throw new NotImplementedException();
        }
    }
}

```
2. Next we are going to check if the received response contains XML by checking the content type
```csharp
using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Xml.Modifier.Response
{
    internal class XmlResponseConstructor : IResponseConstructor
    {
        private static readonly ISet<string> SupportedMediaTypes = new HashSet<string>()
        {
            "application/xml",
            "text/xml"
        };

        public async Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response)
        {
            string? mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType == null || !SupportedMediaTypes.Contains(mediaType))
            {
                
            }
        }
    }
}
```
3. If the response doesn't contain XML, we give up by returning `ConstructedResponse.Empty`.
```csharp
if (mediaType == null || !SupportedMediaTypes.Contains(mediaType))
{
    return ConstructedResponse.Empty;
}
```
4. If it does, we will use a `XmlSerializer` to deserialize it. As we do not want to create a
`XmlSerializer` everytime a request is executed, we inject an apropriate serializer using the constructor.
```csharp
using Gossip4Net.Http.Modifier.Response;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Response
{
    internal class XmlResponseConstructor : IResponseConstructor
    {
        private readonly XmlSerializer xmlSerializer;

        public XmlResponseConstructor(XmlSerializer xmlSerializer)
        {
            this.xmlSerializer = xmlSerializer;
        }

        private static readonly ISet<string> SupportedMediaTypes = new HashSet<string>()
        {
            "application/xml",
            "text/xml"
        };

        public async Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response)
        {
            string? mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType == null || !SupportedMediaTypes.Contains(mediaType))
            {
                return ConstructedResponse.Empty;
            }
            object? responseObject = xmlSerializer.Deserialize(await response.Content.ReadAsStreamAsync());
        }
    }
}

```
5. Once we deserialized the object, we can return it by wrapping it into a `ConstructedResponse`.
```csharp
return new ConstructedResponse(responseObject);
```
6. The final implementation should now look like this:
```csharp
using Gossip4Net.Http.Modifier.Response;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Response
{
    internal class XmlResponseConstructor : IResponseConstructor
    {
        private readonly XmlSerializer xmlSerializer;

        public XmlResponseConstructor(XmlSerializer xmlSerializer)
        {
            this.xmlSerializer = xmlSerializer;
        }

        private static readonly ISet<string> SupportedMediaTypes = new HashSet<string>()
        {
            "application/xml",
            "text/xml"
        };

        public async Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response)
        {
            string? mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType == null || !SupportedMediaTypes.Contains(mediaType))
            {
                return ConstructedResponse.Empty;
            }
            object? responseObject = xmlSerializer.Deserialize(await response.Content.ReadAsStreamAsync());
            return new ConstructedResponse(responseObject);
        }
    }
}
```

#### Creating a response constructor registration
1. In order to use our new `XmlResponseConstructor`, we need to be able to inject it into the request/response process. To do so, we create an implementation of `IResponseConstructorRegistration`.
```csharp
using Gossip4Net.Http.Builder.Response;
using Gossip4Net.Http.Modifier.Response;
using Gossip4Net.Http.Modifier.Response.Registration;

namespace Gossip4Net.Http.Xml.Modifier.Response.Registration
{
    public class XmlResponseConstructorRegistration : IResponseConstructorRegistration
    {
        public IList<IResponseConstructor>? ForMethod(ResponseMethodContext responseMethod)
        {
            throw new NotImplementedException();
        }
    }
}
```
2. As methods returning `void` or `Task` don't need to process the response body at all, we don't want to return a response constructor in these cases.
Instead we can simply return `null` (or an empty list).
```csharp
public IList<IResponseConstructor>? ForMethod(ResponseMethodContext responseMethod)
{
    if (responseMethod.IsVoid)
    {
        return null;
    }
}
```
3. Otherwise we create a new `XmlSerializer` instance, capable of serializing the returned type.
```csharp
XmlSerializer xmlSerializer = new XmlSerializer(responseMethod.ProxyReturnType);
```
4. Now we can create a new instance of `XmlResponseConstructor` and return it. The implementation should now look like this:
```csharp
using Gossip4Net.Http.Builder.Response;
using Gossip4Net.Http.Modifier.Response;
using Gossip4Net.Http.Modifier.Response.Registration;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Modifier.Response.Registration
{
    public class XmlResponseConstructorRegistration : IResponseConstructorRegistration
    {
        public IList<IResponseConstructor>? ForMethod(ResponseMethodContext responseMethod)
        {
            if (responseMethod.IsVoid)
            {
                return null;
            }
            XmlSerializer xmlSerializer = new XmlSerializer(responseMethod.ProxyReturnType);
            return new List<IResponseConstructor>
            {
                new XmlResponseConstructor(xmlSerializer)
            };
        }
    }
}
```

#### Adding XML response support to the builder & testing it
1. First we need to define our API interface and models
```csharp
using FluentAssertions;
using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;
using Gossip4Net.Http.Xml.Modifier.Response.Registration;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Test
{
    [XmlRoot("slideshow")]
    public record Slideshow([property: XmlAttribute("title")] string Title, [property: XmlAttribute("author")] string Author)
    {
        // Default constructor required by XmlSerializer
        public Slideshow() : this(string.Empty, string.Empty) { }
    }

    [HttpApi("https://httpbin.org")]
    public interface IXmlHttpBinClient {

        [GetMapping("/xml")]
        Task<Slideshow> GetSlideshow();
    }
}
```
2. Next we create a test case and a builder targeting our API definition
```csharp
using FluentAssertions;
using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;
using Gossip4Net.Http.Xml.Modifier.Response.Registration;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Test
{
    public class GossipWithXmlTest
    {
        [Fact]
        public async Task XmlResponseBodiesShouldGetDeserialized()
        {
            // Arrange
            IHttpGossipBuilder<IXmlHttpBinClient> builder = new HttpGossipBuilder<IXmlHttpBinClient>();
        }
    }
}
```
3. Now we need to add the XmlResponseConstructorRegistration.
```csharp
// Arrange
IHttpGossipBuilder<IXmlHttpBinClient> builder = new HttpGossipBuilder<IXmlHttpBinClient>()
    .WithRegistrations(reg => reg.With(new XmlResponseConstructorRegistration()));
```
4. Don't forget to add the default behavior (as the builder would instead lack any other "basic" feature).
```csharp
// Arrange
IHttpGossipBuilder<IXmlHttpBinClient> builder = new HttpGossipBuilder<IXmlHttpBinClient>()
    .WithRegistrations(reg => reg.With(new XmlResponseConstructorRegistration()))
    .AddDefaultBehavior();
```
5. Use the `.Build()` method to implement your API definition and test the call.
```csharp
using FluentAssertions;
using Gossip4Net.Http.Attributes;
using Gossip4Net.Http.Attributes.Mappings;
using Gossip4Net.Http.Xml.Modifier.Response.Registration;
using System.Xml.Serialization;

namespace Gossip4Net.Http.Xml.Test
{
    public class GossipWithXmlTest
    {
        [Fact]
        public async Task XmlResponseBodiesShouldGetDeserialized()
        {
            // Arrange
            IHttpGossipBuilder<IXmlHttpBinClient> builder = new HttpGossipBuilder<IXmlHttpBinClient>()
                .WithRegistrations(reg => reg.With(new XmlResponseConstructorRegistration()))
                .AddDefaultBehavior();

            IXmlHttpBinClient client = builder.Build();

            // Act
            Slideshow slideshow = await client.GetSlideshow();

            // Assert
            slideshow.Title.Should().Be("Sample Slide Show");
            slideshow.Author.Should().Be("Yours Truly");
        }
    }
}
```

Congratulations - you've just implemented support for deserializing XML responses.

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