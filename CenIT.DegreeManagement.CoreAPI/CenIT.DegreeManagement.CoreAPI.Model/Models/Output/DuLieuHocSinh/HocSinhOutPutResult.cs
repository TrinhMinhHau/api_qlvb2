using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh
{
    public class HocSinhOutPutResult
    {
        public int Status { get; set; }
        public string? Message { get; set; }
    }

    public class HocSinhResult
    {
        public int MaLoi { get; set; }
        public string SoBatDau { get; set; } = string.Empty;
        public int SoluongHocSinh { get; set; } = 0;
        public bool DaGui { get; set; } = false;
        public bool DaDuyet { get; set; } = false;
        public string IdPhoi { get; set; } = string.Empty;
        public string MaHeDaoTao { get; set; } = string.Empty;
        public string MaHinhThucDaoTao { get; set; } = string.Empty;
        public string IdNamThi { get; set; } = string.Empty;
        public List<HocSinhModel>? HocSinhs { get; set; }
        public string TenTruong { get; set; } = string.Empty;
        public string MaDonYeuCau { get; set; } = string.Empty;
        public string Nam { get; set; } = string.Empty;


    }
}
