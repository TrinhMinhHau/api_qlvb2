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
    public class SysMessageConfigCL
    {
        private string _masterCacheKey = "MessageConfigCL";
        private CacheLayer _cache;
        private SysMessageConfigBL _sysMessageConfigBL;

        public SysMessageConfigCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysMessageConfigBL = new SysMessageConfigBL(connectDBString ?? "");
        }

        public MessageConfigModel GetByActionName(string actionName)
        {
            var hashKey = EHashMd5.FromObject(actionName);
            var rawKey = string.Concat("GetByActionName-", hashKey);

            //Get item from cache
            MessageConfigModel item = _cache.GetCacheKey<MessageConfigModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysMessageConfigBL.GetByActionName(actionName);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }

        public MessageConfigModel GetById(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetById-", hashKey);

            //Get item from cache
            MessageConfigModel item = _cache.GetCacheKey<MessageConfigModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysMessageConfigBL.GetById(id);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }

        public List<MessageConfigModel> GetAll(SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAll-", hashKey);


            //Get item from cache
            List<MessageConfigModel> configs = _cache.GetCacheKey<List<MessageConfigModel>>(rawKey, _masterCacheKey)!;
            if (configs == null)
            {
                configs = _sysMessageConfigBL.GetAll(model);
                _cache.AddCacheItem(rawKey, configs, _masterCacheKey);
            };

            return configs;
        }


        public int Save(MessageConfigInputModel model)
        {
            var result = _sysMessageConfigBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        public int Delete(int id)
        {
            var result = _sysMessageConfigBL.Delete(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

    }
}
