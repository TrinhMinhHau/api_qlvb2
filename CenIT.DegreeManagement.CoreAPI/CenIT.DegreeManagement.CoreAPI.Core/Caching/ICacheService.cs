using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Caching
{
    public interface ICacheService
    {
        public T? GetCacheKey<T>(string cacheKey, string masterKey = "");
        public void AddCacheItem<T>(string rawKey, T value, string masterKey = "");
        public void InvalidateCache(string masterKey = "");
        public void InvalidateCache(string rawKey, string masterKey = "");
        public void InvalidateAllCache();

    }
}
