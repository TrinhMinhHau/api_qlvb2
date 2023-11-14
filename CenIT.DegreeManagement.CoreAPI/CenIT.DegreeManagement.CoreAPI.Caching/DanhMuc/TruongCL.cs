using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using Microsoft.Extensions.Configuration;

namespace CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc
{
    public class TruongCL
    {
        private string _masterCacheKey = "TruongCache";
        private string _masterCacheKeyTrangChu = "TrangChuCache";

        private CacheLayer _cache;
        private TruongBL _BL;
        public TruongCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new TruongBL(configuration);
        }

        /// <summary>
        /// Thêm trường trường
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(TruongInputModel model)
        {
            var result = await _BL.Create(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);

            }
            return result;
        }

        /// <summary>
        /// Update trường
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(TruongInputModel model)
        {
            var result = await _BL.Modify(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);

            }
            return result;
        }

        /// <summary>
        /// Xóa trường
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
        /// Lấy trường theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TruongViewModel? GetById(string id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("Truong-GetById-", hashKey);
            var truong = _cache.GetCacheKey<TruongViewModel>(rawKey, _masterCacheKey)!;
            if (truong == null)
            {
                truong = _BL.GetById(id);
                _cache.AddCacheItem(rawKey, truong, _masterCacheKey);
            }
            return truong;
        }

        public TruongViewModel? GetPhong(string idDonVi)
        {
            var rawKey = string.Concat("Truong-GetPhong-", idDonVi);
            var truong = _cache.GetCacheKey<TruongViewModel>(rawKey, _masterCacheKey)!;
            if (truong == null)
            {
                truong = _BL.GetPhong(idDonVi);
                _cache.AddCacheItem(rawKey, truong, _masterCacheKey);
            }
            return truong;
        }

        public TruongViewModel? GetDonViQuanLySo()
        {
            var rawKey = string.Concat("Truong-GetPhong-");
            var truong = _cache.GetCacheKey<TruongViewModel>(rawKey, _masterCacheKey)!;
            if (truong == null)
            {
                truong = _BL.GetDonViQuanLySo();
                _cache.AddCacheItem(rawKey, truong, _masterCacheKey);
            }
            return truong;
        }

        /// <summary>
        /// Lấy tất cả trương
        /// </summary>
        /// <returns></returns>
        public List<TruongViewModel> GetAll(string idHinhThucDaoTao = null)
        {
            var rawKey = string.Concat("Truongs-GetAll");

            //Get item from cache
            List<TruongViewModel> truongs = _cache.GetCacheKey<List<TruongViewModel>>(rawKey, _masterCacheKey)!;
            if (truongs == null)
            {
                truongs = _BL.GetAll(idHinhThucDaoTao);
                _cache.AddCacheItem(rawKey, truongs, _masterCacheKey);
            };

            return truongs;
        }

        public List<TruongViewModel> GetByMaHeDaoTao(string maHeDaoTao)
        {
            var rawKey = string.Concat("Truongs-GetByMaHeDaoTao" + maHeDaoTao);

            //Get item from cache
            List<TruongViewModel> truongs = _cache.GetCacheKey<List<TruongViewModel>>(rawKey, _masterCacheKey)!;
            if (truongs == null)
            {
                truongs = _BL.GetByMaHeDaoTao(maHeDaoTao);
                _cache.AddCacheItem(rawKey, truongs, _masterCacheKey);
            };

            return truongs;
        }

        /// <summary>
        /// Lấy tất cả trương
        /// </summary>
        /// <returns></returns>
        public List<TruongModel> GetAllHavePhong()
        {
            var rawKey = string.Concat("Truongs-GetAllHavePhong");

            //Get item from cache
            List<TruongModel> truongs = _cache.GetCacheKey<List<TruongModel>>(rawKey, _masterCacheKey)!;
            if (truongs == null)
            {
                truongs = _BL.GetAllHavePhong();
                _cache.AddCacheItem(rawKey, truongs, _masterCacheKey);
            };

            return truongs;
        }

        public List<TruongModel> GetAllTruong(string idDonVi)
        {
            var rawKey = string.Concat("Truongs-GetAllTruong", idDonVi);

            //Get item from cache
            List<TruongModel> truongs = _cache.GetCacheKey<List<TruongModel>>(rawKey, _masterCacheKey)!;
            if (truongs == null)
            {
                truongs = _BL.GetAllTruong(idDonVi);
                _cache.AddCacheItem(rawKey, truongs, _masterCacheKey);
            };

            return truongs;
        }


        public List<TruongModel> GetAllDonViCha()
        {
            var rawKey = string.Concat("Truongs-GetAllDonViCha");

            //Get item from cache
            List<TruongModel> donVis = _cache.GetCacheKey<List<TruongModel>>(rawKey, _masterCacheKey)!;
            if (donVis == null)
            {
                donVis = _BL.GetAllDonViCha();
                _cache.AddCacheItem(rawKey, donVis, _masterCacheKey);
            };

            return donVis;
        }


        /// <summary>
        /// Lấy danh sách trường 
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<TruongViewModel> GetSearch(out int total, SearchParamModel modelSearch, string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idDonVi;
            string rawKey = string.Concat("Truongs-GetSearch-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            List<TruongViewModel> truongs = _cache.GetCacheKey<List<TruongViewModel>>(rawKey, _masterCacheKey)!;
            if (truongs != null) return truongs;
            // Item not found in cache - retrieve it and insert it into the cache
            truongs = _BL.GetSearch(out total, modelSearch, idDonVi);
            _cache.AddCacheItem(rawKey, truongs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return truongs;
        }


        /// <summary>
        /// Lấy tất cả các đơn vị theo id đơn vị quản lý
        /// </summary>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        public List<TruongModel> GetAllDonViByUsername(string idDonVi)
        {
            var rawKey = string.Concat("Truongs-GetAllDonViByUsername", idDonVi);

            //Get item from cache
            List<TruongModel> donVis = _cache.GetCacheKey<List<TruongModel>>(rawKey, _masterCacheKey)!;
            if (donVis == null)
            {
                donVis = _BL.GetAllDonViByUsername(idDonVi);
                _cache.AddCacheItem(rawKey, donVis, _masterCacheKey);
            };

            return donVis;
        }

        #region Cau Hinh Truong
        /// <summary>
        /// insert cấu hình 
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="cauHinhModel"></param>
        /// <returns></returns>
        public int SaveCauHinh(string idTruong, CauHinhInputModel cauHinhModel)
        {
            var result = _BL.SaveCauHinh(idTruong, cauHinhModel);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public int UpdateSoDonYeuCau(string idTruong)
        {
            var result = _BL.UpdateSoDonYeuCau(idTruong);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }


        public int UpdateCauHinhSoVaoSo(UpdateCauHinhSoVaoSoInputModel model)
        {
            var result = _BL.UpdateCauHinhSoVaoSo(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Lấy thông tin cấu hình theo id trường
        /// </summary>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public CauHinhModel? GetCauHinhByIdTruong(string idTruong)
        {
            var hashKey = EHashMd5.FromObject(string.Concat(idTruong));
            var rawKey = string.Concat("CauHinh-GetByIdTruong-", hashKey);
            var cauHinh = _cache.GetCacheKey<CauHinhModel>(rawKey, _masterCacheKey)!;
            if (cauHinh == null)
            {
                cauHinh = _BL.GetCauHinhByIdTruong(idTruong);
                if (cauHinh == null)
                {
                    return null;
                }
                _cache.AddCacheItem(rawKey, cauHinh, _masterCacheKey);
            };
            return cauHinh;
        }

        /// <summary>
        /// Lấy thông tin cấu hình theo ma trường
        /// </summary>
        /// <param name="maTruong"></param>
        /// <returns></returns>
        public CauHinhModel? GetCauHinhByMaTruong(string maTruong)
        {
            var hashKey = EHashMd5.FromObject(string.Concat(maTruong));
            var rawKey = string.Concat("CauHinh-GetByMaTruong-", hashKey);
            var cauHinh = _cache.GetCacheKey<CauHinhModel>(rawKey, _masterCacheKey)!;
            if (cauHinh == null)
            {
                cauHinh = _BL.GetCauHinhByMaTruong(maTruong);
                if (cauHinh == null)
                {
                    return null;
                }
                _cache.AddCacheItem(rawKey, cauHinh, _masterCacheKey);
            };
            return cauHinh;
        }

        public CauHinhXacMinhVanBangModel? GetCauHinhXacMinhByIdDonVi(string idDonVi)
        {
            var hashKey = EHashMd5.FromObject(string.Concat(idDonVi));
            var rawKey = string.Concat("CauHinh-GetCauHinhXacMinhByIdTruong-", hashKey);
            var cauHinh = _cache.GetCacheKey<CauHinhXacMinhVanBangModel>(rawKey, _masterCacheKey)!;
            if (cauHinh == null)
            {
                cauHinh = _BL.GetCauHinhXacMinhByIdDonVi(idDonVi);
                if (cauHinh == null)
                {
                    return null;
                }
                _cache.AddCacheItem(rawKey, cauHinh, _masterCacheKey);
            };
            return cauHinh;
        }

        #endregion
    }
}
