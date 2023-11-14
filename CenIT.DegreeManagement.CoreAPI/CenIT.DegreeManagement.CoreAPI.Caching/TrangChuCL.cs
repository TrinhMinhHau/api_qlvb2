using CenIT.DegreeManagement.CoreAPI.Bussiness;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching
{
    public class TrangChuCL
    {
        private string _masterCacheKey = "TrangChuCache";

        private CacheLayer _cache;
        private TrangChuBL _BL;
        public TrangChuCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _BL = new TrangChuBL(configuration);
        }

        #region Phòng
        /// <summary>
        /// Tra cứu học sinh tốt nghiệp theo cccd và họ tên (Trang chủ - phòng)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public string GetTraCuuHocSinhTotNghiep(string idDonVi,TraCuuHocHinhTotNghiepSearchModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idDonVi;
            string rawKey = string.Concat("TrangChu_GetTraCuuHocSinhTotNghiep-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetTraCuuHocSinhTotNghiep(idDonVi, modelSearch);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Lấy số lượng phôi đã in theo năm học và hedaotao (Trang chủ - phòng)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongPhoiDaIn(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + maHeDaoTao + idDonVi);
            string rawKey = string.Concat("TrangChu_GetSoLuongPhoiDaIn-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoLuongPhoiDaIn(idNamThi, maHeDaoTao, idDonVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Lấy số lượng đơn vị đã gửi theo năm thi và hệ đào tạo (Trang chủ - phòng)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongDonViDaGui(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + maHeDaoTao + idDonVi);
            string rawKey = string.Concat("TrangChu_GetSoLuongDonViDaGui-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoLuongDonViDaGui(idNamThi, maHeDaoTao, idDonVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }


        /// <summary>
        /// Lấy số lượng phôi đơn yêu cầu cấp bản sao năm học và hedaotao (Trang chủ - phòng)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        /// <returns></returns>
        public string GetSoLuongDonYeuCauCapBanSao(string idNamThi, string maHeDaoTao, string idDonVi )
        {
            string objectKey = EHashMd5.FromObject(idNamThi + maHeDaoTao + idDonVi);
            string rawKey = string.Concat("TrangChu_GetSoLuongDonYeuCauCapBanSao-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoLuongDonYeuCauCapBanSao(idNamThi, maHeDaoTao, idDonVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        public string GetThongKeTongQuatByTruong(string idTruong, string idNamThi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + idTruong);
            string rawKey = string.Concat("GetThongKeTongQuatByTruong-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetThongKeTongQuatByTruong(idTruong, idNamThi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        public string GetThongKeTongQuatByPhong(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + maHeDaoTao + idDonVi);
            string rawKey = string.Concat("GetThongKeTongQuatByPhong-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetThongKeTongQuatByPhong(idNamThi, maHeDaoTao, idDonVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Lấy số lượng học sinh chưa duyệt theo năm học và hedaotao (Trang chủ - phòng)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongHocSinhChuaDuyet(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + maHeDaoTao + idDonVi);
            string rawKey = string.Concat("TrangChu_GetSoLuongHocSinhChuaDuyet-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoLuongHocSinhChuaDuyet(idNamThi, maHeDaoTao, idDonVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }


        /// <summary>
        /// Lấy số lượng học sinh qua từng năm (Biểu đồ trang chủ)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongHocSinhQuaTungNam(string maHeDaoTao, string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(maHeDaoTao + idDonVi);
            string rawKey = string.Concat("TrangChu_GetSoLuongHocSinhQuaTungNam-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoLuongHocSinhQuaTungNam(maHeDaoTao, idDonVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Lấy số lượng học sinh theo xếp loại (Biểu đồ trang chủ)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongHocSinhTheoXepLoai(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(maHeDaoTao + idNamThi+ idDonVi);
            string rawKey = string.Concat("TrangChu_GetSoLuongHocSinhTheoXepLoai-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoLuongHocSinhTheoXepLoai(idNamThi,maHeDaoTao, idDonVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Lấy số lượng học sinh cấp phát bằng (Biểu đồ trang chủ)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongHocSinhCapPhatBang(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            string objectKey = EHashMd5.FromObject(maHeDaoTao + idNamThi + idDonVi);
            string rawKey = string.Concat("TrangChu_GetSoLuongHocSinhCapPhatBangi-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoLuongHocSinhCapPhatBang(idNamThi, maHeDaoTao, idDonVi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        #endregion

        #region Trường
        /// <summary>
        /// Lấy tổng số học sinh của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetTongSoHocSinhByTruong(string idTruong, string idNamThi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + idTruong);
            string rawKey = string.Concat("TrangChu_GetTongSoHocSinhByTruong-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetTongSoHocSinhByTruong(idTruong, idNamThi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Lấy số học sinh chờ duyệt của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSoHocSinhChoDuyetByTruong(string idTruong, string idNamThi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + idTruong);
            string rawKey = string.Concat("TrangChu_GetSoHocSinhChoDuyetByTruong-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoHocSinhChoDuyetByTruong(idTruong, idNamThi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }

        /// <summary>
        /// Lấy số học sinh đã duyệt của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSoHocSinhDaDuyetByTruong(string idTruong, string idNamThi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + idTruong);
            string rawKey = string.Concat("TrangChu_GetSoHocSinhDaDuyetByTruong-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoHocSinhDaDuyetByTruong(idTruong, idNamThi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }


        /// <summary>
        /// Lấy số học sinh nhận bằng của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSoHocSinhNhanBangByTruong(string idTruong, string idNamThi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + idTruong);
            string rawKey = string.Concat("TrangChu_GetSoHocSinhNhanBangByTruong-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetSoHocSinhNhanBangByTruong(idTruong, idNamThi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }


        /// <summary>
        /// Lấy số học sinh đã nhận của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetHocSinhDaNhanBangByTruong(string idTruong, string idNamThi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + idTruong);
            string rawKey = string.Concat("TrangChu_GetHocSinhDaNhanBangByTruong-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetHocSinhDaNhanBangByTruong(idTruong, idNamThi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }


        /// <summary>
        /// Lấy số học sinh chưa nhận của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetHocSinhChuaNhanBangByTruong(string idTruong, string idNamThi)
        {
            string objectKey = EHashMd5.FromObject(idNamThi + idTruong);
            string rawKey = string.Concat("TrangChu_GetHocSinhChuaNhanBangByTruong-", objectKey);

            //// See if the item is in the cache
            string result = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (result != null) return result;
            //// Item not found in cache - retrieve it and insert it into the cache
            result = _BL.GetHocSinhChuaNhanBangByTruong(idTruong, idNamThi);
            _cache.AddCacheItem(rawKey, result, _masterCacheKey);
            return result;
        }
        #endregion
    }
}
