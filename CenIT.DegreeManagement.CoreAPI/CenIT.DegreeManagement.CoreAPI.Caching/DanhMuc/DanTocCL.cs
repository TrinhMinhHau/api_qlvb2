using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc
{
    public class DanTocCL
    {
        private string _masterCacheKey = "DanTocCL";
        private CacheLayer _cache;
        private DanTocBL _BL;

        public DanTocCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new DanTocBL(configuration);
        }

        public async Task<int> Create(DanTocInputModel model)
        {
            var result = await _BL.Create(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> Modify(DanTocInputModel model)
        {
            var result = await _BL.Modify(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            var result = await _BL.Delete(id, nguoiThucHien);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public List<DanTocModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("DanToc-GetSearch-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<DanTocModel> danTocs = _cache.GetCacheKey<List<DanTocModel>>(rawKey, _masterCacheKey)!;
            if (danTocs != null) return danTocs;

            danTocs = _BL.GetSearch(out total, modelSearch);
            _cache.AddCacheItem(rawKey, danTocs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return danTocs;
        }

        public DanTocModel GetById(string idDanToc)
        {
            var hashKey = EHashMd5.FromObject(idDanToc);
            var rawKey = string.Concat("HeDaoTao-GetById-", hashKey);
            var danToc = _cache.GetCacheKey<DanTocModel>(rawKey, _masterCacheKey)!;
            if (danToc == null)
            {
                danToc = _BL.GetById(idDanToc);
                _cache.AddCacheItem(rawKey, danToc, _masterCacheKey);
            }
            return danToc;
        }

        public string[] GetAllTenDanToc()
        {
            var rawKey = string.Concat("DanToc-GetAllTenDanToc");

            //Get item from cache
            string[] danTocs = _cache.GetCacheKey<string[]>(rawKey, _masterCacheKey)!;
            if (danTocs == null)
            {
                danTocs = _BL.GetAllTenDanToc();
                _cache.AddCacheItem(rawKey, danTocs, _masterCacheKey);
            };

            return danTocs;
        }
        public List<DanTocModel> GetAll()
        {
            var rawKey = string.Concat("HeDaoTao-GetAll");

            //Get item from cache
            List<DanTocModel> danTocs = _cache.GetCacheKey<List<DanTocModel>>(rawKey, _masterCacheKey)!;
            if (danTocs == null)
            {
                danTocs = _BL.GetAll();
                _cache.AddCacheItem(rawKey, danTocs, _masterCacheKey);
            };

            return danTocs;
        }
    }
}
