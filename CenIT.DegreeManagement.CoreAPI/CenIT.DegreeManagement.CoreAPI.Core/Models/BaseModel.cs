using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Models
{
    public class BaseModel
    {
        public bool Xoa { get; set; } = false;
        public DateTime NgayTao { get; set; } 
        public string NguoiTao { get; set; } = string.Empty;
        public DateTime? NgayCapNhat { get; set; } = null;
        public string NguoiCapNhat { get; set; } = string.Empty;
        public DateTime? NgayXoa { get; set; } = null;
        public string NguoiXoa { get; set; } = string.Empty;
    }
}
