using CenIT.DegreeManagement.CoreAPI.Core.Enums;


namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi
{
    public class PhoiGocLogModel
    {
        public string? IdPhoiGoc { get; set; }
        public string HanhDong { get; set; } = string.Empty;
        public string LyDo { get; set; } = string.Empty;
        public string TenPhoi { get; set; } = null!;
        public string SoHieuPhoi { get; set; } = null!;
        public string SoBatDau { get; set; } = null!;
        public int SoLuongPhoi { get; set; } = 0;
        public int SoLuongPhoiDaSuDung { get; set; } = 0;
        public TinhTrangPhoiEnum TinhTrang { get; set; } = TinhTrangPhoiEnum.DangSuDung;
        public string? AnhPhoi { get; set; }
        public DateTime NgayApDung { get; set; }
        public BienBanHuyPhoiModel BienBanHuyPhoi { get; set; } = new BienBanHuyPhoiModel();
        public List<CauHinhPhoiGocModel> CauHinhPhoiGocs { get; set; } = new List<CauHinhPhoiGocModel>();
        public string NguoiThucHien { get; set; } = string.Empty;
        public DateTime NgayThucHien { get; set; } = DateTime.Now;
    }

    public class PhoiBanSaoLogModel
    {
        public string? IdPhoiGoc { get; set; }
        public string HanhDong { get; set; } = string.Empty;
        public string LyDo { get; set; } = string.Empty;
        public string TenPhoi { get; set; } = null!;
        public int SoLuongPhoi { get; set; } = 0;
        public int SoLuongPhoiDaSuDung { get; set; } = 0;
        public TinhTrangPhoiEnum TinhTrang { get; set; } = TinhTrangPhoiEnum.DangSuDung;
        public string? AnhPhoi { get; set; }
        public DateTime NgayMua { get; set; }
        public List<CauHinhPhoiGocModel> CauHinhPhoiGocs { get; set; } = new List<CauHinhPhoiGocModel>();
        public string NguoiThucHien { get; set; } = string.Empty;
        public DateTime NgayThucHien { get; set; } = DateTime.Now;
    }
}
