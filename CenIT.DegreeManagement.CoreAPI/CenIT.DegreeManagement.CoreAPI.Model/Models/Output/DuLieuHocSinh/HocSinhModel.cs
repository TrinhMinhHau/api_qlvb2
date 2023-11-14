using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using ExcelDataReader;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh
{
    public class HocSinhModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string HoTen { get; set; } = null!;
        public int STT { get; set; }
        public string CCCD { get; set; } = null!;
        public DateTime NgaySinh { get; set; } = new DateTime();
        public string NoiSinh { get; set; } = null!;
        public bool GioiTinh { get; set; } = true;
        public string DanToc { get; set; } = null!;
        public string HanhKiem { get; set; } = null!;
        public string HocLuc { get; set; } = null!;
        public string KetQua { get; set; } = string.Empty;
        public string XepLoai { get; set; } = null!;
        public string GhiChu { get; set; } = string.Empty;
        public string DienXTN { get; set; } = string.Empty;
        public double DiemXTN { get; set; } = 0;
        public double DiemTB { get; set; } = 0;
        public double DiemKK { get; set; } = 0;
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
        public List<KetQuaHocTapModel> KetQuaHocTaps { get; set; }
        public string? DiaChi { get; set; } = string.Empty;
        public string? Lop { get; set; } = string.Empty;
        public string? IdSoGoc { get; set; } = string.Empty;
        public string? IdPhoiGoc { get; set; } = string.Empty;
        public string? IdPhoiBanSao { get; set; } = string.Empty;
        public string? IdSoCapBanSao { get; set; } = string.Empty;
        public string? IdSoCapPhatBang { get; set; } = string.Empty;
        public string? LanDauTotNghiep { get; set; } = string.Empty;
        public int SoLanIn { get; set; } = 0;
        public ThongTinPhatBangModel ThongTinPhatBang { get; set; } = new ThongTinPhatBangModel();
    }

    public class KetQuaHocTapModel
    {
        public string MaMon { get; set; } = string.Empty;
        public double Diem { get; set; } = 0;
    }

    public class HocSinhListModel
    {
        public string Id { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; } = new DateTime();
        public string NoiSinh { get; set; } = string.Empty;
        public bool GioiTinh { get; set; } = true;
        public string DanToc { get; set; } = string.Empty;
        public string SoHieuVanBang { get; set; } = string.Empty;
        public string SoVaoSoCapBang { get; set; } = string.Empty;
        public int TrangThai { get; set; } = 0;

    }

    public class ThongTinPhatBangModel
    {
        public string CccdNguoiNhanBang { get; set; } = string.Empty;
        public string MoiQuanHe { get; set; } = string.Empty;
        public string AnhCCCD { get; set; } = string.Empty;
        public string GiayUyQuyen { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public DateTime NgayNhanBang { get; set; }
        public string NguoiThucHien { get; set; } = string.Empty;

    }

    public class VanBangBanSaoModel
    {
        public int TrangThaiDuyet { get; set; } = 1;
        public string NguoiDuyet { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public string LyDoCap { get; set; } = null!;
        public int SoBanCap { get; set; } = 1;
    }
    public class HocSinhImportViewModel : HocSinhModel
    {
        public string? SoHieuVanBang { get; set; } = string.Empty;
        public string? SoVaoSoCapBang { get; set; } = string.Empty;
        public string? LanDauTotNghiep { get; set; } = string.Empty;
        public double? DiemNV { get; set; } = 0;
        public double? DiemTO { get; set; } = 0;
        public double? DiemMon3 { get; set; } = 0;
        public double? DiemMon4 { get; set; } = 0;
        public double? DiemMon5 { get; set; } = 0;
        public double? DiemMon6 { get; set; } = 0;
        public string? MonNV { get; set; } = string.Empty;
        public string? MonTO { get; set; } = string.Empty;
        public string? Mon3 { get; set; } = string.Empty;
        public string? Mon4 { get; set; } = string.Empty;
        public string? Mon5 { get; set; } = string.Empty;
        public string? Mon6 { get; set; } = string.Empty;
    }

    public class HocSinhViewModel : HocSinhModel
    {
        public string NamThi { get; set; } = string.Empty;
        public string TenHeDaoTao { get; set; } = string.Empty;
        public SoGocModel SoGoc { get; set; } = new SoGocModel();
        public SoCapBanSaoModel SoCapBanSao { get; set; } = new SoCapBanSaoModel();
        public DanhMucTotNghiepModel DanhMucTotNghiep { get; set; } = new DanhMucTotNghiepModel();
        public TruongModel Truong { get; set; } = new TruongModel();
        public DateTime KhoaThi { get; set; }
        public string MaHinhThucDaotao { get; set; }

    }

    public class HocSinhCapPhatBangViewModel : HocSinhModel
    {
        public SoGocModel SoGoc { get; set; } = new SoGocModel();
        public string HinhThucDaoTao { get; set; } = string.Empty;
        public string HeDaoTao { get; set; } = string.Empty;
        public string Truong { get; set; } = string.Empty;
        public DateTime NgayCapBang { get; set; }
        public string NamThi { get; set; } = string.Empty;
    }

    public class HocSinhInBangModel : HocSinhModel
    {
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

    public class HocSinhXacMinhVBViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; } = new DateTime();
        public string NoiSinh { get; set; } = string.Empty;
        public bool GioiTinh { get; set; } = true;
        public string DanToc { get; set; } = string.Empty;
        public string HoiDong { get; set; } = string.Empty;
        public DateTime KhoaThi { get; set; }
    }

    public class HocSinhXMVBModel : HocSinhModel
    {
        public DanhMucTotNghiepModel DanhMucTotNghiep { get; set; }
        public NamThiModel NamThi { get; set; }
        public DateTime KhoaThi { get; set; }
    }



    public class HocSinhImportVM : HocSinhModel
    {
        public string? SoHieuVanBang { get; set; } = string.Empty;
        public string? SoVaoSoCapBang { get; set; } = string.Empty;
        public double? DiemMon1 { get; set; } = 0;
        public double? DiemMon2 { get; set; } = 0;
        public double? DiemMon3 { get; set; } = 0;
        public double? DiemMon4 { get; set; } = 0;
        public double? DiemMon5 { get; set; } = 0;
        public double? DiemMon6 { get; set; } = 0;
        public string? Mon1 { get; set; } = string.Empty;
        public string? Mon2 { get; set; } = string.Empty;
        public string? Mon3 { get; set; } = string.Empty;
        public string? Mon4 { get; set; } = string.Empty;
        public string? Mon5 { get; set; } = string.Empty;
        public string? Mon6 { get; set; } = string.Empty;


        public string? Message { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
    }

}
