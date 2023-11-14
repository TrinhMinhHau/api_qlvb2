using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc
{
    public class SoGocModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdNamThi { get; set; } = null!;
        public string? NguoiKyBang { get; set; }
        public string? CoQuanCapBang { get; set; }
        public string? DiaPhuongCapBang { get; set; }
        public string? UyBanNhanDan { get; set; }
        public bool Khoa { get; set; } =  false;
    }

    public class SoGocViewModel : SoGocModel
    {
        public DanhMucTotNghiepViewModel DanhMucTotNghiep { get; set; } = new DanhMucTotNghiepViewModel();
        public TruongViewModel Truong { get; set; } = new TruongViewModel();
        public List<HocSinhModel> HocSinhs { get; set; } = new List<HocSinhModel>();
    }

    public class SoGocSearchParamModel : SearchParamModel
    {
        public string? IdDanhMucTotNghiep { get; set; }
        public string? IdNamThi { get; set; }
    }

    public class SoGocSearchParam : SearchParamModel
    {
        public string? IdDanhMucTotNghiep { get; set; }
        public string? IdKhoaThi { get; set; }
        public string? IdTruong { get; set; }
    }
}
