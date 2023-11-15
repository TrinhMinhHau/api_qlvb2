using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe;
using Microsoft.Extensions.Configuration;


namespace CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh
{
    public class HocSinhCL
    {
        private string _masterCacheKey = "HocSinhCache";
        private string _masterCacheKeySoGoc = "SoGocCache";
        private string _masterCacheKeySoCapBanSao = "SoCapBanSaoCaches";
        private string _masterCacheKeySoCapPhatBang = "SoCapPhatBang";
        private string _masterCacheKeyThongKe = "ThongKeCL";
        private string _masterCacheKeyTrangChu = "TrangChuCache";

        private CacheLayer _cache;
        private HocSinhBL _BL;
        public HocSinhCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new HocSinhBL(configuration);
        }

        #region Dùng chung
        /// <summary>
        /// Thêm từng học sinh
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(HocSinhInputModel model, bool phong = false)
        {
            var result = await _BL.Create(model, phong);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        /// <summary>
        /// Import danh sách học sinh file excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ImportResultModel> ImportHocSinh(List<HocSinhModel> models, string idTruong, string idDanhMucTotNghiep, bool phong = false)
        {
            ImportResultModel result = await _BL.ImportHocSinh(models, idTruong, idDanhMucTotNghiep, phong);
            if (result.ErrorCode > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }


        /// <summary>
        /// Update HỌC SINH
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(HocSinhInputModel model)
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
        /// Lấy hoc sinh theo cccd
        /// </summary>
        /// <param name="hdtId"></param>
        /// <returns></returns>
        public HocSinhViewModel GetHocSinhByCccd(string cccd)
        {
            var hashKey = EHashMd5.FromObject(cccd);
            var rawKey = string.Concat("HocSinh-GetByCccd-", hashKey);
            var hocSinh = _cache.GetCacheKey<HocSinhViewModel>(rawKey, _masterCacheKey)!;
            if (hocSinh == null)
            {
                hocSinh = _BL.GetHocSinhByCccd(cccd);
                _cache.AddCacheItem(rawKey, hocSinh, _masterCacheKey);
            }
            return hocSinh;
        }

        /// <summary>
        /// Update HỌC SINH
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public  int TongSoHocSinhTheoTrangThai(string idDanhMucTotNghiep, string idTruong, List<TrangThaiHocSinhEnum> trangThais)
        {
            var hashKey = EHashMd5.FromObject(trangThais) + EHashMd5.FromObject(idDanhMucTotNghiep) + EHashMd5.FromObject(idTruong);
            var rawKey = string.Concat("TongSoHocSinhTheoTrangThai-", hashKey);
            var total = _cache.GetCacheKey<int>(rawKey, _masterCacheKey)!;
            if (total == 0)
            {
                total = _BL.TongSoHocSinhTheoTrangThai(idDanhMucTotNghiep, idTruong, trangThais);
                _cache.AddCacheItem(rawKey, total, _masterCacheKey);
            }
            return total;
        }

        #endregion

        #region Học sinh trường
        /// <summary>
        /// Xóa tất cả học sinh chưa xác nhận theo matruong
        /// </summary>
        /// <param name="maTruong"></param>
        /// <returns></returns>
        public async Task<int> XoaTatCaHocSinhChuaXacNhan(string idTruong)
        {
            var result = await _BL.XoaTatCaHocSinhChuaXacNhan(idTruong);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);

            }
            return result;
        }

        /// <summary>
        /// Xóa hoc sinh chưa xác nhận theo danh sách
        /// </summary>
        /// <param name="maTruong"></param>
        /// <returns></returns>
        public async Task<int> XoaHocSinhChuaXacNhan(string idTruong, List<string> listCCCD)
        {
            var result = await _BL.XoaHocSinhChuaXacNhan(idTruong, listCCCD);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách học sinh by trường (Chưa xác nhận, đang chờ duyệt, đã duyệt, đã đưa vào sổ gốc)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSearchHocSinhByTruong(string idTruong, HocSinhParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idTruong;
            string rawKey = string.Concat("HocSinhs-GetSearchHocSinhByTruong-", objectKey);

          
            // See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            // Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSearchHocSinhByTruong( idTruong, modelSearch);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Xác nhận tất cae học sinh
        /// </summary>
        /// <param name="maTruong"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> XacNhanTatCaHocSinh(string idTruong, string idDanhMucTotNghiep)
        {
            var result = await _BL.XacNhanTatCaHocSinh(idTruong, idDanhMucTotNghiep);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }


        /// <summary>
        /// Xác nhận học sinh theo danh sách
        /// </summary>
        /// <param name="maTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> XacNhanHocSinh(string idTruong, string idDanhMucTotNghiep , List<string> listCCCD)
        {
            var result = await _BL.XacNhanHocSinh(idTruong, idDanhMucTotNghiep, listCCCD);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);

            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả danh sách học sinh đã duyệt the id truong và danh mục tốt nghiệp để in bằng tạm thời
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public List<HocSinhViewModel> GetAllHocSinhDaDuyet(string idTruong, string idDanhMucTotNghiep)
        {
            var hashKey = EHashMd5.FromObject(idTruong + idDanhMucTotNghiep);
            var rawKey = string.Concat("HocSinh-GetAllHocSinhDaDuyet-", hashKey);
            var hocSinhs = _cache.GetCacheKey<List<HocSinhViewModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs == null)
            {
                hocSinhs = _BL.GetAllHocSinhDaDuyet(idTruong, idDanhMucTotNghiep);
                _cache.AddCacheItem(rawKey, hocSinhs, _masterCacheKey);
            }
            return hocSinhs;
        }

        /// <summary>
        /// Lấy tất cả danh sách học sinh đã duyệt the id truong và danh mục tốt nghiệp để in bằng tạm thời
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public List<HocSinhViewModel> GetHocSinhDaDuyetByCCCD(string idTruong, List<string> listCCCD)
        {
            var hashKey = EHashMd5.FromObject(listCCCD) + idTruong;
            var rawKey = string.Concat("HocSinh-GetHocSinhDaDuyetByCCCD-", hashKey);
            var hocSinhs = _cache.GetCacheKey<List<HocSinhViewModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs == null)
            {
                hocSinhs = _BL.GetHocSinhDaDuyetByCCCD(idTruong, listCCCD);
                _cache.AddCacheItem(rawKey, hocSinhs, _masterCacheKey);
            }
            return hocSinhs;
        }

        #endregion

        #region Học Sinh Phòng
        /// <summary>
        /// Lấy danh sách học sinh đang chờ duyệt
        /// (Phòng giáo dục và đào tạo)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSearchHocSinhChoDuyetByPhong(string? idTruong, HocSinhParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idTruong;
            string rawKey = string.Concat("HocSinhs-GetSearchHocSinhChoDuyetByPhong-", objectKey);

            // See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            // Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSearchHocSinhChoDuyetByPhong(idTruong, modelSearch);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Trả lại tất cả học sinh
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> TraLaiTatCaHocSinh(string idTruong, string idDanhMucTotNghiep)
        {
            var result = await _BL.TraLaiTatCaHocSinh(idTruong, idDanhMucTotNghiep);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }


        /// <summary>
        /// Trả lại học sinh theo danh ssách
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> TraLaiDanhSachHocSinh(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            var result = await _BL.TraLaiHocSinh(idTruong, idDanhMucTotNghiep, listCCCD);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        /// <summary>
        /// Duyệt tất cả học sinh
        /// </summary>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> DuyetTatCaHocSinh(string idTruong, string idDanhMucTotNghiep)
        {
            var result = await _BL.DuyetTatCaHocSinh(idTruong, idDanhMucTotNghiep);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }


        /// <summary>
        /// Xác nhận học sinh theo danh sách
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> DuyetDanhSachHocSinh(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            var result = await _BL.DuyetDanhSachHocSinh(idTruong, idDanhMucTotNghiep, listCCCD);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }

        #endregion

        #region Cấp bằng
        public List<HocSinhModel> GetSearchHocSinhCapBangByPhong(out int total, string idTruong, HocSinhParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idTruong;
            string rawKey = string.Concat("HocSinhs-GetSearchHocSinhCapBangByPhong-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhModel> hocSinhs = _cache.GetCacheKey<List<HocSinhModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSearchHocSinhCapBangByPhong(out total, idTruong, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }


        public HocSinhResult GetAllPreviewHocSinhVaoSoGoc(string idTruong, string idDanhMucTotNghiep)
        {
            string objectKey = EHashMd5.FromObject(idTruong + idDanhMucTotNghiep);
            string rawKey = string.Concat("GetAllPreview", objectKey);
            string rawKeyNam = string.Concat("GetAllPreview-Nam", objectKey);


            var hocSinhs = _cache.GetCacheKey<List<HocSinhModel>>(rawKey, _masterCacheKey)!;
            var nam = _cache.GetCacheKey<string>(rawKeyNam, _masterCacheKey)!;

            if (hocSinhs != null && nam != null) 
                return new HocSinhResult
                {
                    MaLoi = (int)HocSinhEnum.Success,
                    HocSinhs = hocSinhs,
                    Nam = nam
                };
            // Item not found in cache - retrieve it and insert it into the cache
            var result = _BL.GetAllPreviewHocSinhVaoSoGoc(idTruong, idDanhMucTotNghiep);
            if (result.MaLoi > 0)
            {
                _cache.AddCacheItem(rawKey, result.HocSinhs, _masterCacheKey);
                _cache.AddCacheItem(rawKeyNam, result.Nam, _masterCacheKey);

            }
            return result;
        }

        public async Task<HocSinhResult> PutIntoSoGoc(SoGocInputModel model)
        {
            string objectKey = EHashMd5.FromObject(model.IdTruong + model.IdDanhMucTotNghiep);
            string rawKey = string.Concat("GetAllPreview", objectKey);
            string rawKeyNam = string.Concat("GetAllPreview-Nam", objectKey);

            var hocSinhs = _cache.GetCacheKey<List<HocSinhModel>>(rawKey, _masterCacheKey)!;
            var nam = _cache.GetCacheKey<string>(rawKeyNam, _masterCacheKey)!;
            if (hocSinhs != null && nam != null)
            {
                var result = await _BL.PutIntoSoGoc(model, hocSinhs);
                if (result.MaLoi > 0)
                {
                    // Invalidate the cache
                    _cache.InvalidateCache(_masterCacheKey);
                    _cache.InvalidateCache(_masterCacheKeySoGoc);
                    _cache.InvalidateCache(_masterCacheKeyThongKe);


                }

                result.Nam = nam;
                result.SoluongHocSinh = hocSinhs.Count();

                return result;
            }
           
            return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty};
        }

        public HocSinhInBangModel GetHocSinhDaDuaVaoSoGocById(string idHocSinh)
        {
            string objectKey = EHashMd5.FromObject(idHocSinh);
            string rawKey = string.Concat("GetHocSinhDaDuaVaoSoGocById", objectKey);

            var hocSinh = _cache.GetCacheKey<HocSinhInBangModel>(rawKey, _masterCacheKey)!;
            if (hocSinh != null) return hocSinh;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinh = _BL.GetHocSinhDaDuaVaoSoGocById(idHocSinh);
            _cache.AddCacheItem(rawKey, hocSinh, _masterCacheKey);
            return hocSinh;
        }

        public List<HocSinhInBangModel> GetAllHocSinhDaDuaVaoSo(string idTruong, string idDanhMucTotNghiep)
        {
            string objectKey = EHashMd5.FromObject(idTruong + idDanhMucTotNghiep);
            string rawKey = string.Concat("GetDanhSachHocSinhDaDuaVaoSo", objectKey);

            var hocSinhs = _cache.GetCacheKey<List<HocSinhInBangModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetAllHocSinhDaDuaVaoSo(idTruong, idDanhMucTotNghiep);
            _cache.AddCacheItem(rawKey, hocSinhs, _masterCacheKey);
            return hocSinhs;
        }

        public List<HocSinhInBangModel> GetListHocSinhDaDuaVaoSo(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            string objectKey = EHashMd5.FromObject(idTruong + idDanhMucTotNghiep) + EHashMd5.FromObject(listCCCD);
            string rawKey = string.Concat("GetListHocSinhDaDuaVaoSo", objectKey);

            var hocSinhs = _cache.GetCacheKey<List<HocSinhInBangModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetListHocSinhDaDuaVaoSoByCCCD(idTruong, idDanhMucTotNghiep, listCCCD);
            _cache.AddCacheItem(rawKey, hocSinhs, _masterCacheKey);
            return hocSinhs;
        }


        /// <summary>
        /// Xác nhận in bằng
        /// </summary>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> XacNhanInBang(string idDanhMucTotNghiep, string idTruong,List<string> listCCCD)
        {
            var result = await _BL.XacNhanInBang(idDanhMucTotNghiep, idTruong, listCCCD);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
            }
            return result;
        }

        public async Task<HocSinhResult> XacNhanInBangTatCa(string idTruong, string idDanhMucTotNghiep)
        {
            var result = await _BL.XacNhanInBangTatCa(idTruong, idDanhMucTotNghiep);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);


            }
            return result;
        }

        public async Task<int> CapBangTatCa(string idTruong, string idDanhMucTotNghiep)
        {
            var result = await _BL.CapBangTatCa(idTruong, idDanhMucTotNghiep);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);

            }
            return result;
        }

        public async Task<int> CapBang(string idTruong, string idDanhMucTotNghiep, List<string> cccd)
        {
            var result = await _BL.CapBang(idTruong, idDanhMucTotNghiep, cccd);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
            }
            return result;
        }


        #endregion

        #region Cấp bản sao
        public HocSinhInBangModel GetHocSinhDaDuaVaoSoBanSao(string idHocSinh ,string idDonYeuCau, TruongModel donVi)
        {
            string objectKey = EHashMd5.FromObject(idDonYeuCau + idHocSinh);
            string rawKey = string.Concat("GetHocSinhDaDuaVaoSoBanSao", objectKey);

            var hocSinh = _cache.GetCacheKey<HocSinhInBangModel>(rawKey, _masterCacheKey)!;
            if (hocSinh != null) return hocSinh;
            hocSinh = _BL.GetHocSinhDaDuaVaoSoBanSao(idHocSinh, idDonYeuCau, donVi);
            _cache.AddCacheItem(rawKey, hocSinh, _masterCacheKey);
            return hocSinh;
        }
        #endregion

        #region Cấp phát bằng
        public List<HocSinhModel> GetSearchHocSinhCapPhatBangByTruong(out int total, string idTruong, HocSinhCapPhatBangParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idTruong;
            string rawKey = string.Concat("HocSinhs-GetSearchHocSinhCapPhatBangByTruong-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhModel> hocSinhs = _cache.GetCacheKey<List<HocSinhModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSearchHocSinhCapPhatBangByTruong(out total, idTruong, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }


        /// <summary>
        /// Thêm Hệ đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> CapPhatBang(ThongTinPhatBangInputModel model)
        {
            var result = await _BL.CapPhatBang(model);
            if (result > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
                _cache.InvalidateCache(_masterCacheKeySoCapPhatBang);
            }
            return result;
        }

        public HocSinhCapPhatBangViewModel GetHocSinhPhatBangByCccd(string cccd)
        {
            var hashKey = EHashMd5.FromObject(cccd);
            var rawKey = string.Concat("HocSinh-GetHocSinhPhatBangByCccd-", hashKey);
            var hocSinh = _cache.GetCacheKey<HocSinhCapPhatBangViewModel>(rawKey, _masterCacheKey)!;
            if (hocSinh == null)
            {
                hocSinh = _BL.GetHocSinhCapPhatBangByCccd(cccd);
                _cache.AddCacheItem(rawKey, hocSinh, _masterCacheKey);
            }
            return hocSinh;
        }
        #endregion

        #region Trang chủ
        public ThongKeSoLuongBangModel GetThongKeSoLuongHocSinhNhanBang(string idTruong)
        {
            var rawKey = "HocSinh-GetThongKeSoLuongHocSinhNhanBang" + idTruong;
            var result = _cache.GetCacheKey<ThongKeSoLuongBangModel>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.GetThongKeSoLuongHocSinhNhanBang(idTruong);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            }
            return result;
        }

        public ThongKeHocSinhTongQuatModel GetThongKeHocSinhTongQuat(string idTruong, string idNamThi)
        {
            var rawKey = "HocSinh-GetThongKeHocSinhTongQuat" + idTruong + idNamThi;
            var result = _cache.GetCacheKey<ThongKeHocSinhTongQuatModel>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.GetThongKeHocSinhTongQuat(idTruong, idNamThi);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);

            }
            return result;
        }

        public List<ThongKeSoLuongXepLoaiTheoNamModel> ThongKeSoLuongXepLoaiTheoNam(string idTruong)
        {
            var rawKey = "HocSinh-ThongKeSoLuongXepLoaiTheoNam" + idTruong;
            var result = _cache.GetCacheKey<List<ThongKeSoLuongXepLoaiTheoNamModel>>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.ThongKeSoLuongXepLoaiTheoNam(idTruong);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            }
            return result;
        }

        public List<ThongKeSoLuongHocSinhTheoNamModel> ThongKeSoLuongHocSinhTheoNam(string idTruong)
        {
            var rawKey = "HocSinh-ThongKeSoLuongHocSinhTheoNam" + idTruong;
            var result = _cache.GetCacheKey<List<ThongKeSoLuongHocSinhTheoNamModel>>(rawKey, _masterCacheKey)!;
            if (result == null)
            {
                result = _BL.ThongKeSoLuongHocSinhTheoNam(idTruong);
                _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            }
            return result;
        }

        #endregion

        #region Cổng thông tin
        public List<HocSinhViewModel> GetSearchVBCC(out int total, TraCuuVBCCModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HocSinhs-GetSearchVBCC-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhViewModel> hocSinhs = _cache.GetCacheKey<List<HocSinhViewModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSearchVBCC(out total, modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }
        #endregion

        #region TraCứu

        public string GetSearchHocSinhXacMinhVanBang(HocSinhSearchXacMinhVBModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HocSinhs-GetSearchHocSinhXacMinhVanBang-", objectKey);


            // See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            // Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSearchHocSinhXacMinhVanBang(modelSearch);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }


        public List<HocSinhXMVBModel> GetSearchHocSinhXacMinhVB(out int total, HocSinhSearchXacMinhVBModel modelSearch,string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(modelSearch);
            string rawKey = string.Concat("HocSinhs-GetSearchHocSinhXacMinhVB-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<HocSinhXMVBModel> hocSinhs = _cache.GetCacheKey<List<HocSinhXMVBModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSearchHocSinhXacMinhVB(out total, modelSearch, idDonVi);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }


        public List<HocSinhModel> GetAllHocSinhDaCoSoHieu()
        {
            string rawKey = string.Concat("HocSinhs-GetAllHocSinhDaCoSoHieu-");

            // See if the item is in the cache
            List<HocSinhModel> hocSinhs = _cache.GetCacheKey<List<HocSinhModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetAllHocSinhDaCoSoHieu();
            _cache.AddCacheItem(rawKey, hocSinhs);
            return hocSinhs;
        }

        public string[] GetAllArrCCCD()
        {
            string rawKey = string.Concat("HocSinhs-GetAllArrCCCD-");

            // See if the item is in the cache
            string[] arrCCCD = _cache.GetCacheKey<string[]> (rawKey, _masterCacheKey)!;

            if (arrCCCD != null) return arrCCCD;
            // Item not found in cache - retrieve it and insert it into the cache
            arrCCCD = _BL.GetAllArrCCCD();
            _cache.AddCacheItem(rawKey, arrCCCD);
            return arrCCCD;
        }

        public async Task<HocSinhResult> SaveImport(string idTruong,string idDanhMucTotNghiep ,List<HocSinhModel> models)
        {
            var result = await _BL.SaveImport(idTruong, idDanhMucTotNghiep, models);
            if (result.MaLoi > 0)
            {
                // Invalidate the cache
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeySoGoc);
                _cache.InvalidateCache(_masterCacheKeyTrangChu);
                _cache.InvalidateCache(_masterCacheKeyThongKe);
            }
            return result;
        }


        #endregion

    }
}
