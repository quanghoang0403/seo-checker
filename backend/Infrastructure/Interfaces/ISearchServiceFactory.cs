using Domain.Enums;

namespace Infrastructure.Interfaces
{
    public interface ISearchServiceFactory
    {
        ISearchService GetSearchService(EnumBrowser browserType);
    }
}
