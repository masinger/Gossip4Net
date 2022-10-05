using FluentAssertions;
using Gossip4Net.Http.Test.Clients;
using Moq;

namespace Gossip4Net.Http.Test
{
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
}
