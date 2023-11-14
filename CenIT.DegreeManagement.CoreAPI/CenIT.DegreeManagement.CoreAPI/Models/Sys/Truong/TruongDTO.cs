using CenIT.DegreeManagement.CoreAPI.Core.Models;

namespace CenIT.DegreeManagement.CoreAPI.Models.Sys.Truong
{
    public class TruongDTO : BaseModel
    {
        public string? Id { get; set; }
        public string Ma { get; set; } = null!;
        public string Ten { get; set; } = null!;
        public string MaHeDaoTao { get; set; } = null!;
        public string MaHinhThucDaoTao { get; set; } = null!;
        public string URL { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public bool LaPhong { get; set; } = false;
        public string Email { get; set; } = null!;
        public int DonViQuanLy { get; set; }
        public int STT { get; set; }
        public string? IdCha { get; set; }
    }
}
