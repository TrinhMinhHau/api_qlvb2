using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using MongoDB.Driver.Core.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Sys
{
    public class SysConfigCL
    {
        private string _masterCacheKey = "SysConfigCL";
        private CacheLayer _cache;
        private SysConfigBL _sysConfigBL;

        public SysConfigCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysConfigBL = new SysConfigBL(connectDBString ?? "");
        }


        /// <summary>
        /// Lấy danh sách cấu hình
        /// </summary>
        /// <param name="SearchParamModel"></param>
        /// <returns></returns>
        public List<ConfigModel> GetAllConfig(SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllConfig-", hashKey);


            //Get item from cache
            List<ConfigModel> configs = _cache.GetCacheKey<List<ConfigModel>>(rawKey, _masterCacheKey)!;
            if (configs == null)
            {
                configs = _sysConfigBL.GetAllConfig(model);
                _cache.AddCacheItem(rawKey, configs, _masterCacheKey);
            };

            return configs;
        }

        /// <summary>
        /// Lưu cấu hình
        /// </summary>
        /// <param name="ConfigInputModel"></param>
        /// <returns>
        ///  result > 0 : Lưu thành công
        ///  result == -9: Cấu hình tồn tại
        ///  result == -10: Cấu hình chưa tồn tại
        ///  result == -1: Lưu thất bại
        /// </returns>
        public int Save(ConfigInputModel model)
        {
            var result = _sysConfigBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Xóa cấu hình
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///  result > 0 : Xóa thành công
        ///  result < 0: Cấu hình không tồn tại
        /// </returns>
        public int Delete(int id)
        {
            var result = _sysConfigBL.Delete(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Lấy cấu hình theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>/// </returns>
        public ConfigModel GetConfigById(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetConfigById-", hashKey);

            //Get item from cache
            ConfigModel item = _cache.GetCacheKey<ConfigModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysConfigBL.GetConfigById(id);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }

        /// <summary>
        /// Lấy cấu hình theo key
        /// </summary>
        /// <param name="configkey"></param>
        /// <returns>/// </returns>
        public ConfigModel GetConfigByKey(string? configKey)
        {
            var hashKey = EHashMd5.FromObject(configKey);
            var rawKey = string.Concat("GetConfigByKey-", hashKey);

            //Get item from cache
            ConfigModel item = _cache.GetCacheKey<ConfigModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysConfigBL.GetConfigByKey(configKey);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }
    }
}
