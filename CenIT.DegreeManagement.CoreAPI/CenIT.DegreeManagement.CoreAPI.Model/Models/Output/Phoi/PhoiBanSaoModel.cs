using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi
{
    public class PhoiBanSaoModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenPhoi { get; set; } = null!;
        public int SoLuongPhoi { get; set; } = 0;
        public int SoLuongPhoiDaSuDung { get; set; } = 0;
        public TinhTrangPhoiEnum TinhTrang { get; set; } = TinhTrangPhoiEnum.DangSuDung;
        public string? AnhPhoi { get; set; }
        public string MaHeDaoTao { get; set; }

        public DateTime NgayMua { get; set; }
        public List<CauHinhPhoiGocModel> CauHinhPhoi { get; set; } = new List<CauHinhPhoiGocModel>();
    }
}
