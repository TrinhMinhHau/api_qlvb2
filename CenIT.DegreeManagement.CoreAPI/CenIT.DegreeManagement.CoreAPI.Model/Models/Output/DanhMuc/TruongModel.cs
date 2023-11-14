using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc
{
    public class TruongModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Ma { get; set; } = null!;
        public string Ten { get; set; } = null!;
        public string MaHeDaoTao { get; set; } = null!;
        public string MaHinhThucDaoTao { get; set; } = null!;
        public string URL { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public bool LaPhong { get; set; } = false;
        public string Email { get; set; } = null!;
        public int DonViQuanLy { get; set; }
        public string? IdCha { get; set; }
        public int STT { get; set; }
        public CauHinhModel CauHinh { get; set; } = new CauHinhModel();
    }

    public class TruongViewModel : TruongModel
    {
        public string HeDaoTao { get; set; }
        public string HinhThucDaoTao { get; set; }
    }
    
    public class TruongWithSoGocModel 
    {
        public string? Id { get; set; }
        public string Ma { get; set; } = null!;
        public string Ten { get; set; } = null!;
        public string? HinhThucDaoTao { get; set; } = null!;
        public SoGocModel SoGoc { get; set; } = new SoGocModel();
    }
}