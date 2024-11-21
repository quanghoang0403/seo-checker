using Application.Features.Search.Queries;
using Application.Models;
using Caching;
using Domain.Enums;
using Infrastructure.Interfaces;
using Moq;

namespace Application.Tests.Features.Search.Queries
{

    public class SearchRequestHandlerTests
    {
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<ISearchService> _mockGoogleSearchService;
        private readonly Mock<ISearchService> _mockBingSearchService;
        private readonly Mock<ISearchServiceFactory> _mockSearchServiceFactory;
        private readonly SearchRequestHandler _handler;

        public SearchRequestHandlerTests()
        {
            _mockCacheService = new Mock<ICacheService>();
            _mockSearchServiceFactory = new Mock<ISearchServiceFactory>();
            _mockGoogleSearchService = new Mock<ISearchService>();
            _mockBingSearchService = new Mock<ISearchService>();
            _handler = new SearchRequestHandler(_mockCacheService.Object, _mockSearchServiceFactory.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnData_WhenCacheExists()
        {
            // Arrange
            var keyword = "test";
            var url = "https://example.com";
            var browserType = EnumBrowser.Google;

            var request = new SearchRequest
            {
                Keyword = keyword,
                Url = url,
                BrowserType = browserType
            };

            var cacheKey = string.Format(KeyCacheConstants.SearchKey, keyword, url, browserType);
            var cachedResult = new List<RankingResultModel>
        {
            new() { BrowserName = "Google", Position = "1" }
        };

            _mockCacheService.Setup(cs => cs.GetCache<List<RankingResultModel>>(cacheKey)).Returns(cachedResult);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cachedResult, result.Data);
            _mockCacheService.Verify(cs => cs.GetCache<List<RankingResultModel>>(cacheKey), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnData_WhenCacheDoesNotExist()
        {
            // Arrange
            var keyword = "test";
            var url = "https://example.com";
            var browserType = EnumBrowser.All;

            var request = new SearchRequest
            {
                Keyword = keyword,
                Url = url,
                BrowserType = browserType
            };

            var cacheKey = string.Format(KeyCacheConstants.SearchKey, keyword, url, browserType);
            _mockCacheService.Setup(cs => cs.GetCache<List<RankingResultModel>>(cacheKey)).Returns((List<RankingResultModel>)null);


            _mockGoogleSearchService.Setup(s => s.SearchAsync(request.Keyword, request.Url))
                .ReturnsAsync("1");
            _mockBingSearchService.Setup(s => s.SearchAsync(request.Keyword, request.Url))
                .ReturnsAsync("2");

            var mockSearchService = new Mock<ISearchService>();

            mockSearchService.Setup(s => s.SearchAsync(keyword, url)).ReturnsAsync("1");
            _mockSearchServiceFactory.Setup(f => f.GetSearchService(EnumBrowser.Google)).Returns(_mockGoogleSearchService.Object);
            _mockSearchServiceFactory.Setup(f => f.GetSearchService(EnumBrowser.Bing)).Returns(_mockBingSearchService.Object);
            _mockCacheService.Setup(cs => cs.SetCache(cacheKey, It.IsAny<List<RankingResultModel>>()));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Data?.Count == 2);
            Assert.Equal(result?.Data?[0].Position, "1");
            Assert.Equal(result?.Data?[1].Position, "2");
            _mockCacheService.Verify(cs => cs.SetCache(cacheKey, It.IsAny<List<RankingResultModel>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenKeywordIsEmpty()
        {
            // Arrange
            var request = new SearchRequest
            {
                Keyword = "",
                Url = "https://example.com",
                BrowserType = EnumBrowser.Google
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Keyword must not be empty.", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenUrlIsEmpty()
        {
            // Arrange
            var request = new SearchRequest
            {
                Keyword = "test",
                Url = "",
                BrowserType = EnumBrowser.Google
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Url must not be empty.", result.Message);
        }
    }

}

