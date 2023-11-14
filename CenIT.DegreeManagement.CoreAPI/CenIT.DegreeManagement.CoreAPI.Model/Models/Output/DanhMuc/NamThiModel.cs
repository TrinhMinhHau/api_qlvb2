using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc
{
    public class NamThiModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Ten { get; set; } = null!;
        public bool Xoa { get; set; } = false;
        public DateTime? NgayTao { get; set; } = null;
        public string NguoiTao { get; set; } = string.Empty;
        public DateTime? NgayCapNhat { get; set; } = null;
        public string NguoiCapNhat { get; set; } = string.Empty;
        public DateTime? NgayXoa { get; set; } = null;
        public string NguoiXoa { get; set; } = string.Empty;
        public List<KhoaThiModel> KhoaThis { get; set; } = new List<KhoaThiModel>();
    }

    public class NamThiViewModel : NamThiModel
    {
        public DanhMucTotNghiepModel DanhMucTotNghiep { get; set; } = new DanhMucTotNghiepModel();
    }

    //public class NamThiViaDanhMucTotNghie : NamThiModel
    //{
    //    public DanhMucTotNghiepModel DanhMucTotNghiep { get; set; } = new DanhMucTotNghiepModel();
    //}
}