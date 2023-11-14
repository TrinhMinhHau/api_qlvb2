using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Caching
{
    public class CacheLayer: ICacheService
    {
        private const double CacheDurationSlidingExpiration = 30;
        private const double CacheDurationAbsoluteExpiration = 30;
        private IMemoryCache _cache;
        private List<string> _masterCacheKeyArray;
        private string _masterKey = "";

        public string CONFIGURATION_KEY = "CONFIGURATION_KEY";

        public CacheLayer(IMemoryCache memoryCache, string masterKey = "AppCache")
        {
            _cache = memoryCache;
            _masterKey = masterKey;
        }

        public CacheLayer(IMemoryCache memoryCache, IConfiguration configuration, string masterKey = "AppCache")
        {
            _cache = memoryCache;
            _masterKey = masterKey;
        }

        public void setMasterKey(string rawKey)
        {
            _masterKey = rawKey;
            // init list key cache when object is null
            if (_masterCacheKeyArray == null)
            {
                _masterCacheKeyArray = new List<string>();
                _masterCacheKeyArray.Add(rawKey);
            }
        }

        /// <summary>
        /// Add item to cache
        /// </summary>
        /// <param name="rawKey">Key of cache</param>
        /// <param name="value">Data contain</param>
        public void AddCacheItem<T>(string rawKey, T value, string masterKey = "")
        {
            
            string key = string.Concat(masterKey, (string)rawKey);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(CacheDurationSlidingExpiration))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheDurationAbsoluteExpiration))
                    .SetPriority(CacheItemPriority.NeverRemove)
                    .SetSize(1024);
            _cache.Set(key, value, cacheEntryOptions);
        }

        public void AddCacheItemByKey<T>(string rawKey, T value)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CacheDurationSlidingExpiration))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(CacheDurationAbsoluteExpiration))
                    .SetPriority(CacheItemPriority.NeverRemove)
                    .SetSize(1024);
            _cache.Set(rawKey, value, cacheEntryOptions);
        }

        /// <summary>
        /// Get item from cache
        /// </summary>
        /// <param name="rawKey">Key of cache</param>
        /// <returns></returns>
        public T? GetCacheKey<T>(string rawKey, string masterKey = "")
        {
            if(_cache  == null)
            {
                return default(T);
            }

            if(!rawKey.Contains(CONFIGURATION_KEY))
            {
                rawKey = string.Concat(masterKey, (string)rawKey);
            }    

            return _cache.Get<T>(rawKey);
        }

        /// <summary>
        /// Remove cache by key
        /// </summary>
        /// <param name="rawKey">key of cache</param>
        /// <exception cref="NotImplementedException"></exception>
        public void InvalidateAllCache()
        {
            var keys = CacheHelper.GetCacheKeys(_cache);
            foreach (var key in keys)
            {
                 _cache.Remove(key);
            }
        }

        /// <summary>
        /// Remove cache key contains masterKey
        /// </summary>
        /// <param name="rawKey">key of cache</param>
        /// <exception cref="NotImplementedException"></exception>
        public void InvalidateCache(string masterKey = "")
        {
            var keys = CacheHelper.GetCacheKeys(_cache);
            foreach (var key in keys)
            {
                if (key.Contains(masterKey))
                {
                    _cache.Remove(key);
                }
            }
 
        }

    /// <summary>
    /// Remove cache by key
    /// </summary>
    /// <param name="rawKey">key of cache</param>
    /// <exception cref="NotImplementedException"></exception>
    public void InvalidateCache(string rawKey, string masterKey = "")
        {
            string key = string.Concat(masterKey, (string)rawKey);
            _cache.Remove(key);
        }
    }
}
