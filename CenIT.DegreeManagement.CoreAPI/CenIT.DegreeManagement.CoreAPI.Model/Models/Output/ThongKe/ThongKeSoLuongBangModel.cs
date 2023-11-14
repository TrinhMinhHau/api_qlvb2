using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe
{
    public class ThongKeSoLuongBangModel
    {
        public int SoLuongDaPhat { set; get; } = 0;
        public int SoLuongChuaPhat { set; get; } = 0;
    }

    public class ThongKeHocSinhTongQuatModel
    {
        public int TongSoHocSinh { set; get; } = 0;
        public int TongSoHocSinhChoDuyet { set; get; } = 0;
        public int TongSoHocSinhDaDuyet { set; get; } = 0;
        public int TongSoHocSinhDaNhanBang { set; get; } = 0;
        public int TongSoDonYeuCauCapBanSao { set; get; } = 0;
        public int TongSoTruongDaGui { set; get; } = 0;
        public int TongSoPhoiDaIn { set; get; } = 0;
    }

    public class ThongKeSoLuongXepLoaiTheoNamModel
    {
        public string NamThi { set; get; }
        public int SoLuongHocSinhGioi { set; get; } = 0;
        public int SoLuongHocSinhKha { set; get; } = 0;
        public int SoLuongHocSinhTrungBinh { set; get; } = 0;
        public int SoLuongHocSinhYeu { set; get; } = 0;
    }

    public class ThongKeSoLuongHocSinhTheoNamModel
    {
        public string NamThi { set; get; }
        public int SoLuong { set; get; } = 0;
    }

}
