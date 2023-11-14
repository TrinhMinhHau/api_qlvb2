using CenIT.DegreeManagement.CoreAPI.Bussiness.Phoi;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe;
using Microsoft.Extensions.Configuration;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Phoi
{
    public class PhoiGocCL
    {
        private string _masterCacheKey = "PhoiGocCache";
        private string _masterCacheKeyThongKe = "ThongKeCL";
        private string _masterCacheKeyTrangChu = "TrangChuCL";



        private CacheLayer _cache;
        private PhoiGocBL _BL;
        public PhoiGocCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new PhoiGocBL(configuration);
        }

        public async Task<int> Create(PhoiGocInputModel model, List<CauHinhPhoiGocModel> cauHinhPhoiGocs)
        {
            var result = await _BL.Create(model, cauHinhPhoiGocs);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);

            }
            return result;
        }

        public async Task<int> Modify(PhoiGocInputModel model, string lyDo)
        {
            var result = await _BL.Modify(model, lyDo);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);


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

        public PhoiGocModel GetById(string idPhoiGoc)
        {
            var hashKey = EHashMd5.FromObject(idPhoiGoc);
            var rawKey = string.Concat("PhoiGoc-GetById-", hashKey);
            var phoiGoc = _cache.GetCacheKey<PhoiGocModel>(rawKey, _masterCacheKey)!;
            if (phoiGoc == null)
            {
                phoiGoc = _BL.GetById(idPhoiGoc);
                _cache.AddCacheItem(rawKey, phoiGoc, _masterCacheKey);
            }
            return phoiGoc;
        }

        public PhoiGocModel GetPhoiDangSuDung(string idTruong)
        {
            var rawKey = "PhoiGoc-GetPhoiDangSuDung-" + idTruong;
            var phoiGoc = _cache.GetCacheKey<PhoiGocModel>(rawKey, _masterCacheKey)!;
            if (phoiGoc == null)
            {
                phoiGoc = _BL.GetPhoiDangSuDung(idTruong);
                _cache.AddCacheItem(rawKey, phoiGoc, _masterCacheKey);
            }
            return phoiGoc;
        }

        public PhoiGocModel GetPhoiDangSuDungByHDT(string maHeDaoTao)
        {
            var rawKey = "PhoiGoc-GetPhoiDangSuDungByHeDaoTao-" + maHeDaoTao;
            var phoiGoc = _cache.GetCacheKey<PhoiGocModel>(rawKey, _masterCacheKey)!;
            if (phoiGoc == null)
            {
                phoiGoc = _BL.GetPhoiDangSuDungByHDT(maHeDaoTao);
                _cache.AddCacheItem(rawKey, phoiGoc, _masterCacheKey);
            }
            return phoiGoc;
        }

        public List<PhoiGocModel> GetSearchPhoiGoc(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("PhoiGocs-GetListBySearch-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<PhoiGocModel> phoiGocs = _cache.GetCacheKey<List<PhoiGocModel>>(rawKey, _masterCacheKey)!;
            if (phoiGocs != null) return phoiGocs;
            // Item not found in cache - retrieve it and insert it into the cache
            phoiGocs = _BL.GetSearchPhoiGoc(out total, modelSearch);
            _cache.AddCacheItem(rawKey, phoiGocs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return phoiGocs;
        }

        public List<PhoiGocModel> GetAll()
        {
            string rawKey = string.Concat("PhoiGocs-GetAll");

            // See if the item is in the cache
            List<PhoiGocModel> phoiGocs = _cache.GetCacheKey<List<PhoiGocModel>>(rawKey, _masterCacheKey)!;
            if (phoiGocs != null) return phoiGocs;
            // Item not found in cache - retrieve it and insert it into the cache
            phoiGocs = _BL.GetAll();
            _cache.AddCacheItem(rawKey, phoiGocs, _masterCacheKey);
            return phoiGocs;
        }

        public async Task<int> CauHinhPhoiGoc(string idPhoiGoc, string nguoiThucHien, List<CauHinhPhoiGocModel> cauHinhPhoiGocModels)
        {
            var result = await _BL.CauHinhPhoiGoc(idPhoiGoc, nguoiThucHien, cauHinhPhoiGocModels);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> HuyPhoi(HuyPhoiGocInputModel model)
        {
            var result = await _BL.HuyPhoi(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public async Task<int> CapNhatThongSoPhoi(string soBatDau, string idPhoi, int soLuongPhoi)
        {
            var result = await _BL.CapNhatThongSoPhoi(soBatDau, idPhoi, soLuongPhoi);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);


            }
            return result;
        }

        public async Task<int> CapNhatSoLuongPhoi(int soHocSinh, string idPhoi)
        {
            var result = await _BL.CapNhatSoLuongPhoiGoc(soHocSinh, idPhoi);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        public List<CauHinhPhoiGocModel> GetCauHinhPhoiGoc(string idphoiGoc)
        {
            string rawKey = string.Concat("PhoiGocs-GetCauHinhPhoiGoc", idphoiGoc);

            // See if the item is in the cache
            List<CauHinhPhoiGocModel> cauHinhPhoiGocs = _cache.GetCacheKey<List<CauHinhPhoiGocModel>>(rawKey, _masterCacheKey)!;
            if (cauHinhPhoiGocs != null) return cauHinhPhoiGocs;
            // Item not found in cache - retrieve it and insert it into the cache
            cauHinhPhoiGocs = _BL.GetCauHinhPhoiGoc(idphoiGoc);
            _cache.AddCacheItem(rawKey, cauHinhPhoiGocs, _masterCacheKey);
            return cauHinhPhoiGocs;
        }

        public List<PhoiGocModel> GetSearchPhoiDaHuy(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("PhoiGocs-GetSearchPhoiDaHuy-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<PhoiGocModel> phoiGocs = _cache.GetCacheKey<List<PhoiGocModel>>(rawKey, _masterCacheKey)!;
            if (phoiGocs != null) return phoiGocs;
            // Item not found in cache - retrieve it and insert it into the cache
            phoiGocs = _BL.GetSearchPhoiDaHuy(out total, modelSearch);
            _cache.AddCacheItem(rawKey, phoiGocs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return phoiGocs;
        }

        public CauHinhPhoiGocModel GetCauHinhPhoiGocById(string idPhoiGoc, string idCauHinhPhoi)
        {
            var hashKey = EHashMd5.FromObject(idPhoiGoc + idCauHinhPhoi);
            var rawKey = string.Concat("PhoiGoc-GetCauHinhPhoiGocById-", hashKey);
            var cauHinhPhoi = _cache.GetCacheKey<CauHinhPhoiGocModel>(rawKey, _masterCacheKey)!;
            if (cauHinhPhoi == null)
            {
                cauHinhPhoi = _BL.GetCauHinhPhoiGocById(idPhoiGoc, idCauHinhPhoi);
                _cache.AddCacheItem(rawKey, cauHinhPhoi, _masterCacheKey);
            }
            return cauHinhPhoi;
        }

        public async Task<int> ModifyCauHinhPhoiGoc(string idPhoiGoc, CauHinhPhoiGocInputModel model)
        {
            var result = await _BL.ModifyCauHinhPhoiGoc(idPhoiGoc, model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }
    }
}
