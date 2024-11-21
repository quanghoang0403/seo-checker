using Application.Features.Search.Queries;
using Application.Models;
using Caching;
using Domain.Constants;
using Domain.Enums;
using Domain.Extensions;
using Moq;

namespace Application.Tests.Features.Search.Queries
{
    public class GetSupportBrowsersRequestHandlerTests
    {
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly GetSupportBrowsersRequestHandler _handler;

        public GetSupportBrowsersRequestHandlerTests()
        {
            _mockCacheService = new Mock<ICacheService>();
            _handler = new GetSupportBrowsersRequestHandler(_mockCacheService.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedData_WhenCacheExists()
        {
            // Arrange
            var cacheKey = string.Format(KeyCacheConstants.SupportBrowsers);
            var cachedData = new List<SupportBrowserModel>
        {
            new() { BrowserType = EnumBrowser.All, BrowserName = "All" },
            new() { BrowserType = EnumBrowser.Google, BrowserName = "Google" },
            new() { BrowserType = EnumBrowser.Bing, BrowserName = "Bing" }
        };

            _mockCacheService.Setup(cs => cs.GetCache<IEnumerable<SupportBrowserModel>>(cacheKey))
                             .Returns(cachedData);

            var request = new GetSupportBrowsersRequest();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cachedData, result.Data);
            _mockCacheService.Verify(cs => cs.GetCache<IEnumerable<SupportBrowserModel>>(cacheKey), Times.Once);
            _mockCacheService.Verify(cs => cs.SetCache(It.IsAny<string>(), It.IsAny<IEnumerable<SupportBrowserModel>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFreshData_WhenCacheDoesNotExist()
        {
            // Arrange
            var cacheKey = string.Format(KeyCacheConstants.SupportBrowsers);
            _mockCacheService.Setup(cs => cs.GetCache<IEnumerable<SupportBrowserModel>>(cacheKey))
                             .Returns((IEnumerable<SupportBrowserModel>)null);  // Simulate cache miss

            var supportBrowsers = SearchConstants.SUPPORT_BROWSERS.Select(browser => new SupportBrowserModel
            {
                BrowserType = browser,
                BrowserName = browser.GetDescription()
            }).ToList();

            _mockCacheService.Setup(cs => cs.SetCache(cacheKey, supportBrowsers));

            var request = new GetSupportBrowsersRequest();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);
            var resultData = result?.Data as IEnumerable<SupportBrowserModel>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(supportBrowsers.Count, resultData?.Count());
            _mockCacheService.Verify(cs => cs.GetCache<IEnumerable<SupportBrowserModel>>(cacheKey), Times.Once);
            _mockCacheService.Verify(cs => cs.SetCache(cacheKey, It.Is<IEnumerable<SupportBrowserModel>>(list => list.Count() == supportBrowsers.Count)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenCacheServiceFails()
        {
            // Arrange
            var cacheKey = string.Format(KeyCacheConstants.SupportBrowsers);
            _mockCacheService.Setup(cs => cs.GetCache<IEnumerable<SupportBrowserModel>>(cacheKey))
                             .Throws(new System.Exception("Cache retrieval failed"));

            var request = new GetSupportBrowsersRequest();

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(async () =>
                await _handler.Handle(request, CancellationToken.None));
        }
    }

}
