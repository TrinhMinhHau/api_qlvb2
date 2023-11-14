using CenIT.DegreeManagement.CoreAPI.Bussiness.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.TinTuc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.TinTuc
{
    public class LoaiTinTucCL
    {
        private string _masterCacheKey = "LoaiTinTucCache";
        private CacheLayer _cache;
        private LoaiTinTucBL _BL;
        public LoaiTinTucCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new LoaiTinTucBL(configuration);
        }

        public async Task<int> Create(LoaiTinTucInputModel model)
        {
            var result = await _BL.Create(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> Modify(LoaiTinTucInputModel model)
        {
            var result = await _BL.Modify(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> Delete(string idLoaiTinTuc, string nguoiThucHien)
        {
            var result = await _BL.Delete(idLoaiTinTuc, nguoiThucHien);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }


        public List<LoaiTinTucModel> GetSearchLoaiTinTuc(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("LoaiTinTucs-GetSearchLoaiTinTuc-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<LoaiTinTucModel> loaiTinTucs = _cache.GetCacheKey<List<LoaiTinTucModel>>(rawKey, _masterCacheKey)!;
            if (loaiTinTucs != null) return loaiTinTucs;
            // Item not found in cache - retrieve it and insert it into the cache
            loaiTinTucs = _BL.GetSearchLoaiTinTuc(out total, modelSearch);
            _cache.AddCacheItem(rawKey, loaiTinTucs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return loaiTinTucs;
        }

        public List<LoaiTinTucModel> GetAll()
        {
            string rawKey = string.Concat("LoaiTinTucs-GetAll");

            // See if the item is in the cache
            List<LoaiTinTucModel> loaiTinTucs = _cache.GetCacheKey<List<LoaiTinTucModel>>(rawKey, _masterCacheKey)!;
            if (loaiTinTucs != null) return loaiTinTucs;
            // Item not found in cache - retrieve it and insert it into the cache
            loaiTinTucs = _BL.GetAll();
            _cache.AddCacheItem(rawKey, loaiTinTucs, _masterCacheKey);
            return loaiTinTucs;
        }

        public LoaiTinTucModel GetById(string idLoaiTinTuc)
        {
            var hashKey = EHashMd5.FromObject(idLoaiTinTuc);
            var rawKey = string.Concat("LoaiTinTuc-idLoaiTinTuc-", hashKey);
            var loaiTinTuc = _cache.GetCacheKey<LoaiTinTucModel>(rawKey, _masterCacheKey)!;
            if (loaiTinTuc == null)
            {
                loaiTinTuc = _BL.GetById(idLoaiTinTuc);
                _cache.AddCacheItem(rawKey, loaiTinTuc, _masterCacheKey);
            }
            return loaiTinTuc;
        }
    }
}
