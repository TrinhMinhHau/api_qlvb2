using Amazon.Runtime.Internal.Util;
using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh
{
    public class HocSinhTmpCl
    {
        private string _masterCacheKeyHocSinhTmpCl = "HocSinhTmpCl";
        //private string _masterCacheKeySoGoc = "SoGocCache";
        //private string _masterCacheKeyHocSinh = "HocSinhCache";
        //private string _masterCacheKeyThongKe = "ThongKeCL";
        //private string _masterCacheKeyTrangChu = "TrangChuCache";

        private CacheLayer _cache;
        private HocSinhTmpBL _BL;

        public HocSinhTmpCl(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKeyHocSinhTmpCl);

            _BL = new HocSinhTmpBL(configuration);
        }

        public List<HocSinhTmpModel> GetSearchHocSinhTmp(out int total, HocSinhTmpModelParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HocSinhTmpCl-GetSearchHocSinhTmp-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKeyHocSinhTmpCl);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhTmpModel> hs = _cache.GetCacheKey<List<HocSinhTmpModel>>(rawKey, _masterCacheKeyHocSinhTmpCl)!;
            if (hs != null) return hs;
            // Item not found in cache - retrieve it and insert it into the cache
            hs = _BL.GetSearchHocSinhTmp(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hs, _masterCacheKeyHocSinhTmpCl);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKeyHocSinhTmpCl);
            return hs;
        }

        /// <summary>
        /// Thêm danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> SaveImport(List<HocSinhTmpModel> models)
        {
            var result = await _BL.SaveImport(models);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKeyHocSinhTmpCl);
                //_cache.InvalidateCache(_masterCacheKeySoGoc);
                //_cache.InvalidateCache(_masterCacheKeyHocSinh);
                //_cache.InvalidateCache(_masterCacheKeyThongKe);
                //_cache.InvalidateCache(_masterCacheKeyTrangChu);


            }
            return result;
        }

        public async Task<int> Delete(string nguoiThucHien)
        {
            var result = await _BL.DeleteImport(nguoiThucHien);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKeyHocSinhTmpCl);
                //_cache.InvalidateCache(_masterCacheKeySoGoc);
                //_cache.InvalidateCache(_masterCacheKeyHocSinh);
                //_cache.InvalidateCache(_masterCacheKeyThongKe);
                //_cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        public List<HocSinhTmpModel> GetAllHocSinhTmpSave(string idTruong, string nguoiThucHien, string idDanhMucTotNghiep)
        {
            string rawKey = string.Concat("GetAllHocSinhTmpSave", idTruong) + nguoiThucHien + idDanhMucTotNghiep;

            List<HocSinhTmpModel> hs = _cache.GetCacheKey<List<HocSinhTmpModel>>(rawKey, _masterCacheKeyHocSinhTmpCl)!;
            if (hs != null) return hs;
            hs = _BL.GetAllHocSinhTmpSave(idTruong, nguoiThucHien, idDanhMucTotNghiep);
            _cache.AddCacheItem(rawKey, hs, _masterCacheKeyHocSinhTmpCl);
            return hs;
        }

        public ThongKeHocSinhTmpModel GetThongKeHocSinhTmp(string idTruong, string nguoiThucHien, string idDanhMucTotNghiep)
        {
            string rawKey = string.Concat("GetThongKeHocSinhTmp", idTruong) + nguoiThucHien + idDanhMucTotNghiep;

            ThongKeHocSinhTmpModel hs = _cache.GetCacheKey<ThongKeHocSinhTmpModel> (rawKey, _masterCacheKeyHocSinhTmpCl)!;
            if (hs != null) return hs;
            hs = _BL.GetThongKeHocSinhTmp(idTruong, nguoiThucHien, idDanhMucTotNghiep);
            _cache.AddCacheItem(rawKey, hs, _masterCacheKeyHocSinhTmpCl);
            return hs;
        }
    }
}
