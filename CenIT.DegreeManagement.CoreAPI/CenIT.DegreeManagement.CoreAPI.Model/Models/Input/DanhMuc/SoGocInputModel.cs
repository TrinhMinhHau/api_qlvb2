using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc
{
    public class SoGocInputModel
    {
        [CustomRequired]
        public string IdTruong { get; set; } = null!; 
        [CustomRequired]
        public string IdDanhMucTotNghiep { get; set; } = null!;
        [CustomRequired]
        public string NguoiKyBang { get; set; } = null!;
        [CustomRequired]
        public string? CoQuanCapBang { get; set; }
        [CustomRequired]
        public string? DiaPhuongCapBang { get; set; }
        [CustomRequired]
        public string? UyBanNhanDan { get; set; }
        [CustomRequired]
        public string NguoiThucHien { get; set; } = string.Empty;

    }
}
