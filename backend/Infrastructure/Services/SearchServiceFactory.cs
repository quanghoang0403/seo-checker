using Domain.Enums;
using Infrastructure.Interfaces;
using Infrastructure.Services.SearchServices;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services
{
    public class SearchServiceFactory : ISearchServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISearchService GetSearchService(EnumBrowser browserType)
        {
            return browserType switch
            {
                EnumBrowser.Google => _serviceProvider.GetRequiredService<GoogleSearchService>(),
                EnumBrowser.Bing => _serviceProvider.GetRequiredService<BingSearchService>(),
                _ => throw new ArgumentException("Invalid search engine type")
            };
        }
    }
}
