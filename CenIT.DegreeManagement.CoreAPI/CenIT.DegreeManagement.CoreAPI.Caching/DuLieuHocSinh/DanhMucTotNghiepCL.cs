using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.Extensions.Configuration;


namespace CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh
{
    public class DanhMucTotNghiepCL
    {
        private string _masterCacheKey = "DanhMucTotNghiepCache";
        private CacheLayer _cache;
        private DanhMucTotNghiepBL _BL;
        private string _masterCacheKeyThongKe = "ThongKeCL";
        private string _masterCacheKeyTrangChu = "TrangChuCL";


        public DanhMucTotNghiepCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            _BL = new DanhMucTotNghiepBL(configuration);
        }

        /// <summary>
        /// Lấy danh sách danh mục tốt nghiệp theo search param
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<DanhMucTotNghiepViewModel> GetSearch(out int total, DanhMucTotNghiepSearchParam modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("DanhMucTotNghieps-GetSearch-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<DanhMucTotNghiepViewModel> danhMucTotNghieps = _cache.GetCacheKey<List<DanhMucTotNghiepViewModel>>(rawKey, _masterCacheKey)!;
            if (danhMucTotNghieps != null) return danhMucTotNghieps;
            // Item not found in cache - retrieve it and insert it into the cache
            danhMucTotNghieps = _BL.GetSearch(out total, modelSearch);
            _cache.AddCacheItem(rawKey, danhMucTotNghieps, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return danhMucTotNghieps;
        }

        /// <summary>
        /// Lấy tất cả danh mục tốt nghiệp
        /// </summary>
        /// <returns></returns>
        public List<DanhMucTotNghiepViewModel> GetAll()
        {
            string rawKey = string.Concat("DanhMucTotNghieps-GetAll");

            List<DanhMucTotNghiepViewModel> danhMucTotNghieps = _cache.GetCacheKey<List<DanhMucTotNghiepViewModel>>(rawKey, _masterCacheKey)!;
            if (danhMucTotNghieps != null) return danhMucTotNghieps;
            danhMucTotNghieps = _BL.GetAll();
            _cache.AddCacheItem(rawKey, danhMucTotNghieps, _masterCacheKey);
            return danhMucTotNghieps;
        }

        /// <summary>
        /// Lấy danh sách danh mục tốt nghiệp chưa khóa
        /// </summary>
        /// <returns></returns>
        public List<DanhMucTotNghiepViewModel> GetAllUnBlock()
        {
            string rawKey = string.Concat("DanhMucTotNghieps-GetAllUnBlock");

            List<DanhMucTotNghiepViewModel> danhMucTotNghieps = _cache.GetCacheKey<List<DanhMucTotNghiepViewModel>>(rawKey, _masterCacheKey)!;
            if (danhMucTotNghieps != null) return danhMucTotNghieps;
            danhMucTotNghieps = _BL.GetAllUnBlock();
            _cache.AddCacheItem(rawKey, danhMucTotNghieps, _masterCacheKey);
            return danhMucTotNghieps;
        }

        /// <summary>
        /// Thêm danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(DanhMucTotNghiepInputModel model)
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
        /// Cập nhật danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(DanhMucTotNghiepInputModel model)
        {
            var result = await _BL.Modify(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);

            }
            return result;
        }

        /// <summary>
        /// Xóa danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Delete(string idDanhMucTotNghiep, string nguoiThucHien)
        {
            var result = await _BL.Delete(idDanhMucTotNghiep, nguoiThucHien);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Lấy danh mục tốt nghiệp theo id
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public DanhMucTotNghiepViewModel GetById(string idDanhMucTotNghiep)
        {
            var hashKey = EHashMd5.FromObject(idDanhMucTotNghiep);
            var rawKey = string.Concat("DanhMucTotNghiep-GetById-", hashKey);
            var danhMucTotNghiep = _cache.GetCacheKey<DanhMucTotNghiepViewModel>(rawKey, _masterCacheKey)!;
            if (danhMucTotNghiep == null)
            {
                danhMucTotNghiep = _BL.GetByID(idDanhMucTotNghiep);
                _cache.AddCacheItem(rawKey, danhMucTotNghiep, _masterCacheKey);
            }
            return danhMucTotNghiep;
        }

        /// <summary>
        /// Lấy danh mục tốt nghiệp theo idnamthi và idHHinhThucDaoTao
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idHinhThucDaoTao"></param>
        /// <returns></returns>
        public List<DanhMucTotNghiepViewModel> GetByIdNamThiAndMaHinhThucDaoTao(string idNamThi, string maHinhThucDaoTao, TruongModel donvi)
        {
            var hashKey = EHashMd5.FromObject(donvi);
            string rawKey = string.Concat("GetByIdNamThiAndMaHinhThucDaoTao", idNamThi + maHinhThucDaoTao) + hashKey;

            // See if the item is in the cache
            List<DanhMucTotNghiepViewModel> danhMucTotNghieps = _cache.GetCacheKey<List<DanhMucTotNghiepViewModel>>(rawKey, _masterCacheKey)!;
            if (danhMucTotNghieps != null) return danhMucTotNghieps;
            // Item not found in cache - retrieve it and insert it into the cache
            danhMucTotNghieps = _BL.GetByIdNamThiAndMaHinhThucDaoTao(idNamThi, maHinhThucDaoTao, donvi);
            _cache.AddCacheItem(rawKey, danhMucTotNghieps, _masterCacheKey);
            return danhMucTotNghieps;
        }

        /// <summary>
        /// Cập nhật trạng thái khóa mở danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="nguoiThucHien"></param>
        /// <param name="khoa"></param>
        /// <returns></returns>

        public async Task<int> KhoaDanhMucTotNghiep(string idDanhMucTotNghiep, string nguoiThucHien, bool khoa)
        {
            var result = await _BL.KhoaDanhMucTotNghiep(idDanhMucTotNghiep, nguoiThucHien, khoa);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật trạng thái in bằng danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public async Task<int> CapNhatTrangThaiDaInBang(string idDanhMucTotNghiep)
        {
            var result = await _BL.CapNhatTrangThaiDaInBang(idDanhMucTotNghiep);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật số lượng trường đẫ gửi
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public async Task<int> CapNhatSoLuongTruongDaGui(string idDanhMucTotNghiep, bool traLai = false)
        {
            var result = await _BL.CapNhatSoLuongTruongDaGui(idDanhMucTotNghiep, traLai);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật số lượng học sinh
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public async Task<int> CapNhatSoLuongHocSinh(string idDanhMucTotNghiep,  int soLuongHocSinh)
        {
            var result = await _BL.CapNhatSoLuongHocSinh(idDanhMucTotNghiep, soLuongHocSinh);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật số lượng trường đã duyệt
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public async Task<int> CapNhatSoLuongTruongDaDuyet(string idDanhMucTotNghiep)
        {
            var result = await _BL.CapNhatSoLuongTruongDaDuyet(idDanhMucTotNghiep);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        public List<DanhMucTotNghiepViewModel> GetAllByHeDaoTao(string maHeDaoTao)
        {
            string rawKey = string.Concat("DanhMucTotNghieps-GetAllByHeDaoTao", maHeDaoTao);

            List<DanhMucTotNghiepViewModel> danhMucTotNghieps = _cache.GetCacheKey<List<DanhMucTotNghiepViewModel>>(rawKey, _masterCacheKey)!;
            if (danhMucTotNghieps != null) return danhMucTotNghieps;
            danhMucTotNghieps = _BL.GetAllByHeDaoTao(maHeDaoTao);
            _cache.AddCacheItem(rawKey, danhMucTotNghieps, _masterCacheKey);
            return danhMucTotNghieps;
        }

        public List<DanhMucTotNghiepViewModel> GetAllByTruong(string idTruong)
        {
            string rawKey = string.Concat("DanhMucTotNghieps-GetAllByTruong", idTruong);

            List<DanhMucTotNghiepViewModel> danhMucTotNghieps = _cache.GetCacheKey<List<DanhMucTotNghiepViewModel>>(rawKey, _masterCacheKey)!;
            if (danhMucTotNghieps != null) return danhMucTotNghieps;
            danhMucTotNghieps = _BL.GetAllByTruong(idTruong);
            _cache.AddCacheItem(rawKey, danhMucTotNghieps, _masterCacheKey);
            return danhMucTotNghieps;
        }

        /// <summary>
        /// Cho phép trường gửi danh sách
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> CreateDanhMucTotNghiepViaTruong(DanhMucTotNghiepViaTruongInputModel model)
        {
            var result = await _BL.CreateDanhMucTotNghiepViaTruong(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public List<TruongHocViaDanhMucTotNghiepModel> GetTruong(out int total, string idDonVi, string idDanhMucTotNghiep, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("DanhMucTotNghieps-GetTruong-", objectKey, idDanhMucTotNghiep, idDonVi);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<TruongHocViaDanhMucTotNghiepModel> truongsWithPermission = _cache.GetCacheKey<List<TruongHocViaDanhMucTotNghiepModel>>(rawKey, _masterCacheKey)!;
            if (truongsWithPermission != null) return truongsWithPermission;
            truongsWithPermission = _BL.GetTruong(out total,idDonVi ,idDanhMucTotNghiep, modelSearch);
            _cache.AddCacheItem(rawKey, truongsWithPermission, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);

            return truongsWithPermission;
        }

        public List<TruongHocViaDanhMucTotNghiepModel> GetTruongHasPermision(out int total, string idDanhMucTotNghiep, SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("DanhMucTotNghieps-GetTruongHasPermision-", objectKey, idDanhMucTotNghiep);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal, _masterCacheKey);
            total = cacheTotal ?? 0;

            List<TruongHocViaDanhMucTotNghiepModel> truongs = _cache.GetCacheKey<List<TruongHocViaDanhMucTotNghiepModel>>(rawKey, _masterCacheKey)!;
            if (truongs != null) return truongs;
            truongs = _BL.GetTruongHasPermision(out total,idDanhMucTotNghiep, modelSearch);
            _cache.AddCacheItem(rawKey, truongs, _masterCacheKey);
            _cache.AddCacheItem(rawKeyTotal, total, _masterCacheKey);
            return truongs;
        }
    }
}
