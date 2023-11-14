using CenIT.DegreeManagement.CoreAPI.Core.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang
{
    public class LichSuHuyBoModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? IdHocSinh { get; set; }
        public string? LyDo { get; set; }
        public string PathFileVanBan { get; set; }
    }

    public class LichSuHuyBoViewModel : LichSuHuyBoModel
    {
        public HocSinhModel? HocSinh { get; set; }
    }
}
