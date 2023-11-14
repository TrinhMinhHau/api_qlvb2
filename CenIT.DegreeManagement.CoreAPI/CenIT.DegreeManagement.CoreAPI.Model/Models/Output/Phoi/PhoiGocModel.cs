using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Core.Attributes;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi
{
    public class PhoiGocModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenPhoi { get; set; } = null!;
        public string SoHieuPhoi { get; set; } = null!;
        public string SoBatDau { get; set; } = null!;
        public int SoLuongPhoi { get; set; } = 0;
        public int SoLuongPhoiDaSuDung { get; set; } = 0;
        public TinhTrangPhoiEnum TinhTrang { get; set; } = TinhTrangPhoiEnum.DangSuDung;
        public string? AnhPhoi { get; set; }
        public string MaHeDaoTao { get; set; }
        public DateTime NgayApDung { get; set; }  
        public BienBanHuyPhoiModel BienBanHuyPhoi { get; set; } = new BienBanHuyPhoiModel();
        public List<CauHinhPhoiGocModel> CauHinhPhoiGocs { get; set; } = new List<CauHinhPhoiGocModel>();
    }

    public class CauHinhPhoiGocModel
    {
        public string? Id { get; set; }
        public string? MaTruongDuLieu { get; set; } = string.Empty;
        public string? KieuChu { get; set; } = string.Empty;
        public string? CoChu { get; set; }
        public string? MauChu { get; set; } = string.Empty;
        public string? DinhDangKieuChu { get; set; } = string.Empty;
        public int? ViTriTren { get; set; }
        public int? ViTriTrai { get; set; }
    }

    public class BienBanHuyPhoiModel
    {
        public DateTime? NgayHuy { get; set; } = null;
        public string NguoiHuy { get; set; } = string.Empty;
        public string? FileBienBanHuyPhoi { get; set; }
        public string? LyDoHuy { get; set; }
    }

}
