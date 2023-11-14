using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc
{
    public class HeDaoTaoCL
    {
        private string _masterCacheKey = "HeDaoTaoCache";
        private CacheLayer _cache;
        private HeDaoTaoBL _BL;
        public HeDaoTaoCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new HeDaoTaoBL(configuration);
        }

        /// <summary>
        /// Thêm Hệ đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(HeDaoTaoInputModel model)
        {
            var result = await _BL.Create(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật hệ đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(HeDaoTaoInputModel model)
        {
            var result = await _BL.Modify(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Xóa hệ đào tạo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            var result = await _BL.Delete(id, nguoiThucHien);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }


        /// <summary>
        /// Lấy danh sách hệ đào tạo (param)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<HeDaoTaoModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HeDaoTao-GetSearch-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<HeDaoTaoModel> heDaoTaos = _cache.GetCacheKey<List<HeDaoTaoModel>>(rawKey, _masterCacheKey)!;
            if (heDaoTaos != null) return heDaoTaos;

            heDaoTaos = _BL.GetSearch(out total, modelSearch);
            _cache.AddCacheItem(rawKey, heDaoTaos, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return heDaoTaos;
        }

        /// <summary>
        /// Lấy hệ đào tạo theo Id
        /// </summary>
        /// <param name="idHeDaoTao"></param>
        /// <returns></returns>
        public HeDaoTaoModel GetById(string idHeDaoTao)
        {
            var hashKey = EHashMd5.FromObject(idHeDaoTao);
            var rawKey = string.Concat("HeDaoTao-GetById-", hashKey);
            var heDaoTao = _cache.GetCacheKey<HeDaoTaoModel>(rawKey, _masterCacheKey)!;
            if (heDaoTao == null)
            {
                heDaoTao = _BL.GetById(idHeDaoTao);
                _cache.AddCacheItem(rawKey, heDaoTao, _masterCacheKey);
            }
            return heDaoTao;
        }

        /// <summary>
        /// Lấy tất cả hệ đào tạo
        /// </summary>
        /// <returns></returns>
        public List<HeDaoTaoModel> GetAll()
        {
            var rawKey = string.Concat("HeDaoTao-GetAll");

            //Get item from cache
            List<HeDaoTaoModel> hdts = _cache.GetCacheKey<List<HeDaoTaoModel>>(rawKey, _masterCacheKey)!;
            if (hdts == null)
            {
                hdts = _BL.GetAll();
                _cache.AddCacheItem(rawKey, hdts, _masterCacheKey);
            };

            return hdts;
        }
    }
}
