using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Account
{
    public class TruongInfoModel
    {
        public string Id { get; set; } = string.Empty;
        public string Ma { get; set; } = "GDDT";
        public string Ten { get; set; } = "Phòng giáo dục và đào tạo";
        public string URL { get; set; } = "gddt.qlvb.gov.vn";
        public string MaHeDaoTao { get; set; } = null!;
        public string MaHinhThucDaoTao { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public bool LaPhong { get; set; } = false;
        public string Email { get; set; } = null!;
    }
}
