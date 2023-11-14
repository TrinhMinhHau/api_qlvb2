using CenIT.DegreeManagement.CoreAPI.Bussiness.Phoi;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Phoi
{
    public class PhoiBanSaoCL
    {
        private string _masterCacheKey = "PhoiBanSaoCache";
        private CacheLayer _cache;
        private PhoiBanSaoBL _BL;
        public PhoiBanSaoCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new PhoiBanSaoBL(configuration);
        }

        public async Task<int> Create(PhoiBanSaoInputModel model, List<CauHinhPhoiGocModel> cauHinhPhoiBanSaos)
        {
            var result =  await _BL.Create(model, cauHinhPhoiBanSaos);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> Modify(PhoiBanSaoInputModel model, string lyDo)
        {
            var result = await _BL.Modify(model, lyDo);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> Delete(string idPhoiGoc, string nguoiThucHien, string lyDo)
        {
            var result = await _BL.Delete(idPhoiGoc, nguoiThucHien, lyDo);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public PhoiBanSaoModel GetPhoiDangSuDung(string idTruong)
        {
            var rawKey = "PhoiBanSao-GetPhoiDangSuDung-";
            var phoiBanSao = _cache.GetCacheKey<PhoiBanSaoModel>(rawKey, _masterCacheKey)!;
            if (phoiBanSao == null)
            {
                phoiBanSao = _BL.GetPhoiDangSuDung(idTruong);
                _cache.AddCacheItem(rawKey, phoiBanSao, _masterCacheKey);
            }
            return phoiBanSao;
        }


        public List<PhoiBanSaoModel> GetSearchPhoiBanSao(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("PhoiBanSao-GetSearchPhoiBanSao-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<PhoiBanSaoModel> phoiBanSaos = _cache.GetCacheKey<List<PhoiBanSaoModel>>(rawKey, _masterCacheKey)!;
            if (phoiBanSaos != null) return phoiBanSaos;
            // Item not found in cache - retrieve it and insert it into the cache
            phoiBanSaos = _BL.GetSearchPhoiGoc(out total, modelSearch);
            _cache.AddCacheItem(rawKey, phoiBanSaos, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return phoiBanSaos;
        }

        public PhoiBanSaoModel GetById(string idPhoiBanSao)
        {
            var hashKey = EHashMd5.FromObject(idPhoiBanSao);
            var rawKey = string.Concat("PhoiBanSao-GetById-", hashKey);
            var phoiBanSao = _cache.GetCacheKey<PhoiBanSaoModel>(rawKey, _masterCacheKey)!;
            if (phoiBanSao == null)
            {
                phoiBanSao = _BL.GetById(idPhoiBanSao);
                _cache.AddCacheItem(rawKey, phoiBanSao, _masterCacheKey);
            }
            return phoiBanSao;
        }

        public List<PhoiBanSaoModel> GetAll()
        {
            string rawKey = string.Concat("PhoiBanSao-GetAll");

            // See if the item is in the cache
            List<PhoiBanSaoModel> phoiBanSaos = _cache.GetCacheKey<List<PhoiBanSaoModel>>(rawKey, _masterCacheKey)!;
            if (phoiBanSaos != null) return phoiBanSaos;
            // Item not found in cache - retrieve it and insert it into the cache
            phoiBanSaos = _BL.GetAll();
            _cache.AddCacheItem(rawKey, phoiBanSaos, _masterCacheKey);
            return phoiBanSaos;
        }

        public async Task<int> CauHinhBanSao(string idPhoiBanSao, string nguoiThucHien, List<CauHinhPhoiGocModel> cauHinhPhoiBanSaoModels)
        {
            var result = await _BL.CauHinhPhoiBanSao(idPhoiBanSao, nguoiThucHien, cauHinhPhoiBanSaoModels);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> HuyPhoi(string id)
        {
            var result = await _BL.HuyPhoi(id);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public List<CauHinhPhoiGocModel> GetCauHinhBanSao(string idphoiBanSao)
        {
            string rawKey = string.Concat("PhoiBanSao-GetCauHinhBanSao-", idphoiBanSao);

            // See if the item is in the cache
            List<CauHinhPhoiGocModel> cauHinhPhoiBanSaos = _cache.GetCacheKey<List<CauHinhPhoiGocModel>>(rawKey, _masterCacheKey)!;
            if (cauHinhPhoiBanSaos != null) return cauHinhPhoiBanSaos;
            // Item not found in cache - retrieve it and insert it into the cache
            cauHinhPhoiBanSaos = _BL.GetCauHinhPhoiBanSao(idphoiBanSao);
            _cache.AddCacheItem(rawKey, cauHinhPhoiBanSaos, _masterCacheKey);
            return cauHinhPhoiBanSaos;
        }

        public CauHinhPhoiGocModel GetCauHinhBanSaoById(string idPhoiBanSao, string idCauHinhPhoi)
        {
            var hashKey = EHashMd5.FromObject(idPhoiBanSao + idCauHinhPhoi);
            var rawKey = string.Concat("PhoiGoc-GetCauHinhPhoiGocById-", hashKey);
            var cauHinhPhoi = _cache.GetCacheKey<CauHinhPhoiGocModel>(rawKey, _masterCacheKey)!;
            if (cauHinhPhoi == null)
            {
                cauHinhPhoi = _BL.GetCauHinhBanSaoById(idPhoiBanSao, idCauHinhPhoi);
                _cache.AddCacheItem(rawKey, cauHinhPhoi, _masterCacheKey);
            }
            return cauHinhPhoi;
        }

        public async Task<int> ModifyCauHinhBanSao(string idPhoiGoc, CauHinhPhoiGocInputModel model)
        {
            var result = await _BL.ModifyCauHinhBanSao(idPhoiGoc, model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }
    }
}
