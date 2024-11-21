using Application.Features.Search.Queries;
using Application.Models;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace WebApi.Tests.Controllers
{
    public class SearchControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly SearchController _controller;

        public SearchControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new SearchController(_mockMediator.Object);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnOkResult_WhenRequestIsValid()
        {
            // Arrange
            var request = new SearchRequest
            {
                Keyword = "example",
                Url = "http://example.com",
                BrowserType = Domain.Enums.EnumBrowser.Google
            };

            var expectedResponse = new BaseResponseModel { Data = "some data" };

            _mockMediator.Setup(m => m.Send(request, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.SearchAsync(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<BaseResponseModel>(okResult.Value);
            Assert.Equal(expectedResponse, returnValue);
        }

        [Fact]
        public async Task GetSupportBrowsersAsync_ShouldReturnOkResult_WhenRequestIsValid()
        {
            // Arrange
            var request = new GetSupportBrowsersRequest();
            var expectedResponse = new BaseResponseModel
            {
                Data = new List<SupportBrowserModel>
            {
                new() { BrowserType = Domain.Enums.EnumBrowser.Google, BrowserName = "Google" },
                new() { BrowserType = Domain.Enums.EnumBrowser.Bing, BrowserName = "Bing" }
            }
            };

            _mockMediator.Setup(m => m.Send(request, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetSupportBrowsersAsync(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<BaseResponseModel>(okResult.Value);
            Assert.Equal(expectedResponse, returnValue);
        }
    }
}
