using Application.Models;
using Caching;
using Domain.Enums;
using Domain.Extensions;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.Search.Queries
{
    public class SearchRequest : IRequest<BaseResponseModel>
    {
        public string Keyword { get; set; }
        public string Url { get; set; }
        public EnumBrowser BrowserType { get; set; }
    }

    public class SearchRequestHandler : IRequestHandler<SearchRequest, BaseResponseModel>
    {
        private readonly ICacheService _cacheService;
        private readonly ISearchServiceFactory _searchServiceFactory;

        public SearchRequestHandler(ICacheService cacheService, ISearchServiceFactory searchServiceFactory)
        {
            _cacheService = cacheService;
            _searchServiceFactory = searchServiceFactory;
        }

        public async Task<BaseResponseModel> Handle(SearchRequest request, CancellationToken cancellationToken)
        {
            var invalidRequest = RequestValidation(request);
            if (invalidRequest != null)
            {
                return invalidRequest;
            }
            var keyCache = string.Format(KeyCacheConstants.SearchKey, request.Keyword, request.Url, request.BrowserType);
            var res = _cacheService.GetCache<List<RankingResultModel>>(keyCache);

            if (res != null)
            {
                return BaseResponseModel.ReturnData(res);
            }

            var rankingResults = new List<RankingResultModel>();
            if (request.BrowserType == EnumBrowser.All)
            {
                foreach (EnumBrowser browser in Enum.GetValues(typeof(EnumBrowser)))
                {
                    if (browser != EnumBrowser.All)
                    {
                        var service = _searchServiceFactory.GetSearchService(browser);
                        if (service != null)
                        {
                            var result = await service.SearchAsync(request.Keyword, request.Url);
                            rankingResults.Add(new RankingResultModel()
                            {
                                BrowserName = browser.GetDescription(),
                                Position = result
                            });
                        }
                    }
                }
            }
            else
            {
                var searchService = _searchServiceFactory.GetSearchService(request.BrowserType);
                var result = await searchService.SearchAsync(request.Keyword, request.Url);
                rankingResults.Add(new RankingResultModel()
                {
                    BrowserName = request.BrowserType.GetDescription(),
                    Position = result
                });
            }
            _cacheService.SetCache(keyCache, rankingResults);
            return BaseResponseModel.ReturnData(rankingResults);
        }

        private static BaseResponseModel? RequestValidation(SearchRequest request)
        {
            if (string.IsNullOrEmpty(request.Keyword))
            {
                return BaseResponseModel.ReturnError("Keyword must not be empty.");
            }
            else if (string.IsNullOrEmpty(request.Url))
            {
                return BaseResponseModel.ReturnError("Url must not be empty.");
            }
            else
            {
                return null;
            }

        }
    }
}
