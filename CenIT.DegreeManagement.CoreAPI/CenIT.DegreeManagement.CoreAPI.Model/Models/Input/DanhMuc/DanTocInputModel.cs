using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc
{
    public class DanTocInputModel
    {
        public string? Id { get; set; }
        public string Ten { get; set; } = null!;
        public string NguoiThucHien { get; set; } = null!;

    }
}
