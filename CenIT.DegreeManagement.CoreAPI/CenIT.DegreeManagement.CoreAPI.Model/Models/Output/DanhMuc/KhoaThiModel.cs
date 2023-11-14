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
    public class KhoaThiModel
    {
        public string? Id { get; set; } // Id Khóa thi
        public string Ten { get; set; } = null!;
        public DateTime Ngay { get; set; }
        public bool Xoa { get; set; } = false;
        public DateTime? NgayTao { get; set; } = null;
        public string NguoiTao { get; set; } = string.Empty;
        public DateTime? NgayCapNhat { get; set; } = null;
        public string NguoiCapNhat { get; set; } = string.Empty;
        public DateTime? NgayXoa { get; set; } = null;
        public string NguoiXoa { get; set; } = string.Empty;
    }
}
