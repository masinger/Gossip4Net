# Gossip4Net
Gossip4Net is an extensible http client middleware similar to Spring Feign. It allows developers to easily consume APIs using an interface contract, which gets automatically implemented.

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
using Gossip4Net.Http.DependencyInjection;


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
using  Gossip4Net.Http;

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