using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.AspNetCore.Http;


namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh
{
    public class HocSinhInputModel
    {
        public string? Id { get; set; } = null;
        [CustomRequired]
        public string HoTen { get; set; } = null!;
        [CustomRequired]
        public string CCCD { get; set; } = null!;
        [CustomRequired]
        public DateTime NgaySinh { get; set; } = new DateTime();
        [CustomRequired]
        public string NoiSinh { get; set; } = null!;
        [CustomRequired]
        public bool GioiTinh { get; set; } = true;
        [CustomRequired]
        public string DanToc { get; set; } = null!;
        public string IdTruong { get; set; } = null!;
        public string IdDanhMucTotNghiep { get; set; } = null!;
        public string IdKhoaThi { get; set; } = null!;
        public string? HanhKiem { get; set; } = string.Empty;
        public string? HocLuc { get; set; } = string.Empty;
        public string? GhiChu { get; set; } = string.Empty;
        public string? DienXTN { get; set; } = string.Empty;
        public double DiemXTN { get; set; } = 0;
        [CustomRequired]
        public double DiemTB { get; set; } = 0;
        public string NguoiThucHien { get; set; } = string.Empty;
        public string? MaDKThi { get; set; } = string.Empty;
        public string? TenDKThi { get; set; } = string.Empty;
        public string? KetQua { get; set; } = string.Empty;
        public string XepLoai { get; set; } = null!;
        public string? MaHD { get; set; } = string.Empty;
        public string? HoiDong { get; set; } = string.Empty;
        public string? DiaChi { get; set; } = string.Empty;
        public string? Lop { get; set; } = string.Empty;
        public List<KetQuaHocTapModel> KetQuaHocTaps { get; set; } = new List<KetQuaHocTapModel>();
    }

    public class HocSinhParamModel : SearchParamModel
    {
        public string? CCCD { get; set; } = string.Empty;
        public string? HoTen { get; set; } = string.Empty;
        public string? NoiSinh { get; set; } = string.Empty;
        public string? DanToc { get; set; } = string.Empty;
        public string? IdDanhMucTotNghiep { get; set; } = string.Empty;
        public int? TrangThai { get; set; } = null;
    }

    public class HocSinhCapPhatBangParamModel : SearchParamModel
    {
        public string? CCCD { get; set; } = string.Empty;
        public string? HoTen { get; set; } = string.Empty;
        public string? SoHieuVanBang { get; set; } = string.Empty;
        public string? SoVaoSoCapBang { get; set; } = string.Empty;
        public string? IdDanhMucTotNghiep { get; set; } = string.Empty;
        public int? TrangThai { get; set; } = null;
    }

    public class HocSinhCapBangParamModel : SearchParamModel
    {
        public string? CCCD { get; set; } = string.Empty;
        public string? HoTen { get; set; } = string.Empty;
        public string? NoiSinh { get; set; } = string.Empty;
        public string? DanToc { get; set; } = string.Empty;
        public string? IdSoGoc { get; set; } = string.Empty;
    }

    public class HocSinhImportModel 
    {
        [CustomRequired]
        public string IdTruong { get; set; } = string.Empty;
        [CustomRequired]
        public string IdKhoaThi { get; set; } = string.Empty;
        [CustomRequired]
        public string IdDanhMucTotNghiep { get; set; } = string.Empty;
        public string NguoiThucHien { get; set; } = string.Empty;
        public IFormFile FileExcel { get; set; }
    }

    public class ThongTinPhatBangInputModel
    {
        public string IdHocSinh { get; set; } = string.Empty;
        public string CccdNguoiNhanBang { get; set; } = string.Empty;
        public string MoiQuanHe { get; set; } = string.Empty;
        public string AnhCCCD { get; set; } = string.Empty;
        public string? GiayUyQuyen { get; set; }
        public string? GhiChu { get; set; }
        public string NguoiThucHien { get; set; } = string.Empty;
        public IFormFile FileAnhCCCD { get; set; }
        public IFormFile? FileUyQuyen { get; set; }
    }

    public class HocSinhSearchXacMinhVBModel : SearchParamModel
    {
        public string? CCCD { get; set; } = string.Empty;
        public string? HoTen { get; set; } = string.Empty;
        public string? NoiSinh { get; set; } = string.Empty;
        public string? DanToc { get; set; } = string.Empty;
        public string? IdDanhMucTotNghiep { get; set; } = string.Empty;
        public string? IdHinhThucDaoTao { get; set; } = string.Empty;
        public string? IdNamThi { get; set; } = string.Empty;
        public string? IdTruong { get; set; } = string.Empty;
        public string NguoiThucHien { get; set; }


    }
}
