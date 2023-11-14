//using CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang;
//using CenIT.DegreeManagement.CoreAPI.Core.Caching;
//using CenIT.DegreeManagement.CoreAPI.Core.Models;
//using CenIT.DegreeManagement.CoreAPI.Core.Utils;
//using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
//using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang
//{
//    public class CapLaiVanBangCL
//    {
//        private string _masterCacheKey = "CapLaiVanBangCL";
//        private string _masterCacheKeyHocSinh = "HocSinhCache";

//        private CacheLayer _cache;
//        private CapLaiVanBangBL _BL;

//        public CapLaiVanBangCL(ICacheService cacheService, IConfiguration configuration)
//        {
//            _cache = (CacheLayer)cacheService;
//            _cache.setMasterKey(_masterCacheKey);

//            _BL = new CapLaiVanBangBL(configuration);
//        }

//        public async Task<int> Create(CapLaiVangBangInputModel model)
//        {

//            var result = await _BL.Create(model);
//            if (result > 0)
//            {
//                _cache.InvalidateCache(_masterCacheKey);
//                _cache.InvalidateCache(_masterCacheKeyHocSinh);
//            }

//            return result;
//        }

//        public List<CapLaiVanBangModel> GetSearchLichSuCapLaiVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
//        {
//            string objectKey = EHashMd5.FromObject(modelSearch) + idHocSinh;
//            string rawKey = string.Concat("HocSinhs-GetSearchLichSuCapLaiVanBang-", objectKey);
//            string rawKeyTotal = string.Concat(rawKey, "-Total");

//            total = 0;
//            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
//            total = cacheTotal ?? 0;

//            // See if the item is in the cache
//            List<CapLaiVanBangModel> hocSinhs = _cache.GetCacheKey<List<CapLaiVanBangModel>>(rawKey, _masterCacheKey)!;
//            if (hocSinhs != null) return hocSinhs;
//            // Item not found in cache - retrieve it and insert it into the cache
//            hocSinhs = _BL.GetSerachCapLaiVanBang(out total, idHocSinh, modelSearch);
//            _cache.AddCacheItem(rawKey, hocSinhs);
//            _cache.AddCacheItem(rawKeyTotal, total);
//            return hocSinhs;
//        }

//        public CapLaiVanBangModel GetCapLaiVanBangById(string cccd, string idLichSuCapLai)
//        {
//            string objectKey = cccd + idLichSuCapLai;
//            string rawKey = string.Concat("HocSinhs-GetCapLaiVanBangById-", objectKey);

//            // See if the item is in the cache
//            CapLaiVanBangModel hocSinhs = _cache.GetCacheKey<CapLaiVanBangModel>(rawKey, _masterCacheKey)!;
//            if (hocSinhs != null) return hocSinhs;
//            // Item not found in cache - retrieve it and insert it into the cache
//            hocSinhs = _BL.GetCapLaiVanBangById(cccd, idLichSuCapLai);
//            _cache.AddCacheItem(rawKey, hocSinhs);
//            return hocSinhs;
//        }
//    }
//}
