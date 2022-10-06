using Gossip4Net.Http.Test.Clients;
using FluentAssertions;
using System.Text.Json;
using Gossip4Net.Http.Attributes;

namespace Gossip4Net.Http.Test
{
    public class HttpGossipBuilderTest
    {
        private readonly IHttpBinClient client = HttpGossipBuilder<IHttpBinClient>.NewDefaultBuilder(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }
        ).Build();

        [Fact]
        public void BuildShouldReturnAnInterfaceImplementation()
        {
            // Arrange
            IGossipBuilder<IHttpBinClient> gossipBuilder = new HttpGossipBuilder<IHttpBinClient>();

            // Act
            IHttpBinClient client = gossipBuilder.Build();

            // Assert
            client.Should().NotBeNull();
        }

        [Fact]
        public void ClientShouldReturnHttpResponse()
        {
            // Act
            HttpResponseMessage result = client.GetResponse();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public void ClientShoulSupportVoidMethods()
        {
            // Act & Assert
            client.MakeGetRequest();
        }

        [Fact]
        public void ClientShoulReturnJsonBody()
        {
            // Act
            HttpBinResponse result = client.Get();

            // Assert
            result.Should().NotBeNull();
            result.Origin.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void PathVariablesShouldBeSubsituted()
        {
            // Act
            HttpBinResponse result = client.GetWithPathVariable("get");

            // Assert
            result.Should().NotBeNull();
            result.Origin.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void QueryVariablesShouldBeApplied()
        {
            // Act
            HttpBinResponse result = client.GetWithQuery("foo");

            // Assert
            result.Should().NotBeNull();
            result.Args.Should().ContainKey("testParam");
            result.Args["testParam"].Should().Be("foo");
        }

        [Fact]
        public void QueryVariablesShouldBeAppliedMultipleTimesForCollections()
        {
            // Act
            HttpBinResponse result = client.GetWithEnumerableQuery(new List<string>() { "a", "b", "c" });

            // Assert
            result.Should().NotBeNull();
            result.Args.Should().ContainKey("testParams");
            result.Args["testParams"].Should().Be("a,b,c");
        }

        [Fact]
        public void PostShouldSerializeTheBody()
        {
            // Act
            HttpBinPostResponse<Person> result = client.Post(new Person { Firstname = "Foo", Lastname = "Bar", Age = 42 });

            // Assert
            result.Should().NotBeNull();
            result.Json.Should().NotBeNull();
            result.Json.Firstname.Should().Be("Foo");
            result.Json.Lastname.Should().Be("Bar");
            result.Json.Age.Should().Be(42);
        }

        [Fact]
        public async Task AsyncMethodsShouldBeSupported()
        {
            // Act
            HttpBinPostResponse<Person> result = await client.PostAsync(new Person { Firstname = "Foo", Lastname = "Bar", Age = 42 });

            // Assert
            result?.Json.Should().NotBeNull();
        }


        [Fact]
        public async void AsyncVoidMethodsShouldBeSupported()
        {
            // Act & Assert
            await client.PostAsyncWithoutResult(new Person { Firstname = "Foo", Lastname = "Bar", Age = 42 });
        }

        [Fact]
        public async void HeaderVariablesShouldBeSupported()
        {
            // Act
            HttpBinResponse result = await client.DeleteAsyncWithHeader("bar");

            // Assert
            result.Headers.Should().ContainKey("Actor"); // httpbin returns header names capitalized 
            result.Headers["Actor"].Should().Be("bar");
        }

        [Fact]
        public async void FixedHeaderValueShoudBeSupported()
        {
            // Act
            HttpBinResponse result = await client.GetAyncWithStaticHeader();

            // Assert
            result.Headers["Method-Header"].Should().Be("method");
            result.Headers["Interface-Header"].Should().Be("interface");
        }

        [Fact]
        public async void FixedQueryValueShouldBeSupported()
        {
            // Act
            HttpBinResponse result = await client.GetAsyncWithStaticQuery();

            // Assert
            result.Args["interface"].Should().Be("True");
            result.Args["interface-index"].Should().Be("3");

            result.Args["method"].Should().Be("present");
            result.Args["method2"].Should().Be("a,b,c");
        }
    }
}