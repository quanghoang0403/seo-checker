namespace Caching
{
    public interface ICacheService
    {
        T? GetCache<T>(string key);

        void SetCache<T>(string key, T data);

        void SetCache<T>(string key, T data, TimeSpan timeExpried);

        void RemoveCache(string key);
    }
}
