using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang
{

    public class XacMinhVanBangBaseModel
    {
        public string? NguoiKyBang { get; set; }
        public string? CoQuanCapBang { get; set; }
        public string? DiaPhuongCapBang { get; set; }
        public string? UyBanNhanDan { get; set; }
        public string DonViYeuCauXacMinh { get; set; } = null!;
        public string CongVanSo { get; set; } = null!;
        public DateTime NgayTrenCongVan { get; set; }
        public string? PathFileYeuCau { get; set; }
    }

    public class XacMinhVanBangModel : XacMinhVanBangBaseModel
    {
        public string? IdHocSinh { get; set; }
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public string NoiSinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public DateTime KhoaThi { get; set; }
        public string HoiDong { get; set; }
    }

    public class XacMinhVanBangListModel : XacMinhVanBangBaseModel
    {
       public List<HocSinhXacMinhVBViewModel> HocSinhs { get; set; }
    }

    public class CauHinhXacMinhVanBangModel
    {
        public string? NguoiKyBang { get; set; }
        public string? CoQuanCapBang { get; set; }
        public string? DiaPhuongCapBang { get; set; }
        public string? UyBanNhanDan { get; set; }
    }

}
