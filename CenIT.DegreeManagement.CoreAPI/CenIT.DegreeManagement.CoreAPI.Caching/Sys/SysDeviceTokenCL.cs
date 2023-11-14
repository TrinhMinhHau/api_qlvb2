using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Sys
{
    public class SysDeviceTokenCL
    {
        private string _masterCacheKey = "SysDeviceToken";
        private CacheLayer _cache;
        private SysDeviceTokenBL _BL;

        public SysDeviceTokenCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _BL = new SysDeviceTokenBL(connectDBString ?? "");
        }

        public List<DeviceTokenModel> GetByIdDonVi(string idDonVi)
        {
            var hashKey = EHashMd5.FromObject(idDonVi);
            var rawKey = string.Concat("GetByIdDonVi-", hashKey);

            //Get item from cache
            List<DeviceTokenModel> configs = _cache.GetCacheKey<List<DeviceTokenModel>>(rawKey, _masterCacheKey)!;
            if (configs == null)
            {
                configs = _BL.GetByIdDonVi(idDonVi);
                _cache.AddCacheItem(rawKey, configs, _masterCacheKey);
            };

            return configs;
        }

        public int Save(DeviceTokenInputModel model)
        {
            var result = _BL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }
    }
}
