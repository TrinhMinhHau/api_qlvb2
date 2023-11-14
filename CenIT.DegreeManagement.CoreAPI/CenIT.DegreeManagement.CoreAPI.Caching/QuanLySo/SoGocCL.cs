using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
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
    public class SoGocCL
    {
        private string _masterCacheKey = "SoGocCache";
        private CacheLayer _cache;
        private SoGocBL _BL;
        public SoGocCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new SoGocBL(configuration);
        }


        /// <summary>
        /// Lấy thông tin sổ gốc theo danh muc tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public SoGocModel GetbyIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var hashKey = EHashMd5.FromObject(idDanhMucTotNghiep);
            var rawKey = string.Concat("SoGoc-GetbyIdDanhMucTotNghiep-", hashKey);
            var soGoc = _cache.GetCacheKey<SoGocModel>(rawKey, _masterCacheKey)!;
            if (soGoc == null)
            {
                soGoc = _BL.GetbyIdDanhMucTotNghiep(idDanhMucTotNghiep);
                _cache.AddCacheItem(rawKey, soGoc, _masterCacheKey);
            }
            return soGoc;
        }

        public string GetHocSinhTheoSoGoc(TruongViewModel truong,DanhMucTotNghiepViewModel dmtn, SoGocSearchParam paramModel)
        {
            var hashKey = EHashMd5.FromObject(truong) + EHashMd5.FromObject(dmtn) + EHashMd5.FromObject(paramModel);
            var rawKey = string.Concat("SoGoc-GetHocSinhTheoSoGoc-", hashKey);
            var soGoc = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (soGoc == null)
            {
                soGoc = _BL.GetHocSinhTheoSoGoc(truong, dmtn, paramModel);
                _cache.AddCacheItem(rawKey, soGoc, _masterCacheKey);
            }
            return soGoc;
        }

    }
}
