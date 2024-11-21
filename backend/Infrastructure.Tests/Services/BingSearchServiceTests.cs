using Domain.Constants;
using Infrastructure.Services.SearchServices;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace Infrastructure.Tests.Services
{
    public class BingSearchServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly BingSearchService _bingSearchService;

        public BingSearchServiceTests()
        {
            // Mocking HttpClient handler to simulate HTTP responses
            _mockHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHandler.Object);
            _bingSearchService = new BingSearchService(_httpClient);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnPosition_WhenTargetUrlFound()
        {
            // Arrange
            var keywords = "example";
            var targetUrl = "http://example.com";
            var htmlContents = new List<string>
            {
                "<ol id=\"b_results\"><li><cite>http://example.com</cite></li></ol>", // Page 1
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 2
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 3
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 4
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 5
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 6
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 7
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 8
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 9
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>"  // Page 10
            };
            _mockHandler.SetupRequestForMultiplePages(SearchConstants.BingBaseUrl, keywords, htmlContents, HttpStatusCode.OK);

            // Act
            var result = await _bingSearchService.SearchAsync(keywords, targetUrl);

            // Assert
            Assert.Equal("1", result); // Expecting position 1 for the target URL
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnZero_WhenTargetUrlNotFound()
        {
            // Arrange
            var keywords = "example";
            var targetUrl = "http://nonexistenturl.com";

            var htmlContents = new List<string>
            {
                "<ol id=\"b_results\"><li><cite>http://example.com</cite></li></ol>", // Page 1
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 2
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 3
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 4
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 5
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 6
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 7
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 8
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 9
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>"  // Page 10
            };
            _mockHandler.SetupRequestForMultiplePages(SearchConstants.BingBaseUrl, keywords, htmlContents, HttpStatusCode.OK);

            // Act
            var result = await _bingSearchService.SearchAsync(keywords, targetUrl);

            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMultiplePositions_WhenTargetUrlAppearsMultipleTimes()
        {
            // Arrange
            var keywords = "example";
            var targetUrl = "http://example.com";
            var htmlContents = new List<string>
            {
                "<ol id=\"b_results\"><li><cite>http://example.com</cite></li></ol>", // Page 1
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 2
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 3
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 4
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 5
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 6
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 7
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 8
                "<ol id=\"b_results\"><li><cite>http://anotherurl.com</cite></li></ol>", // Page 9
                "<ol id=\"b_results\"><li><cite>http://example.com</cite></li></ol>"  // Page 10
            };
            _mockHandler.SetupRequestForMultiplePages(SearchConstants.BingBaseUrl, keywords, htmlContents, HttpStatusCode.OK);

            // Act
            var result = await _bingSearchService.SearchAsync(keywords, targetUrl);

            // Assert
            Assert.Equal("1, 10", result); // Expecting positions 1 and 2 for the target URL
        }


    }

    public static class HttpMessageHandlerExtensions
    {
        public static void SetupRequestForMultiplePages(this Mock<HttpMessageHandler> mockHandler, string baseUrl, string keywords, List<string> htmlContents, HttpStatusCode statusCode)
        {
            if (htmlContents.Count != 10)
            {
                throw new ArgumentException("You must provide exactly 10 HTML contents for 10 pages.");
            }

            for (int i = 0; i < 10; i++)
            {
                var url = $"{baseUrl}/search?q={keywords}&first={i * SearchConstants.PageSize + 1}";
                var response = new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(htmlContents[i], Encoding.UTF8, "text/html")
                };

                mockHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Get &&
                            req.RequestUri.ToString().StartsWith(url)),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ReturnsAsync(response);
            }
        }
    }
}
