namespace CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh
{
    public class TruongHocViaDanhMucTotNghiepDTO
    {
        public string IdTruong { get; set; } = null!;
        public string MaTruong { get; set; } = null!;
        public string TenTruong { get; set; } = null!;
        public bool HasPermision { get; set; }
    }

    public class TruongHocViaDMTNHasPermisionDTO
    {
        public string IdTruong { get; set; } = null!;
        public string MaTruong { get; set; } = null!;
        public string TenTruong { get; set; } = null!;
    }
}
