using CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang
{
    public class HuyBoVangBangCL
    {
        private string _masterCacheKey = "HuyBoVangBangCL";
        private string _masterCacheKeyHocSinh = "HocSinhCache";

        private CacheLayer _cache;
        private HuyBoVanBangBL _BL;

        public HuyBoVangBangCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            _BL = new HuyBoVanBangBL(configuration);
        }

        public async Task<int> Create(HuyBoVangBangInputModel model)
        {

            var result = await _BL.Create(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyHocSinh);
            }

            return result;
        }

        public List<LichSuHuyBoViewModel> GetSerachHuyBoVanBangByIdHocSinh(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idHocSinh;
            string rawKey = string.Concat("HocSinhs-GetSearchLichSuHuyBoVanBang-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<LichSuHuyBoViewModel> hocSinhs = _cache.GetCacheKey<List<LichSuHuyBoViewModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSerachHuyBoVanBangByIdHocSinh(out total, idHocSinh, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }

        public List<LichSuHuyBoViewModel> GetSerachHuyBoVanBang(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HocSinhs-GetSerachHuyBoVanBang-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<LichSuHuyBoViewModel> hocSinhs = _cache.GetCacheKey<List<LichSuHuyBoViewModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSerachHuyBoVanBang(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }


        //public HuyBoVangBangModel GetHuyBoVanBangById(string cccd, string idLichSuChinhSua)
        //{
        //    string objectKey = cccd + idLichSuChinhSua;
        //    string rawKey = string.Concat("HocSinhs-GetHuyBoVanBangById-", objectKey);

        //    // See if the item is in the cache
        //    HuyBoVangBangModel hocSinhs = _cache.GetCacheKey<HuyBoVangBangModel>(rawKey, _masterCacheKey)!;
        //    if (hocSinhs != null) return hocSinhs;
        //    // Item not found in cache - retrieve it and insert it into the cache
        //    hocSinhs = _BL.GetHuyBoVanBangById(cccd, idLichSuChinhSua);
        //    _cache.AddCacheItem(rawKey, hocSinhs);
        //    return hocSinhs;
        //}
    }
}
