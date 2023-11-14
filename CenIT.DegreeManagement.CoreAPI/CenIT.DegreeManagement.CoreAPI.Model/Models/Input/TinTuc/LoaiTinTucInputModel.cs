using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.TinTuc
{
    public class LoaiTinTucInputModel
    {
        public string? Id { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string? GhiChu { get; set; } = string.Empty;
        public string NguoiThucHien { get; set; } = string.Empty;

    }
}
