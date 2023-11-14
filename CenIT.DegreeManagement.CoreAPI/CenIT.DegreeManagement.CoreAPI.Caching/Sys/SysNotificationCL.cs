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
    public class SysNotificationCL
    {
        private string _masterCacheKey = "SysNotificationCL";
        private CacheLayer _cache;
        private SysNotificationBL _Bl;

        public SysNotificationCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _Bl = new SysNotificationBL(connectDBString ?? "");
        }

        /// <summary>
        /// Lấy danh sách tin nhắn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<NotificationModel> GetAllNotification(SearchParamFilterDateModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllConfig-", hashKey);


            //Get item from cache
            List<NotificationModel> configs = _cache.GetCacheKey<List<NotificationModel>>(rawKey, _masterCacheKey)!;
            if (configs == null)
            {
                configs = _Bl.GetAllNotification(model);
                _cache.AddCacheItem(rawKey, configs, _masterCacheKey);
            };

            return configs;
        }

        public int Save(NotificationInputModel model)
        {
            var result = _Bl.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        public NotificationModel GetNotificationById(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetNotificationById-", hashKey);

            //Get item from cache
            NotificationModel item = _cache.GetCacheKey<NotificationModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _Bl.GetNotificationById(id);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }
    }
}
