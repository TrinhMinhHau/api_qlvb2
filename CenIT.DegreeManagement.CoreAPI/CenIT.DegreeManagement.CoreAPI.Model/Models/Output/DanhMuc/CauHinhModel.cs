using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc
{
    public class CauHinhModel
    {
        public string LogoDonvi { get; set; } = string.Empty;
        public string MaCoQuanCapBang { get; set; } = string.Empty;
        public string TenCoQuanCapBang { get; set; } = string.Empty;
        public string TenDiaPhuongCapBang { get; set; } = string.Empty;
        public string TenUyBanNhanDan { get; set; } = string.Empty;
        public string HoTenNguoiKySoGoc { get; set; } = string.Empty;
        public int DinhDangSoThuTuSoGoc { get; set; } = 0;
        public int DinhDangSoThuTuCapLai { get; set; } = 0;
        public string Nam { get; set; } = string.Empty;
        public string NguoiKyXacMinh { get; set; } = string.Empty;

        //Trường 
        public string HieuTruong { get; set; } = string.Empty;
        public string TenDiaPhuong { get; set; } = string.Empty;
        public string NgayBanHanh { get; set; } = string.Empty;

        //public string Dia { get; set; } = string.Empty;

        //Cấu hình bản sao
        public string TienToBanSao { get; set; } = string.Empty;
        public int SoKyTu { get; set; }
        public int? SoBatDau { get; set; }
        public int SoDonYeuCau { get; set; } = 0;

    }
}
