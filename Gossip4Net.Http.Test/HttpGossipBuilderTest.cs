using Gossip4Net.Http.Test.Clients;
using Gossip4Net.Model;
using FluentAssertions;

namespace Gossip4Net.Http.Test
{
    public class HttpGossipBuilderTest
    {
        private readonly IHttpBinClient client = new HttpGossipBuilder<IHttpBinClient>()
        {
            JsonOptions = new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            }
        }.Build();

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
            result.origin.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void PathVariablesShouldBeSubsituted()
        {
            // Act
            HttpBinResponse result = client.GetWithPathVariable("get");

            // Assert
            result.Should().NotBeNull();
            result.origin.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void QueryVariablesShouldBeApplied()
        {
            // Act
            HttpBinResponse result = client.GetWithQuery("foo");

            // Assert
            result.Should().NotBeNull();
            result.args.Should().ContainKey("testParam");
            result.args["testParam"].Should().Be("foo");
        }

        [Fact]
        public void QueryVariablesShouldBeAppliedMultipleTimesForCollections()
        {
            // Act
            HttpBinResponse result = client.GetWithEnumerableQuery(new List<string>() { "a", "b", "c" });

            // Assert
            result.Should().NotBeNull();
            result.args.Should().ContainKey("testParams");
            result.args["testParams"].Should().Be("a,b,c");
        }

        [Fact]
        public void PostShouldSerializeTheBody()
        {
            // Act
            HttpBinPostResponse<Person> result = client.Post(new Person { Firstname = "Foo", Lastname = "Bar", Age = 42 });

            // Assert
            result.Should().NotBeNull();
            result.json.Should().NotBeNull();
            result.json.Firstname.Should().Be("Foo");
            result.json.Lastname.Should().Be("Bar");
            result.json.Age.Should().Be(42);
        }

        [Fact]
        public async Task AsyncMethodsShouldBeSupported()
        {
            // Act
            HttpBinPostResponse<Person> result = await client.PostAsync(new Person { Firstname = "Foo", Lastname = "Bar", Age = 42 });

            // Assert
            result?.json.Should().NotBeNull();
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
            result.headers.Should().ContainKey("Actor"); // httpbin returns header names capitalized 
            result.headers["Actor"].Should().Be("bar"); 
        }

        [Fact]
        public async void FixedHeaderValueShoudBeSupported()
        {
            // Act
            HttpBinResponse result = await client.GetAyncWithStaticHeader();

            // Assert
            result.headers["Method-Header"].Should().Be("method");
            result.headers["Interface-Header"].Should().Be("interface");
        }

    }
}