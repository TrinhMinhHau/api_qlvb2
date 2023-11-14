using CenIT.DegreeManagement.CoreAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh
{
    public class HocSinhTmpModel: HocSinhModel
    {
        public string? Message { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
    }

    public class HocSinhTmpModelParamModel : SearchParamModel
    {
        public string? CCCD { get; set; } = string.Empty;
        public string? HoTen { get; set; } = string.Empty;
        public string? SoHieuVanBang { get; set; } = string.Empty;
        public string? SoVaoSoCapBang { get; set; } = string.Empty;
        public string IdTruong { get; set; } = string.Empty;
        public string IdDanhMucTotNghiep { get; set; } = string.Empty;
        public string NguoiThucHien { get; set; } = string.Empty;
    }

    public class ThongKeHocSinhTmpModel
    {
        public int? TotalRow { get; set; }
        public int? NotErrorRow { get; set; } 
        public int? ErrorRow { get; set; }
    }
}
