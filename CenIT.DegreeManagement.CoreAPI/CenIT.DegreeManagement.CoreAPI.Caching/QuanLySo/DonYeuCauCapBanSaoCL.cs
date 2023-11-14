using CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
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
    public class DonYeuCauCapBanSaoCL
    {
        private string _masterCacheKey = "DonYeuCauCapBanSaoCache";
        private string _masterCacheKeyHocSinh = "HocSinhCache";
        private string _masterCacheKeyPhoiBanSao = "PhoiBanSaoCache";
        private string _masterCacheKeyTrangChu = "TrangChuCL";
        private string _masterCacheKeySoCapBanSao = "SoCapBanSaoCache";



        private CacheLayer _cache;
        private DonYeuCauCapBanSaoBL _BL;
        public DonYeuCauCapBanSaoCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new DonYeuCauCapBanSaoBL(configuration);
        }

        /// <summary>
        /// Thêm đơn yêu cầu cấp bản sao
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> CreateDonYeuCau(DonYeuCauCapBanSaoInputModel model, string idTruong)
        {
            var result = await _BL.CreateDonYeuCau(model, idTruong);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyHocSinh);
                _cache.InvalidateCache(_masterCacheKeyPhoiBanSao);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }


        public string GetSerachDonYeuCapBanSao(DonYeuCauCapBanSaoParamModel modelSearch, TruongModel donVi)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + EHashMd5.FromObject(donVi);
            string rawKey = string.Concat("GetSerachDonYeuCapBanSao-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSerachDonYeuCapBanSao(modelSearch, donVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        public List<DonYeuCauCapBanSaoViewModel> GetSearchDonYeuCau(out int total, DonYeuCauCapBanSaoParamModel modelSearch, TruongModel donVi)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + EHashMd5.FromObject(donVi);
            string rawKey = string.Concat("DonYeuCau-GetSearchDonYeuCau-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<DonYeuCauCapBanSaoViewModel> donYeuCaus = _cache.GetCacheKey<List<DonYeuCauCapBanSaoViewModel>>(rawKey, _masterCacheKey)!;
            if (donYeuCaus != null) return donYeuCaus;

            donYeuCaus = _BL.GetSearchDonYeuCau(out total, modelSearch, donVi);
            _cache.AddCacheItem(rawKey, donYeuCaus, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return donYeuCaus;
        }

        public List<DonYeuCauCapBanSaoViewModel> GetSearchDonYeuCauCongThongTin(out int total, TraCuuDonYeuCau modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("DonYeuCau-GetSearchDonYeuCauCongThongTin-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<DonYeuCauCapBanSaoViewModel> donYeuCaus = _cache.GetCacheKey<List<DonYeuCauCapBanSaoViewModel>>(rawKey, _masterCacheKey)!;
            if (donYeuCaus != null) return donYeuCaus;

            donYeuCaus = _BL.GetSearchDonYeuCauCongThongTin(out total, modelSearch);
            _cache.AddCacheItem(rawKey, donYeuCaus, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return donYeuCaus;
        }


        public List<DonYeuCauCapBanSaoViewModel> GetSearchDonYeuCauDaDuyet(out int total, HocSinhCapBanSaoParamModel modelSearch, TruongModel donVi)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + EHashMd5.FromObject(donVi);
            string rawKey = string.Concat("DonYeuCau-GetSearchDonYeuCauDaDuyet-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<DonYeuCauCapBanSaoViewModel> donYeuCaus = _cache.GetCacheKey<List<DonYeuCauCapBanSaoViewModel>>(rawKey, _masterCacheKey)!;
            if (donYeuCaus != null) return donYeuCaus;

            donYeuCaus = _BL.GetSearchDonYeuCauDaDuyet(out total, modelSearch, donVi);
            _cache.AddCacheItem(rawKey, donYeuCaus, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return donYeuCaus;
        }


        public DonYeuCauCapBanSaoViewModel GetById(string idDonYeuCauCapBanSao)
        {
            var hashKey = EHashMd5.FromObject(idDonYeuCauCapBanSao);
            var rawKey = string.Concat("DonYeuCau-GetById-", hashKey);
            var donYeuCau = _cache.GetCacheKey<DonYeuCauCapBanSaoViewModel>(rawKey, _masterCacheKey)!;
            if (donYeuCau == null)
            {
                donYeuCau = _BL.GetById(idDonYeuCauCapBanSao);
                _cache.AddCacheItem(rawKey, donYeuCau, _masterCacheKey);
            }
            return donYeuCau;
        }


        public async Task<DonYeuCauOutPutResult> TuChoiDoYeuCau(string idDonYeuCauCapBanSao, string lyDoTuChoi , string nguoiThucHien)
        {
            var result = await _BL.TuChoiDoYeuCau(idDonYeuCauCapBanSao, lyDoTuChoi, nguoiThucHien);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);

            }
            return result;
        }

        public async Task<DonYeuCauOutPutResult> DuyetDonYeuCau(DuyetDonYeuCauInputModel model)
        {
            var result = await _BL.DuyetDonYeuCau(model);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyPhoiBanSao);
                _cache.InvalidateCache(_masterCacheKeyHocSinh);
                _cache.InvalidateCache(_masterCacheKeySoCapBanSao);
            }
            return result;
        }

        public async Task<int> XacNhanIn(string idDonYeuCauCapBanSao, string nguoiThucHien)
        {
            var result = await _BL.XacNhanIn(idDonYeuCauCapBanSao, nguoiThucHien);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public List<DonYeuCauCapBanSaoViewModel> GetSearchLichSuDonYeuCau(out int total,string idHocSinh ,SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idHocSinh;
            string rawKey = string.Concat("DonYeuCau-GetSearchLichSuDonYeuCau-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<DonYeuCauCapBanSaoViewModel> donYeuCaus = _cache.GetCacheKey<List<DonYeuCauCapBanSaoViewModel>>(rawKey, _masterCacheKey)!;
            if (donYeuCaus != null) return donYeuCaus;

            donYeuCaus = _BL.GetSearchLichSuDonYeuCau(out total,idHocSinh ,modelSearch);
            _cache.AddCacheItem(rawKey, donYeuCaus, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return donYeuCaus;
        }

        public async Task<int> XacNhanDaPhat(string idDonYeuCauCapBanSao)
        {
            var result = await _BL.XacNhanDaPhat(idDonYeuCauCapBanSao);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }
    }
}
