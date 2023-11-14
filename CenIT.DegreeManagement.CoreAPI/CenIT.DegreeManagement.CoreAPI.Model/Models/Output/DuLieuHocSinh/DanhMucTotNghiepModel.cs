using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh
{
    public class DanhMucTotNghiepModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string IdNamThi { get; set; } = null!;
        public string IdHinhThucDaoTao { get; set; } = null!;
        public string MaHeDaoTao { get; set; } = null!;
        public string? TenKyThi { get; set; }
        public string TieuDe { get; set; } = null!;
        public string SoQuyetDinh { get; set; } = null!;
        public DateTime NgayQuyetDinh { get; set; }
        public DateTime NgayCapBang { get; set; }
        public TrangThaiDanhMucTotNghiepEnum TrangThai { get; set; } = TrangThaiDanhMucTotNghiepEnum.DangSuDung;
        public int SoLuongNguoiHoc { get; set; } = 0;
        public int TongSoTruongDaGui { get; set; } = 0;
        public int TongSoTruongDaDuyet { get; set; } = 0;
        public bool Khoa { get; set; } = false;
        public string? GhiChu { get; set; }
    }

    public class DanhMucTotNghiepViewModel : DanhMucTotNghiepModel
    {
        public int TongSoTruong { get; set; } = 0;
        public string? NamThi { get; set; } = string.Empty;
        public string? HinhThucDaoTao { get; set; } = string.Empty;
    }
}
