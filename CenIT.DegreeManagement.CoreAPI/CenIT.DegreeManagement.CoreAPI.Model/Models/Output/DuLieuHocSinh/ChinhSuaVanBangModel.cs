using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh
{
    public class ChinhSuaVanBangModel : BaseModel
    {
        public string? Id { get; set; }
        public string? LyDo { get; set; }
        public string PathFileVanBan{ get; set; }
        public string? HoTenCu { get; set; }
        public string? CCCDCu { get; set; }
        public DateTime? NgaySinhCu { get; set; } = new DateTime();
        public string? NoiSinhCu { get; set; }
        public bool? GioiTinhCu { get; set; }
        public string? DanTocCu { get; set; }
    }

}
