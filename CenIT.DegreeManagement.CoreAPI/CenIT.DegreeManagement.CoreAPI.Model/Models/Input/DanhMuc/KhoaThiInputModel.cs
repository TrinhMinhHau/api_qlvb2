using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc
{
    public class KhoaThiInputModel
    {
        public string? Id { get; set; }
        [CustomRequired]
        public string? Ten { get; set; } = null!;
        [CustomRequired]
        public string? NguoiThucHien { get; set; } = null!;
        [CustomRequired]
        public DateTime Ngay { get; set; }
    }
}
