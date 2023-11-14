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
    public class SoCapPhatBangCL
    {
        private string _masterCacheKey = "SoCapPhatBangCache";
        private CacheLayer _cache;
        private SoCapPhatBangBL _BL;
        public SoCapPhatBangCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new SoCapPhatBangBL(configuration);
        }

        public string GetHocSinhTheoSoCapPhatBang(TruongViewModel truong, DanhMucTotNghiepViewModel dmtn, SoCapPhatBangSearchParam paramModel)
        {
            var hashKey = EHashMd5.FromObject(truong) + EHashMd5.FromObject(dmtn) + EHashMd5.FromObject(paramModel);
            var rawKey = string.Concat("SoCapPhatBang-GetHocSinhTheoSoCapPhatBang-", hashKey);
            var soCapPhatBangVM = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (soCapPhatBangVM == null)
            {
                soCapPhatBangVM = _BL.GetHocSinhTheoSoCapPhatBang(truong, dmtn, paramModel);
                _cache.AddCacheItem(rawKey, soCapPhatBangVM, _masterCacheKey);
            }
            return soCapPhatBangVM;
        }
    }
}
