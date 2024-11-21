namespace Infrastructure.Interfaces
{
    public interface ISearchService
    {
        Task<string> SearchAsync(string keyword, string url);
    }
}
