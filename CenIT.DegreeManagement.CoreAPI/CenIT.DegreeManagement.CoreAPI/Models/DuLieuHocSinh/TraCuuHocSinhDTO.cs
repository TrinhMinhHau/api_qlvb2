using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;

namespace CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh
{
    public class TraCuuHocSinhDTO : HocSinhModel
    {
        public string TenTruong { get; set; }
        public DateTime KhoaThi { get; set; }
        public string NamThi { get; set; }
        public string TenHinhThucDaoTao { get; set; }
        public SoGocModel SoGoc { get; set; }
        public CauHinhModel CauHinhDonViHienTai { get; set; }
        public DonYeuCauCapBanSaoModel DonYeuCauCapBanSao { get; set; }
    }
}
