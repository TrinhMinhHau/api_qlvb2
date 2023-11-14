using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;

namespace CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh
{
    public class HocSinhInBangDTO : BaseModel
    {
        public string? Id { get; set; }
        public string HoTen { get; set; } = null!;
        public int STT { get; set; }
        public string CCCD { get; set; } = null!;
        public DateTime NgaySinh { get; set; } = new DateTime();
        public string NoiSinh { get; set; } = null!;
        public bool GioiTinh { get; set; } = true;
        public string DanToc { get; set; } = null!;
        public string KetQua { get; set; } = string.Empty;
        public string XepLoai { get; set; } = null!;
        public string? MaDKThi { get; set; } = string.Empty;
        public string? TenDKThi { get; set; } = string.Empty;
        public string? MaHD { get; set; } = string.Empty;
        public string? HoiDong { get; set; } = string.Empty;
        public TrangThaiHocSinhEnum TrangThai { get; set; }
        public string IdDanhMucTotNghiep { get; set; } = null!;
        public string IdTruong { get; set; } = null!;
        public string IdKhoaThi { get; set; } = null!;
        public string SoHieuVanBang { get; set; } = string.Empty;
        public string SoVaoSoCapBang { get; set; } = string.Empty;
        public string SoVaoSoBanSao { get; set; } = string.Empty;
        public string? DiaChi { get; set; } = string.Empty;
        public string? Lop { get; set; } = string.Empty;
        public string? IdSoGoc { get; set; } = string.Empty;
        public string? IdPhoiGoc { get; set; } = string.Empty;
        public string? IdPhoiBanSao { get; set; } = string.Empty;
        public string? IdSoCapBanSao { get; set; } = string.Empty;
        public string? IdSoCapPhatBang { get; set; } = string.Empty;
        public int SoLanIn { get; set; } = 0;
        public ThongTinPhatBangModel ThongTinPhatBang { get; set; } = new ThongTinPhatBangModel();
        public string? NguoiKyBang { get; set; } = string.Empty;
        public string? CoQuanCapBang { get; set; } = string.Empty;
        public string? DiaPhuongCapBang { get; set; } = string.Empty;
        public DateTime NgayCapBang { get; set; }
        public string? UyBanNhanDan { get; set; } = string.Empty;
        public string? HinhThucDaoTao { get; set; } = string.Empty;
        public string? NamThi { get; set; } = string.Empty;
        public string Truong { get; set; } = string.Empty;
        public int SoLuongBanSao { get; set; }
    }
}
