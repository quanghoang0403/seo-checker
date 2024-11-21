using Infrastructure.Services.SearchServices;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace Infrastructure.Tests.Services
{
    public class GoogleSearchServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly GoogleSearchService _googleSearchService;

        public GoogleSearchServiceTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHandler.Object);
            _googleSearchService = new GoogleSearchService(_httpClient);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnPositions_WhenTargetUrlFound()
        {
            // Arrange
            var keywords = "example";
            var targetUrl = "https://example.com";
            var htmlContent = "<html><div id=\"rso\"><div><a jsname=\"abc\" href=\"https://example.com\">Link</a></div></div>div id=\"rso\"><div><a jsname=\"abc\" href=\"https://another.com\">Link</a></div></div></html>";

            SetupHttpClientMock(htmlContent);

            // Act
            var result = await _googleSearchService.SearchAsync(keywords, targetUrl);

            // Assert
            Assert.Equal("1", result); // Expecting positions 1 and 3 for the target URL
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnZero_WhenTargetUrlNotFound()
        {
            // Arrange
            var keywords = "example";
            var targetUrl = "https://notfound.com";
            var htmlContent = @"
                <div id='rso'>
                    <div><a jsname='abc' href='https://example.com'>Example</a></div>
                    <div><a jsname='abc' href='https://another.com'>Another</a></div>
                </div>";

            SetupHttpClientMock(htmlContent);

            // Act
            var result = await _googleSearchService.SearchAsync(keywords, targetUrl);

            // Assert
            Assert.Equal("0", result); // Expecting 0 because the URL is not found
        }

        [Fact]
        public async Task SearchAsync_ShouldThrowException_OnHttpRequestFailure()
        {
            // Arrange
            var keywords = "example";
            var targetUrl = "https://example.com";

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _googleSearchService.SearchAsync(keywords, targetUrl));
        }

        private void SetupHttpClientMock(string htmlContent)
        {
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(htmlContent, Encoding.UTF8, "text/html")
                });
        }
    }
}
