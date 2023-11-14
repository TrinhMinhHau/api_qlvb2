using CenIT.DegreeManagement.CoreAPI.Core.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.TinTuc
{
    public class TinTucModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TieuDe { get; set; }
        public string MoTaNgan { get; set; }
        public string NoiDung { get; set; }
        public string IdLoaiTinTuc { get; set; }
        public string HinhAnh { get; set; }
        public string LuotXem { get; set; }
        public TrangThaiTinTucEnum TrangThai { get; set; } = TrangThaiTinTucEnum.Hien;

    }

    public class TinTucListModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TieuDe { get; set; }
        public string MoTaNgan { get; set; }
        public string IdLoaiTinTuc { get; set; }
        public string HinhAnh { get; set; }
        public string LuotXem { get; set; }
        public TrangThaiTinTucEnum TrangThai { get; set; } = TrangThaiTinTucEnum.Hien;
        public string TenLoaiTinTuc { get; set; } = string.Empty;

    }

    public class TinTucViewModel : TinTucModel
    {
        public string TenLoaiTinTuc { get; set; } = string.Empty;
    }
}
