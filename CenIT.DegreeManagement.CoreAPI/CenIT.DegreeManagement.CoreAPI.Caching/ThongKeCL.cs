using CenIT.DegreeManagement.CoreAPI.Bussiness;
using CenIT.DegreeManagement.CoreAPI.Bussiness.Phoi;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace CenIT.DegreeManagement.CoreAPI.Caching
{
    public class ThongKeCL
    {
        private string _masterCacheKey = "ThongKeCL";

        private CacheLayer _cache;
        private ThongKeBL _BL;
        public ThongKeCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new ThongKeBL(configuration);
        }

        public List<ThongKePhoiGocModel> ThongKePhoiGocDaIn(string idNamThi, string maHeDaoTao, string idPhoiGoc)
        {
            var hashKey = EHashMd5.FromObject(idPhoiGoc+ idNamThi+ maHeDaoTao);
            var rawKey = string.Concat("ThongKePhoiGocDaIn-", hashKey);
        

            var result = _cache.GetCacheKey<List<ThongKePhoiGocModel>>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.ThongKePhoiGocDaIn(idNamThi, maHeDaoTao, idPhoiGoc);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            }
            return result;
        }

        public List<ThongKeHocSinhTotNghiepTheoTruongModel> ThongKeHocSinhTotNghiepTheoTruong(out int total, string idNamThi, string idTruong)
        {
            var hashKey = EHashMd5.FromObject(idTruong + idNamThi);
            var rawKey = string.Concat("ThongKeHocSinhTotNghiepTheoTruong-", hashKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            var result = _cache.GetCacheKey<List<ThongKeHocSinhTotNghiepTheoTruongModel>>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.ThongKeHocSinhTotNghiepTheoTruong(out total, idNamThi, idTruong);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
                _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            }
            return result;
        }

        public List<ThongKeHocSinhTotNghiepTheoDMTNModel> ThongKeHocSinhDoTotNghiepTheoDMTN(out int total, string idTruong, string idNamThi,string idDanhMucTotNghiep)
        {
            var hashKey = EHashMd5.FromObject(idTruong + idNamThi);
            var rawKey = string.Concat("ThongKeHocSinhDoTotNghiepTheoDMTN-", hashKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            var result = _cache.GetCacheKey<List<ThongKeHocSinhTotNghiepTheoDMTNModel>>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.ThongKeHocSinhDoTotNghiepTheoDMTN(out total, idTruong, idNamThi, idDanhMucTotNghiep );
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
                _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            }
            return result;
        }

        public List<ThongKePhatBangModel> ThongKePhatBang( string idNamThi, string idTruong)
        {
            var hashKey = EHashMd5.FromObject(idTruong + idNamThi);
            var rawKey = string.Concat("ThongKePhatBang-", hashKey);

            var result = _cache.GetCacheKey<List<ThongKePhatBangModel>>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.ThongKePhatBang(idNamThi, idTruong);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            }
            return result;
        }

        public ThongKeTongQuatPhatBangModel ThongKeTongQuatPhatBang(string idNamThi)
        {
            var hashKey = EHashMd5.FromObject( idNamThi);
            var rawKey = string.Concat("ThongKeTongQuatPhatBang-", hashKey);

            var result = _cache.GetCacheKey<ThongKeTongQuatPhatBangModel>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.ThongKeTongQuatPhatBang(idNamThi);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            }
            return result;
        }

        public List<HocSinhListModel> GetHocSinhDoTotNghiepByTruongAndNam(out int total, HSByTruongNamSearchModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("GetHocSinhDoTotNghiepByTruongAndNam-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhListModel> hocSinhs = _cache.GetCacheKey<List<HocSinhListModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetHocSinhDoTotNghiepByTruongAndNam(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return hocSinhs;
        }

        public List<HocSinhListModel> GetHocSinhDTNByTruongAndNamOrDMTN(out int total, HSByTruongNamOrDMTNSearchModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("GetHocSinhDTNByTruongAndNamOrDMTN-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhListModel> hocSinhs = _cache.GetCacheKey<List<HocSinhListModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetHocSinhDTNByTruongAndNamOrDMTN(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return hocSinhs;
        }


        public List<ThongKePhoiGocModel> GetThongKeInPhoiBang(out int total, ThongKeInPhoiBangSearchModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("ThongKeInPhoiBangSearchModel-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<ThongKePhoiGocModel> phoiGocs = _cache.GetCacheKey<List<ThongKePhoiGocModel>>(rawKey, _masterCacheKey)!;
            if (phoiGocs != null) return phoiGocs;
            // Item not found in cache - retrieve it and insert it into the cache
            phoiGocs = _BL.GetThongKeInPhoiBang(out total, modelSearch);
            _cache.AddCacheItem(rawKey, phoiGocs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return phoiGocs;
        }

        public List<ThongKePhatBangModel> GetThongKePhatBang(out int total, ThongKePhatBangSearchModel modelSearch, string idCha)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idCha;
            string rawKey = string.Concat("GetThongKePhatBang-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<ThongKePhatBangModel> phoiGocs = _cache.GetCacheKey<List<ThongKePhatBangModel>>(rawKey, _masterCacheKey)!;
            if (phoiGocs != null) return phoiGocs;
            // Item not found in cache - retrieve it and insert it into the cache
            phoiGocs = _BL.GetThongKePhatBang(out total, modelSearch, idCha);
            _cache.AddCacheItem(rawKey, phoiGocs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return phoiGocs;
        }

        public List<HocSinhListModel> GetHocSinhDoTotNghiep(out int total, HocSinhTotNghiepSearchModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("GetHocSinhDoTotNghiep-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhListModel> hocSinhs = _cache.GetCacheKey<List<HocSinhListModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetHocSinhDoTotNghiep(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return hocSinhs;
        }
    }
}
