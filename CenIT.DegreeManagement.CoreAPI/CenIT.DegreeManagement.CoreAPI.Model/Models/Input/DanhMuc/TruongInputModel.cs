using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc
{
    public class TruongInputModel
    {
        public string? Id { get; set; }
        public string Ma { get; set; } = null!;
        public string Ten { get; set; } = null!;
        public string NguoiThucHien { get; set; } = null!;
        public string? MaHeDaoTao { get; set; }
        public string? MaHinhThucDaoTao { get; set; }
        public string? Email { get; set; }
        public bool LaPhong { get; set; } = false;
        public int DonViQuanLy { get; set; } = 0;
        public string? URL { get; set; }
        public string? DiaChi { get; set; }
        public string? IdCha { get; set; }
    }
}
