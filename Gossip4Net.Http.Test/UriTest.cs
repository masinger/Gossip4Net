using FluentAssertions;

namespace Gossip4Net.Http.Test
{
    public class UriTest
    {

        record UriTestCombination(Uri? Original, string Appendix, Uri Expected) {

            public UriTestCombination(string? original, string appendix, string expected) 
                : this(original != null ? new Uri(original) : null, appendix, new Uri(expected))
            {
            
            }

        }

        private static readonly IList<UriTestCombination> TestCombinations = new List<UriTestCombination>
        {
            new UriTestCombination(null, "https://localhost", "https://localhost"),
            new UriTestCombination("https://localhost", "", "https://localhost"),
            new UriTestCombination("https://localhost", "/api", "https://localhost/api"),
            new UriTestCombination("https://localhost/api/", "test/foo", "https://localhost/api/test/foo"),
            new UriTestCombination("https://localhost/api", "/test", "https://localhost/api/test"),
            new UriTestCombination("https://localhost/api", "test", "https://localhost/test"),
            new UriTestCombination("https://localhost/api/", "/test", "https://localhost/test")
        };

        [Fact]
        public void AppendingToAnUriShouldProduceTheExpectedResults()
        {
            foreach (UriTestCombination combination in TestCombinations)
            {
                combination.Original.Append(combination.Appendix).Should().Be(combination.Expected);
            }
        }
    }
}
