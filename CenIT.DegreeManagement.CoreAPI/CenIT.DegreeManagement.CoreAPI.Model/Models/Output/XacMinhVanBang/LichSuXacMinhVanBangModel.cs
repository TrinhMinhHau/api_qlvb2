using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang
{
    public class LichSuXacMinhVanBangModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public List<string> IdHocSinhs { get; set; }
        public string DonViYeuCauXacMinh { get; set; } = null!;
        public string CongVanSo { get; set; } = null!;
        public DateTime NgayTrenCongVan { get; set; }
        public string? PathFileYeuCau { get; set; }
        public string? NguoiKyBang { get; set; }
        public string? CoQuanCapBang { get; set; }
        public string? DiaPhuongCapBang { get; set; }
        public string? UyBanNhanDan { get; set; }
    }

    public class LichSuXacMinhVanBangViewModel : LichSuXacMinhVanBangModel
    {
        public HocSinhModel? HocSinh { get; set; }
    }

    public class LichSuXacMinhVanBangSearchModel : SearchParamModel 
    {
        public string IdHocSinh { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

    }


}
