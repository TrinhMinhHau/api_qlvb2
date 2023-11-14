using CenIT.DegreeManagement.CoreAPI.Bussiness;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching
{
    public class AppCL
    {
        private string _masterCacheKey = "AppCache";
        private CacheLayer _cache;

        public AppCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
        }

        /// <summary>
        /// Lưu thông tin chuỗi connect string
        /// </summary>
        /// <param name="connectString"></param>
        public void saveConfiguration(string connectString)
        {
            _cache.AddCacheItemByKey<string>(_cache.CONFIGURATION_KEY, connectString);
        }


    }
}
