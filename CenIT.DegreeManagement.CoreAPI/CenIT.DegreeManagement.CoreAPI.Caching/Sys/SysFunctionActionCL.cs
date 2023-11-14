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
    public class SysFunctionActionCL
    {
        private string _masterCacheKey = "SysFunctionActionCL";
        private CacheLayer _cache;
        private SysFunctionActionBL _sysFunctionActionBL;

        public SysFunctionActionCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysFunctionActionBL = new SysFunctionActionBL(connectDBString ?? "");

        }

        /// <summary>
        /// Lấy danh sách functionAction
        /// </summary>
        /// <param name="SearchParamModel"></param>
        /// <returns></returns>
        public List<FunctionActionModel> GetAll(SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllFunctionAction-", hashKey);

            //Get item from cache
            List<FunctionActionModel> function_action = _cache.GetCacheKey<List<FunctionActionModel>>(rawKey, _masterCacheKey)!;
            if (function_action == null)
            {
                function_action = _sysFunctionActionBL.GetAll(model);
                _cache.AddCacheItem(rawKey, function_action, _masterCacheKey);

            }
            return function_action;
        }

        /// <summary>
        /// Lấy danh sách functionAction theo functionId
        /// </summary>
        /// <param name="SearchParamModel"></param>
        /// <returns></returns>
        public List<FunctionActionModel> GetActionsByFunctionId(int id, SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetByIDFunction-", id, hashKey);

            //Get item from cache
            List<FunctionActionModel> item = _cache.GetCacheKey<List<FunctionActionModel>>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysFunctionActionBL.GetActionsByFunctionId(id, model);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }
            return item;
        }

        /// <summary>
        /// Lấy functionAction theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FunctionActionModel GetByID(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetByIDFunctionAction-", hashKey);

            //Get item from cache
            FunctionActionModel item = _cache.GetCacheKey<FunctionActionModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysFunctionActionBL.GetById(id);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }

        /// <summary>
        /// Lưu functionAction 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(FunctionActionInputModel model)
        {
            var result = _sysFunctionActionBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Xóa functionAction 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            var result = _sysFunctionActionBL.Delete(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }
    }

}
