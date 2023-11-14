using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;


namespace CenIT.DegreeManagement.CoreAPI.Caching.Sys
{
    public class SysFunctionCL
    {
        private string _masterCacheKey = "SysFunctionCL";
        private CacheLayer _cache;
        private SysFunctionBL _sysFunctionBL;

        public SysFunctionCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysFunctionBL = new SysFunctionBL(connectDBString ?? "");
        }



        /// <summary>
        /// Lấy danh sách Function
        /// </summary>
        /// <param name="SearchParamModel"></param>
        /// <returns></returns>
        public List<FunctionModel> GetAll(SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllFunction-", hashKey);

            //Get item from cache
            List<FunctionModel> item = _cache.GetCacheKey<List<FunctionModel>>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysFunctionBL.GetAll(model);
                _cache.AddCacheItem(rawKey, item);
            }

            return item;
        }

        /// <summary>
        /// Lưu Function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FunctionModel GetByID(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("Get-", hashKey);

            //Get item from cache
            FunctionModel item = _cache.GetCacheKey<FunctionModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysFunctionBL.GetById(id);
                _cache.AddCacheItem(rawKey, item);
            }

            return item;
        }


        /// <summary>
        /// Lấy function theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Save(FunctionInputModel model)
        {
            var result = _sysFunctionBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Xóa function
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            var result = _sysFunctionBL.Delete(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

    }
}
