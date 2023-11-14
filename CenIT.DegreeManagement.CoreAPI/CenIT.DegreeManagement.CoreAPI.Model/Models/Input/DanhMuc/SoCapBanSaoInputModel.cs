using CenIT.DegreeManagement.CoreAPI.Core.Attributes;


namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc
{
    public class SoCapBanSaoInputModel
    {
        public string? Id { get; set; }
        [CustomRequired]
        public string TenSo { get; set; } = null!;
        [CustomRequired]
        public string IdDanhMucTotNghiep { get; set; } = null!;
        [CustomRequired]
        public string IdHinhThucDaoTao { get; set; } = null!;
        [CustomRequired]
        public DateTime NgayQuyetDinhTotNghiep { get; set; }
        [CustomRequired]
        public string IdNamThi { get; set; } = null!;
        public string? GhiChu { get; set; }
        [CustomRequired]
        public string NguoiThucHien { get; set; } = null!;
        [CustomRequired]
        public string IdTruong { get; set; }
    }
}