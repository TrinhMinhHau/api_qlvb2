using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;

namespace CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc
{
    public class NamThiCL
    {
        private string _masterCacheKey = "NamThi_KhoaThi_Cache";
        private CacheLayer _cache;
        private NamThiBL _namThiBL;
        public NamThiCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _namThiBL = new NamThiBL(configuration);
        }
        #region Năm thi
        /// <summary>
        /// Lấy tất cả năm thi
        /// </summary>
        /// <returns></returns>
        public List<NamThiModel> GetAll()
        {
            var rawKey = string.Concat("NamThi-GetAll");

            //Get item from cache
            List<NamThiModel> namThis = _cache.GetCacheKey<List<NamThiModel>>(rawKey, _masterCacheKey)!;
            if (namThis == null)
            {
                namThis = _namThiBL.GetAll();
                _cache.AddCacheItem(rawKey, namThis, _masterCacheKey);
            };

            return namThis;
        }

        /// <summary>
        /// Lấy danh sách Năm Thi theo search param
        /// </summary>
        /// <returns></returns>
        public List<NamThiModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("NamThi-GetSearch-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;
            List<NamThiModel> namThis = _cache.GetCacheKey<List<NamThiModel>>(rawKey, _masterCacheKey)!;
            if (namThis != null) return namThis;

            namThis = _namThiBL.GetSearch(out total, modelSearch);
            _cache.AddCacheItem(rawKey, namThis, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return namThis;
        }

        /// <summary>
        /// Thêm năm thi
        /// </summary>
        /// <param name="model"></param>
        /// <param name="soGocModel"></param>
        /// <param name="soCapBanSaoModel"></param>
        /// <returns></returns>
        public async Task<int> Create(NamThiInputModel model, SoGocModel soGoc, SoCapBanSaoModel soCapBanSao, SoCapPhatBangModel soCapPhatBang)
        {
            var result = await _namThiBL.Create(model, soGoc, soCapBanSao, soCapPhatBang);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật năm thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(NamThiInputModel model)
        {
            var result = await _namThiBL.Modify(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Xóa năm thi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            var result = await _namThiBL.Delete(id, nguoiThucHien);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Lấy năm thi theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NamThiModel GetById(string id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("NamThi-GetById", hashKey);
            var namThi = _cache.GetCacheKey<NamThiModel>(rawKey, _masterCacheKey)!;
            if (namThi == null)
            {
                namThi = _namThiBL.GetById(id);
                _cache.AddCacheItem(rawKey, namThi, _masterCacheKey);
            };
            return namThi;
        }

        public NamThiViewModel GetByIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var hashKey = EHashMd5.FromObject(idDanhMucTotNghiep);
            var rawKey = string.Concat("NamThi_GetByIdDanhMucTotNghiep", hashKey);
            var namThi = _cache.GetCacheKey<NamThiViewModel>(rawKey, _masterCacheKey)!;
            if (namThi == null)
            {
                namThi = _namThiBL.GetNamThiByDanhMucTotNghiepId(idDanhMucTotNghiep);
                _cache.AddCacheItem(rawKey, namThi, _masterCacheKey);
            };
            return namThi;
        }

        #endregion

        #region Khóa thi
        /// <summary>
        /// insert khoa thi
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="khoaThiModel"></param>
        /// <returns></returns>
        public async Task<int> CreateKhoaThi(string idNamThi, KhoaThiInputModel khoaThiModel)
        {
            var result = await _namThiBL.CreateKhoaThi(idNamThi, khoaThiModel);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật khóa thi
        /// Action: 1-Update | 2-Delete
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="khoaThiModel"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int ModifyKhoaThi(string idNamThi, KhoaThiInputModel khoaThiModel)
        {
            var result = _namThiBL.ModifyKhoaThi(idNamThi, khoaThiModel);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }


        public int DeleteKhoaThi(string IdNamThi, string IdKhoaThi, string UserAction)
        {
            var result = _namThiBL.DeleteKhoaThi(IdNamThi, IdKhoaThi, UserAction);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả khóa thi
        /// </summary>
        /// <returns></returns>
        public List<KhoaThiModel> GetAllKhoaThi()
        {
            var rawKey = string.Concat("AllKhoaThis");

            //Get item from cache
            List<KhoaThiModel> namThis = _cache.GetCacheKey<List<KhoaThiModel>>(rawKey, _masterCacheKey)!;
            if (namThis == null)
            {
                namThis = _namThiBL.GetAllKhoaThi();
                _cache.AddCacheItem(rawKey, namThis, _masterCacheKey);
            };

            return namThis;
        }

        public List<KhoaThiModel> GetKhoaThisByNam(string idNamThi)
        {
            if (idNamThi == null) return null;

            var hashKey = EHashMd5.FromObject(idNamThi);
            var rawKey = string.Concat("GetKhoaThisByNam-", hashKey);

            //Get item from cache
            List<KhoaThiModel> namThis = _cache.GetCacheKey<List<KhoaThiModel>>(rawKey, _masterCacheKey)!;
            if (namThis == null)
            {
                namThis = _namThiBL.GetKhoaThiByNamThiId(idNamThi);
                _cache.AddCacheItem(rawKey, namThis, _masterCacheKey);
            };

            return namThis;
        }

        public List<KhoaThiModel> GetKhoaThiByIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var hashKey = EHashMd5.FromObject(idDanhMucTotNghiep);
            var rawKey = string.Concat("GetKhoaThiByIdDanhMucTotNghiep-", hashKey);

            //Get item from cache
            List<KhoaThiModel> namThis = _cache.GetCacheKey<List<KhoaThiModel>>(rawKey, _masterCacheKey)!;
            if (namThis == null)
            {
                namThis = _namThiBL.GetKhoaThiByIdDanhMucTotNghiep(idDanhMucTotNghiep);
                _cache.AddCacheItem(rawKey, namThis, _masterCacheKey);
            };

            return namThis;
        }

        /// <summary>
        /// Get khoa thi by IdS
        /// </summary>
        /// <param name="idKhoaThi"></param>
        /// <returns></returns>
        public KhoaThiModel? GetKhoaThiById(string idNamThi, string idKhoaThi)
        {
            var hashKey = EHashMd5.FromObject(string.Concat(idKhoaThi, idNamThi));
            var rawKey = string.Concat("KhoaThi-GetById-", hashKey);
            var khoaThi = _cache.GetCacheKey<KhoaThiModel>(rawKey, _masterCacheKey)!;
            if (khoaThi == null)
            {
                khoaThi = _namThiBL.GetKhoaThiById(idNamThi, idKhoaThi);
                _cache.AddCacheItem(rawKey, khoaThi, _masterCacheKey);
            };
            return khoaThi;
        }

        /// <summary>
        /// Tìm kiếm Khoa Thi 
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<KhoaThiModel> GetSearchKhoaThiByNamThiId(out int total, SearchParamModel modelSearch, string IdNamThi)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("KhoaThis-GetListBySearch-", objectKey, IdNamThi);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            List<KhoaThiModel> namThis = _cache.GetCacheKey<List<KhoaThiModel>>(rawKey, _masterCacheKey)!;
            if (namThis != null) return namThis;
            // Item not found in cache - retrieve it and insert it into the cache
            namThis = _namThiBL.GetSearchKhoaThiByNamThiId(out total, modelSearch, IdNamThi);
            _cache.AddCacheItem(rawKey, namThis);
            _cache.AddCacheItem(rawKeyTotal, total);
            return namThis;
        }
        #endregion
    }
}
