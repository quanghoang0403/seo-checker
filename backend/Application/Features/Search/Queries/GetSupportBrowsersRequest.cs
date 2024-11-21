using Application.Models;
using Caching;
using Domain.Constants;
using Domain.Extensions;
using Domain.Models;
using MediatR;

namespace Application.Features.Search.Queries
{
    public class GetSupportBrowsersRequest : IRequest<BaseResponseModel> { }

    public class GetSupportBrowsersResponse
    {
        public List<SupportBrowserModel> SupportBrowsers { get; set; }
    }

    public class GetSupportBrowsersRequestHandler : IRequestHandler<GetSupportBrowsersRequest, BaseResponseModel>
    {
        private readonly ICacheService _cacheService;

        public GetSupportBrowsersRequestHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }


        public async Task<BaseResponseModel> Handle(GetSupportBrowsersRequest request, CancellationToken cancellationToken)
        {
            var keyCache = string.Format(KeyCacheConstants.SupportBrowsers);
            var res = _cacheService.GetCache<IEnumerable<SupportBrowserModel>>(keyCache);

            if (res != null)
            {
                return BaseResponseModel.ReturnData(res);
            }
            var supportBrowsers = SearchConstants.SUPPORT_BROWSERS.Select(browser => new SupportBrowserModel { BrowserType = browser, BrowserName = browser.GetDescription() });
            _cacheService.SetCache(keyCache, supportBrowsers);
            return BaseResponseModel.ReturnData(supportBrowsers);
        }

    }
}
