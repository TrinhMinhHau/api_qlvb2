using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc
{
    public class DonYeuCauCapBanSaoModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string IdHocSinh { get; set; } = null!;
        public string Ma { get; set; } = null!;
        public string LyDo { get; set; } = null!;
        public int SoLuongBanSao { get; set; }
        public TrangThaiDonYeuCauEnum TrangThai { get; set; } = TrangThaiDonYeuCauEnum.ChuaDuyet;
        public string IdTruong { get; set; } = null!;
        public string IdNamThi { get; set; } = null!;
        public string DonYeuCau { get; set; } = null!;
        public string LyDoTuChoi { get; set; } = null!;
        public string HinhAnhCCCD { get; set; } = null!;
        public ThongTinNguoiYeuCauModel ThongTinNguoiYeuCau { get; set; } = new ThongTinNguoiYeuCauModel();
        public DateTime? NgayIn { get; set; } = null;
        public string NguoiIn { get; set; } = string.Empty;
        public bool DaIn { get; set; } = false;
        public int PhuongThucNhan { get; set; }
        public string? DiaChiNhan { get; set; }
        public string? SoHieu { get; set; }
        public string? SoVaoSoBanSao { get; set; }
        public string? NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string? IdPhoiBanSao { get; set; } = string.Empty;
        public string? IdSoCapBanSao { get; set; } = string.Empty;

    }



    public class ThongTinNguoiYeuCauModel
    {
        public string HoTenNguoiYeuCau { get; set; } = null!;
        public string SoDienThoaiNguoiYeuCau { get; set; } = null!;
        public string DiaChiNguoiYeuCau { get; set; } = null!;
        public string EmailNguoiYeuCau { get; set; } = null!;
        public string CCCDNguoiYeuCau { get; set; } = null!;
    }

    public class DonYeuCauCapBanSaoViewModel : DonYeuCauCapBanSaoModel
    {
        public HocSinhModel HocSinh { get; set; } = new HocSinhModel();
        public TruongModel Truong { get; set; } = new TruongModel();

    }

    public class DonYeuCauCapBanSaoParamModel : SearchParamModel
    {
        public TrangThaiDonYeuCauEnum TrangThai { get; set; } = TrangThaiDonYeuCauEnum.DaDuyet;
        public string? Ma { get; set; }
        public string? HoTen { get; set; }
        public string? CCCD { get; set; }
        public string NguoiThucHien { get; set; }
    }

    public class HocSinhCapBanSaoParamModel : DonYeuCauCapBanSaoParamModel
    {
        public string? HoTen { get; set; }
        public string? CCCD { get; set; }
        public string? SoDienThoaiNguoiYeuCau { get; set; }
        public DateTime? NgayDuyet { get; set; }

    }

    public class DuyetDonYeuCauInputModel
    {
        public string IdDonYeuCauCapBanSao { get; set; } = null!;
        public string IdHocSinh { get; set; } = null!;
        public string NguoiThucHien { get; set; } = null!;
        public DateTime NgayNhan { get; set; }
        public string LePhi { get; set; }

    }

    public class DonYeuCauOutPutResult
    {
        public int MaLoi { get; set; }
        public string? HoTenNguoiYeuCau { get; set; }
        public string EmailNguoiYeuCau { get; set; } = string.Empty;
        public string? MaDon { get; set; }
        public int SoLuongBanSao { get; set; } = 0;
        public string? LePhi { get; set; }
        public string DiaChiNhan { get; set; } = string.Empty;
        public int PhuongThucNhan { get; set; } = 0;
    }

    public class HocSinhCapBanSaoViewModel : DonYeuCauCapBanSaoModel
    {
        public HocSinhModel HocSinh { get; set; } = new HocSinhModel();
    }
}
