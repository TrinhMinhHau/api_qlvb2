using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using Microsoft.AspNetCore.Http;


namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc
{
    public class CauHinhInputModel
    {
        public string? MaCoQuanCapBang { get; set; } = null;
        public string? LogoDonvi { get; set; } = null;
        public string? TenCoQuanCapBang { get; set; } = null;
        public string? TenDiaPhuongCapBang { get; set; } = null;
        public string? TenUyBanNhanDan { get; set; } = null;
        public string? HoTenNguoiKySoGoc { get; set; } = null;
        public string? DinhDangSoThuTuSoGoc { get; set; } = null;
        public string? DinhDangSoThuTuCapLai { get; set; } = null;
        public string? SoHieuBanSao { get; set; } = string.Empty;
        public string? NguoiKyXacMinh { get; set; } = string.Empty;
        public string? HieuTruong { get; set; } = string.Empty;
        public string? TenDiaPhuong { get; set; } = string.Empty;
        public string? NgayBanHanh { get; set; } = string.Empty;
        public IFormFile? FileImage { get; set; }
        public string? TienToBanSao { get; set; } = string.Empty;
        public int SoKyTu { get; set; }
        public int? SoBatDau { get; set; }
    }

    public class CauHinhTruongInputModel
    {
        public string? LogoDonvi { get; set; } = null;
        public string? HieuTruong { get; set; } = string.Empty;
        public string? TenDiaPhuong { get; set; } = string.Empty;
        public string? NgayBanHanh { get; set; } = string.Empty;
        public IFormFile? FileImage { get; set; }
    }

    public class CauHinhDonViQuanLyInputModel
    {
        public string? MaCoQuanCapBang { get; set; } = null;
        public string? LogoDonvi { get; set; } = null;
        public string? TenCoQuanCapBang { get; set; } = null;
        public string? TenDiaPhuongCapBang { get; set; } = null;
        public string? TenUyBanNhanDan { get; set; } = null;
        public string? HoTenNguoiKySoGoc { get; set; } = null;
        public string? TienToBanSao { get; set; } = string.Empty;
        public int SoKyTu { get; set; }
        public int? SoBatDau { get; set; }
        public IFormFile? FileImage { get; set; }
    }

    public class UpdateCauHinhSoVaoSoInputModel
    {
        public string IdTruong { get; set; } = string.Empty;
        public int DinhDangSoThuTuSoGoc { get; set; } = 0;
        public int DinhDangSoThuTuCapLai { get; set; } = 0;
        public string Nam { get; set; } = string.Empty;
        public int SoDonYeuCau { get; set; } = 0;
        public SoVaoSoEnum LoaiHanhDong { get; set; }
    }

}
