using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using Microsoft.Extensions.Configuration;


namespace CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc
{
    public class MonThiCL
    {
        private string _masterCacheKey = "MonThiCache";
        private CacheLayer _cache;
        private MonThiBL _BL;
        public MonThiCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new MonThiBL(configuration);
        }

        /// <summary>
        /// Thêm môn thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(MonThiInputModel model)
        {
            var result = await _BL.Create(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật môn thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(MonThiInputModel model)
        {
            var result = await _BL.Modify(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Xóa môn thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Delete(string Id, string UserAction)
        {
            var result = await _BL.Delete(Id, UserAction);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách môn thi
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<MonThiModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("GetSearch-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            List<MonThiModel> monThis = _cache.GetCacheKey<List<MonThiModel>>(rawKey, _masterCacheKey)!;
            if (monThis != null) return monThis;
            // Item not found in cache - retrieve it and insert it into the cache
            monThis = _BL.GetSearch(out total, modelSearch);
            _cache.AddCacheItem(rawKey, monThis, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return monThis;
        }

        /// <summary>
        /// Lấy môn thi theo Id
        /// </summary>
        /// <param name="hdtId"></param>
        /// <returns></returns>
        public MonThiModel GetById(string hdtId)
        {
            var hashKey = EHashMd5.FromObject(hdtId);
            var rawKey = string.Concat("MonThi-GetById-", hashKey);
            var monThi = _cache.GetCacheKey<MonThiModel>(rawKey, _masterCacheKey)!;
            if (monThi == null)
            {
                monThi = _BL.GetById(hdtId);
                _cache.AddCacheItem(rawKey, monThi, _masterCacheKey);
            }
            return monThi;
        }

        /// <summary>
        /// Lấy tất cả môn thi
        /// </summary>
        /// <returns></returns>
        public List<MonThiModel> GetAll()
        {
            var rawKey = string.Concat("HeDaoTao-GetAll");

            //Get item from cache
            List<MonThiModel> monThis = _cache.GetCacheKey<List<MonThiModel>>(rawKey, _masterCacheKey)!;
            if (monThis == null)
            {
                monThis = _BL.GetAll();
                _cache.AddCacheItem(rawKey, monThis, _masterCacheKey);
            };

            return monThis;
        }

        /// <summary>
        /// Lấy tất cả môn thi
        /// </summary>
        /// <returns></returns>
        public string[] GetAllMaMonThi()
        {
            var rawKey = string.Concat("MonThi-GetAllMaMonThi");

            //Get item from cache
            string[] monThis = _cache.GetCacheKey<string[]>(rawKey, _masterCacheKey)!;
            if (monThis == null)
            {
                monThis = _BL.GetAllMaMonThi();
                _cache.AddCacheItem(rawKey, monThis, _masterCacheKey);
            };

            return monThis;
        }
    }
}
