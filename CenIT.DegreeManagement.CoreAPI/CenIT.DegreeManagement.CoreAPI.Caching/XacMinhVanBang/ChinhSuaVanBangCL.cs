using Amazon.Runtime.Internal.Util;
using CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.QuanLySo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang
{
    public class ChinhSuaVanBangCL
    {
        private string _masterCacheKey = "LichSuChinhSuaVanBangCL";

        private string _masterCacheKeyHocSinh = "HocSinhCache";

        private CacheLayer _cache;
        private ChinhSuaVanBangBL _BL;

        public ChinhSuaVanBangCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            _BL = new ChinhSuaVanBangBL(configuration);
        }

        public async Task<HocSinhResult> Create(ChinhSuaVanBangInputModel model, TruongModel donvi)
        {

            var result = await _BL.Create(model, donvi);
            if (result.MaLoi > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyHocSinh);
            }

            return result;
        }

        public List<PhuLucSoGocModel> GetSearchLichSuChinhSuaVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idHocSinh;
            string rawKey = string.Concat("HocSinhs-GetSearchLichSuChinhSuaVanBang-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<PhuLucSoGocModel> hocSinhs = _cache.GetCacheKey<List<PhuLucSoGocModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSerachChinhSuaVanBang(out total, idHocSinh, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }

        public LichSuChinhSuaModel GetChinhSuaVanBangById(string idPhuLuc)
        {
            string rawKey = string.Concat("HocSinhs-GetChinhSuaVanBangById-", idPhuLuc);

            // See if the item is in the cache
            LichSuChinhSuaModel hocSinhs = _cache.GetCacheKey<LichSuChinhSuaModel>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetChinhSuaVanBangById(idPhuLuc);
            _cache.AddCacheItem(rawKey, hocSinhs);
            return hocSinhs;
        }

        #region Phụ lục
        public List<PhuLucSoGocViewModel> GetSerachPhuLuc(out int total, PhuLucSoGocSearchModel searchModel)
        {
            string objectKey = EHashMd5.FromObject(searchModel);
            string rawKey = string.Concat("HocSinhs-GetSerachPhuLuc-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<PhuLucSoGocViewModel> hocSinhs = _cache.GetCacheKey<List<PhuLucSoGocViewModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSerachPhuLuc(out total, searchModel);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }

        public PhuLucSoGocModel GetThongTinChinhSuaMoiNhat(string idHocSinh)
        {
            string rawKey = string.Concat("HocSinhs-GetThongTinChinhSuaMoiNhat-", idHocSinh);

            // See if the item is in the cache
            PhuLucSoGocModel hocSinhs = _cache.GetCacheKey<PhuLucSoGocModel>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetThongTinChinhSuaMoiNhat(idHocSinh);
            _cache.AddCacheItem(rawKey, hocSinhs);
            return hocSinhs;
        }

        public List<PhuLucSoGocModel> GetSearchLichSuCapLaiVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idHocSinh;
            string rawKey = string.Concat("HocSinhs-GetSearchLichSuCapLaiVanBang-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<PhuLucSoGocModel> hocSinhs = _cache.GetCacheKey<List<PhuLucSoGocModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSerachCapLaiVanBang(out total, idHocSinh, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }

        #endregion

        #region Cổng thông tin
        public List<PhuLucSoGocModel> GetSerachPhuLucChinhSuaVanBang(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HocSinhs-GetSerachPhuLucChinhSuaVanBang-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<PhuLucSoGocModel> hocSinhs = _cache.GetCacheKey<List<PhuLucSoGocModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSerachPhuLucChinhSuaVanBang(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }

        public List<PhuLucSoGocModel> GetSerachPhuLucCapLaiVanBang(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HocSinhs-GetSerachPhuLucCapLaiVanBang-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<PhuLucSoGocModel> hocSinhs = _cache.GetCacheKey<List<PhuLucSoGocModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSerachPhuLucCapLaiVanBang(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }
        #endregion

    }


}
