using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh
{
    public class DanhMucTotNghiepInputModel
    {
        public string? Id { get; set; }
        [CustomRequired]
        public string IdNamThi { get; set; } = null!;
        [CustomRequired]
        public string IdHinhThucDaoTao { get; set; } = null!;
        [CustomRequired]
        public string TieuDe { get; set; } = null!;
        [CustomRequired]
        public string SoQuyetDinh { get; set; } = null!;
        [CustomRequired]
        public DateTime NgayCapBang { get; set; }
        public string? GhiChu { get; set; }
        [CustomRequired]
        public string NguoiThucHien { get; set; } = null!;
        public string MaHeDaoTao { get; set; } = null!;
        public string? TenKyThi { get; set; }
    }

    public class DanhMucTotNghiepViaTruongInputModel 
    {
        public string IdDanhMucTotNghiep { get; set; } = null!;
        public string IdTruongs { get; set; } = null!;
        public string NguoiThucHien { get; set; } = null!;

    }

    public class DanhMucTotNghiepSearchParam : SearchParamModel 
    {
        public string? IdNamThi { get; set; }  = string.Empty;
        public string? IdHinhThucDaoTao { get; set; } = string.Empty;

    }
}
