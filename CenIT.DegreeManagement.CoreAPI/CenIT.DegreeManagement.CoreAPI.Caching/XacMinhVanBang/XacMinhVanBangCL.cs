using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang
{
    public class XacMinhVanBangCL
    {
        private string _masterCacheKey = "XacMinhVanBangCL";
        private CacheLayer _cache;
        private XacMinhVanBangBL _BL;

        public XacMinhVanBangCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            _BL = new XacMinhVanBangBL(configuration);
        }

        public async Task<int> Create(XacMinhVangBangInputModel model, TruongModel donVi)
        {

            var result = await _BL.Create(model, donVi);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);

            }

            return result;
        }

        public async Task<int> CreateList(XacMinhVangBangListInputModel model, TruongModel donVi)
        {

            var result = await _BL.CreateList(model, donVi);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);

            }

            return result;
        }


        public string GetSearchLichSuXacMinh(LichSuXacMinhVanBangSearchModel modelSearch)
        {
            var hashKey = EHashMd5.FromObject(modelSearch);
            var rawKey = string.Concat("XacMinhVanBang-GetSearchLichSuXacMinh-", hashKey);
            var result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.GetSearchLichSuXacMinh(modelSearch);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            }
            return result;
        }

        public XacMinhVanBangListModel GetLichSuXacMinhById(string id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("XacMinhVanBang-GetLichSuXacMinhById-", hashKey);
            var result = _cache.GetCacheKey<XacMinhVanBangListModel>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.GetLichSuXacMinhById(id);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            }
            return result;
        }


    }
}
