using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe
{
    public class ThongKePhatBangModel
    {
        public string Id { get; set; }
        public string Ten { get; set; }
        public int ChuaPhat { get; set; }
        public int DaPhat { get; set; }
    }

    public class ThongKeTongQuatPhatBangModel
    {
        public int TongBangChuaPhat { get; set; }
        public int TongBangDaPhat { get; set; }
        public int TongSoLuongBang { get; set; }
    }
}
