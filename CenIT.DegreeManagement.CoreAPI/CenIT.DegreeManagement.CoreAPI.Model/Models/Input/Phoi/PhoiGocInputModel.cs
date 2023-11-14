using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using Microsoft.AspNetCore.Http;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi
{
    public class PhoiGocInputModel
    {
        public string? Id { get; set; }
        [CustomRequired]
        public string TenPhoi { get; set; } = null!;
        [CustomRequired]
        public string SoHieuPhoi { get; set; } = null!;
        [CustomRequired]
        public string SoBatDau { get; set; } = null!;
        [CustomRequired]
        public int SoLuongPhoi { get; set; } = 0;
        public string MaHeDaoTao { get; set; } = null!;
        public string? AnhPhoi { get; set; }
        [CustomRequired]
        public string NguoiThucHien { get; set; } = null!;
        [CustomRequired]
        public DateTime NgayApDung { get; set; }
        public IFormFile? FileImage { get; set; }
    }

    /// <summary>
    /// Model huy phoi
    /// </summary>
    public class HuyPhoiGocInputModel
    {
        public string Id { get; set; } = null!; // id phoi cần thêm/cập nhật Phôi
        [CustomRequired]
        public IFormFile? FileBienBan { get; set; } = null;
        [CustomRequired]
        public string NguoiHuy { get; set; } = string.Empty;
        public string? FileBienBanHuyPhoi { get; set; }
        [CustomRequired]
        public string? LyDoHuy { get; set; }

    }

    public class CauHinhPhoiGocInputModel
    {
        [CustomRequired]
        public string Id { get; set; } = string.Empty;
        [CustomRequired]
        public string? MaTruongDuLieu { get; set; } = string.Empty;
        [CustomRequired]
        public string? KieuChu { get; set; } = string.Empty;
        [CustomRequired]
        public string? CoChu { get; set; }
        [CustomRequired]
        public string? DinhDangKieuChu { get; set; } = string.Empty;
        [CustomRequired]
        public string? MauChu { get; set; } = string.Empty;
        [CustomRequired]
        public int? ViTriTren { get; set; }
        [CustomRequired]
        public int? ViTriTrai { get; set; }
    }

}