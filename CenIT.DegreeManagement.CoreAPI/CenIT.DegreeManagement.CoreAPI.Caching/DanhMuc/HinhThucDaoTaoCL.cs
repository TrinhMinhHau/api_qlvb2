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
    public class HinhThucDaoTaoCL
    {
        private string _masterCacheKey = "HinhThucDaoTaoCache";
        private CacheLayer _cache;
        private HinhThucDaoTaoBL _BL;

        public HinhThucDaoTaoCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new HinhThucDaoTaoBL(configuration);
        }


        /// <summary>
        /// Thêm mới hình thức đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(HinhThucDaoTaoInputModel model)
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
        /// Cập nhật hình thức đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(HinhThucDaoTaoInputModel model)
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
        /// Xóa hình thức đào tạo
        /// </summary>
        /// <param name="nguoiThucHien"></param>
        /// <param name="id"></param>
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
        /// Lấy danh sách hinh thuc đào tạo (param)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<HinhThucDaoTaoModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HTDTs-GetSearch-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<HinhThucDaoTaoModel> hinhThucDaoTaos = _cache.GetCacheKey<List<HinhThucDaoTaoModel>>(rawKey, _masterCacheKey)!;
            if (hinhThucDaoTaos != null) return hinhThucDaoTaos;

            hinhThucDaoTaos = _BL.GetSearch(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hinhThucDaoTaos, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return hinhThucDaoTaos;
        }

        /// <summary>
        /// Lấy hình thưc đào tạo theo Id
        /// </summary>
        /// <param name="hdtId"></param>
        /// <returns></returns>
        public HinhThucDaoTaoModel GetById(string hdtId)
        {
            var hashKey = EHashMd5.FromObject(hdtId);
            var rawKey = string.Concat("HTDTs-GetById-", hashKey);
            var hinhThucDaoTao = _cache.GetCacheKey<HinhThucDaoTaoModel>(rawKey, _masterCacheKey)!;
            if (hinhThucDaoTao == null)
            {
                hinhThucDaoTao = _BL.GetById(hdtId);
                _cache.AddCacheItem(rawKey, hinhThucDaoTao, _masterCacheKey);
            }
            return hinhThucDaoTao;
        }

        /// <summary>
        /// Lấy tất cả danh sách hình thức dào tạo
        /// </summary>
        /// <param name="hdtId"></param>
        /// <returns></returns>
        public List<HinhThucDaoTaoModel> GetAll()
        {
            var rawKey = string.Concat("HinhThucDaoTao-GetAll");

            //Get item from cache
            List<HinhThucDaoTaoModel> hinhThucDaoTaos = _cache.GetCacheKey<List<HinhThucDaoTaoModel>>(rawKey, _masterCacheKey)!;
            if (hinhThucDaoTaos == null)
            {
                hinhThucDaoTaos = _BL.GetAll();
                _cache.AddCacheItem(rawKey, hinhThucDaoTaos, _masterCacheKey);
            };

            return hinhThucDaoTaos;
        }
    }
}
