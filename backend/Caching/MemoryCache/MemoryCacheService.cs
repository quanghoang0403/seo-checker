using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Caching.MemoryCache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MemoryCacheService(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public T? GetCache<T>(string key)
        {
            var keyCache = CreateKeyCache(key);
            var isRemoveCache = CheckClearCache(key);
            if (isRemoveCache)
            {
                RemoveCache(key);
                return default;
            }
            var value = _memoryCache.Get<T>(keyCache);
            return value ?? default;
        }

        public void SetCache<T>(string key, T data)
        {
            SetCache(key, data, TimeCacheConstants.DateHour);
        }

        public void SetCache<T>(string key, T data, TimeSpan timeExpired)
        {
            var keyCache = CreateKeyCache(key);
            var cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = timeExpired };
            _memoryCache.Set(keyCache, data, cacheEntryOptions);
        }

        public void RemoveCache(string key)
        {
            var keyCache = CreateKeyCache(key);
            _memoryCache.Remove(keyCache);
        }

        protected bool CheckClearCache(string key)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var httpRequest = _httpContextAccessor.HttpContext.Request;
                var typeCacheClear = httpRequest.Query["clearcache"].ToString();

                if (string.IsNullOrEmpty(typeCacheClear) == false)
                {
                    if (key.Contains(typeCacheClear)) { return true; }
                    if (typeCacheClear == "1") { return true; }
                }


            }
            return false;
        }

        protected string CreateKeyCache(string key)
        {
            return $"{KeyCacheConstants.Prefix}_{key}";
        }
    }
}
