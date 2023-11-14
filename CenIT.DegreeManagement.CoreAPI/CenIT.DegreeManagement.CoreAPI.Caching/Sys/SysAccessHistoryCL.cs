using Amazon.Runtime.Internal.Util;
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
    public class SysAccessHistoryCL
    {
        private string _masterCacheKey = "SysAccessHistoryCL";
        private CacheLayer _cache;
        private SysAccessHistoryBL _sysAccessHistoryBL;

        public SysAccessHistoryCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysAccessHistoryBL = new SysAccessHistoryBL(connectDBString ?? "");
        }

        /// <summary>
        /// Lấy danh sách thông tin lịch sử truy cập
        /// </summary>
        /// <param name="SearchParamFilterDateModel"></param>
        /// <returns></returns>
        public List<AccessHistoryModel> GetAllAccessHistory(SearchParamFilterDateModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllAccessHistory-", hashKey);
            //Get item from cache
            List<AccessHistoryModel> accessHistories = _cache.GetCacheKey<List<AccessHistoryModel>>(rawKey, _masterCacheKey)!;
            if (accessHistories == null)
            {
                accessHistories = _sysAccessHistoryBL.GetAllAccessHistory(model);
                _cache.AddCacheItem(rawKey, accessHistories);
            }
            return accessHistories;
        }

        public int Save(AccessHistoryInputModel model)
        {
            var result = _sysAccessHistoryBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách thông tin lịch sử truy cập theo từng username hoặc khoảng thời gian
        /// </summary>
        /// <param name="AccessHistoryInputModel"></param>
        /// <returns></returns>

        public List<AccessHistoryModel> GetAllAccessHistoryByUsernameOrDate(AccessHistorySearchModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllAccessHistoryExport-", hashKey);
            //Get item from cache
            List<AccessHistoryModel> accessHistories = _cache.GetCacheKey<List<AccessHistoryModel>>(rawKey, _masterCacheKey)!;
            if (accessHistories == null)
            {
                accessHistories = _sysAccessHistoryBL.GetAllAccessHistoryByUsernameOrDate(model);
                _cache.AddCacheItem(rawKey, accessHistories);
            }
            return accessHistories;
        }

        /// <summary>
        /// Lấy danh sách người dùng đã từng truy cập
        /// </summary>
        /// <returns></returns>
        public List<UserAccessHistoryModel> GetAllUserInAccessHistory()
        {
            var rawKey = string.Concat("GettAllUserInAccessHistory-");
            //Get item from cache
            List<UserAccessHistoryModel> accessHistories = _cache.GetCacheKey<List<UserAccessHistoryModel>>(rawKey, _masterCacheKey)!;
            if (accessHistories == null)
            {
                accessHistories = _sysAccessHistoryBL.GetAllUserInAccessHistory();
                _cache.AddCacheItem(rawKey, accessHistories);
            }
            return accessHistories;
        }
    }
}
