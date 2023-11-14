using CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo
{
    public class SoCapBanSaoCL
    {
        private string _masterCacheKey = "SoCapBaSaoCache";
        private CacheLayer _cache;
        private SoCapBanSaoBL _BL;
        public SoCapBanSaoCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new SoCapBanSaoBL(configuration);
        }

        public string GetHocSinhTheoCapBanSao(TruongViewModel truong ,DanhMucTotNghiepViewModel dmtn, SearchParamModel paramModel)
        {
            var hashKey = EHashMd5.FromObject(truong) + EHashMd5.FromObject(dmtn) + EHashMd5.FromObject(paramModel);
            var rawKey = string.Concat("SoBanSao-GetHocSinhTheoCapBanSao-", hashKey);
            var soBanSaoVM = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (soBanSaoVM == null)
            {
                soBanSaoVM = _BL.GetHocSinhTheoCapBanSao(truong, dmtn, paramModel);
                _cache.AddCacheItem(rawKey, soBanSaoVM, _masterCacheKey);
            }
            return soBanSaoVM;
        }

        public List<HocSinhCapBanSaoViewModel> GetHocSinhCapBanSao(out int total, SoCapBanSaoSearchParamModel paramModel)
        {
            string objectKey = EHashMd5.FromObject(paramModel);
            string rawKey = string.Concat("SoCapBanSao-GetHocSinhCapBanSao-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhCapBanSaoViewModel> hocSinhs = _cache.GetCacheKey<List<HocSinhCapBanSaoViewModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetHocSinhCapBanSao(out total, paramModel);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }
    }
}
