using CenIT.DegreeManagement.CoreAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search
{
    public class HSByTruongNamSearchModel : SearchParamModel
    {
        public string IdTruong { get; set; } = null!;
        public string IdNamThi { get; set; } = null!;
    }

    public class HocSinhTotNghiepSearchModel : SearchParamModel
    {
        public string IdNamThi { get; set; } = null!;
        public string? IdKhoaThi { get; set; }
        public string? MaHeDaoTao { get; set; }
        public string? MaHinhThucDaoTao { get; set; }
        public string? NguoiThucHien { get; set; } = null!;



    }

    public class HSByTruongNamOrDMTNSearchModel : SearchParamModel
    {
        public string IdTruong { get; set; } = null!;
        public string IdNamThi { get; set; } = null!;
        public string? IdDanhMucTotNghiep { get; set; }

    }
}
