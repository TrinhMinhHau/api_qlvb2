using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;

namespace CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh
{

    public class ThongTinSoGocViewModel
    {
        public string NamTotNghiep { get; set; } = null!;
        public DateTime NgayCapBang { get; set; }
        public string NguoiKyBang { get; set; } = null!;
        public string? CoQuanCapBang { get; set; }
        public string? DiaPhuongCapBang { get; set; }
        public string? UyBanNhanDan { get; set; }
        public string? TenTruong { get; set; }

    }

    public class ThongTinDanhSachHocSinhInVanBangViewModel : ThongTinSoGocViewModel
    {
        public string? HinhThucDaoTao { get; set; }
        public List<HocSinhModel>? HocSinhs { get; set; } = new List<HocSinhModel>();
    }

    public class ThongTinHocSinhInVanBangViewModel : ThongTinSoGocViewModel
    {
        public string? HinhThucDaoTao { get; set; }
        public HocSinhModel HocSinh { get; set; } = new HocSinhModel();
    }



}
