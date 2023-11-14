using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Bussiness.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.TinTuc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.TinTuc
{
    public class TinTucCL
    {
        private string _masterCacheKey = "TinTucCache";
        private CacheLayer _cache;
        private TinTucBL _BL;
        public TinTucCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new TinTucBL(configuration);
        }
        public async Task<int> Create(TinTucInputModel model)
        {
            var result = await _BL.Create(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> Modify(TinTucInputModel model)
        {
            var result = await _BL.Modify(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> Delete(string idTinTuc, string nguoiThucHien)
        {
            var result = await _BL.Delete(idTinTuc, nguoiThucHien);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> HideTinTuc(string idTinTuc, bool isHide = true)
        {
            var result = await _BL.HideTinTuc(idTinTuc, isHide);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }


        public List<TinTucViewModel> GetSearchTinTuc(out int total, SearchParamModel modelSearch, bool isPublish = false)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("TinTuc-GetSearchTinTuc-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<TinTucViewModel> tinTucs = _cache.GetCacheKey<List<TinTucViewModel>>(rawKey, _masterCacheKey)!;
            if (tinTucs != null) return tinTucs;
            // Item not found in cache - retrieve it and insert it into the cache
            tinTucs = _BL.GetSearchTinTuc(out total, modelSearch, isPublish);
            _cache.AddCacheItem(rawKey, tinTucs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return tinTucs;
        }

        public List<TinTucViewModel> GetSearchTinTucByIdLoaiTin(out int total, string idLoaiTin,SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + EHashMd5.FromObject(idLoaiTin);
            string rawKey = string.Concat("TinTuc-GetSearchTinTucByIdLoaiTin-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<TinTucViewModel> tinTucs = _cache.GetCacheKey<List<TinTucViewModel>>(rawKey, _masterCacheKey)!;
            if (tinTucs != null) return tinTucs;
            // Item not found in cache - retrieve it and insert it into the cache
            tinTucs = _BL.GetSearchTinTucByIdLoaiTin(out total, idLoaiTin,modelSearch);
            _cache.AddCacheItem(rawKey, tinTucs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return tinTucs;
        }

        public List<TinTucViewModel> GetAll()
        {
            string rawKey = string.Concat("TinTucs-GetAll");

            // See if the item is in the cache
            List<TinTucViewModel> tinTucs = _cache.GetCacheKey<List<TinTucViewModel>>(rawKey, _masterCacheKey)!;
            if (tinTucs != null) return tinTucs;
            // Item not found in cache - retrieve it and insert it into the cache
            tinTucs = _BL.GetAll();
            _cache.AddCacheItem(rawKey, tinTucs, _masterCacheKey);
            return tinTucs;
        }


        public List<TinTucViewModel> GetLatest4News()
        {
            string rawKey = string.Concat("TinTucs-GetLatest4News");

            // See if the item is in the cache
            List<TinTucViewModel> tinTucs = _cache.GetCacheKey<List<TinTucViewModel>>(rawKey, _masterCacheKey)!;
            if (tinTucs != null) return tinTucs;
            // Item not found in cache - retrieve it and insert it into the cache
            tinTucs = _BL.GetLatest4News();
            _cache.AddCacheItem(rawKey, tinTucs, _masterCacheKey);
            return tinTucs;
        }

        public TinTucViewModel GetById(string idTinTuc)
        {
            var hashKey = EHashMd5.FromObject(idTinTuc);
            var rawKey = string.Concat("LoaiTinTuc-idTinTuc-", hashKey);
            var tinTuc = _cache.GetCacheKey<TinTucViewModel>(rawKey, _masterCacheKey)!;
            if (tinTuc == null)
            {
                tinTuc = _BL.GetById(idTinTuc);
                _cache.AddCacheItem(rawKey, tinTuc, _masterCacheKey);
            }
            return tinTuc;
        }
    }
}
