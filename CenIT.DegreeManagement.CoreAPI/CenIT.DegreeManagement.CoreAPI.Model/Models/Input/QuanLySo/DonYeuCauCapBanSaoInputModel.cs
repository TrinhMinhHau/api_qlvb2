using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.QuanLySo
{
    public class DonYeuCauCapBanSaoInputModel
    {
        [CustomRequired]
        public string HoTen { get; set; } = null!;
        [CustomRequired]

        public string IdTruong { get; set; } = null!;
        [CustomRequired]

        public string IdNamThi { get; set; } = null!;
        [CustomRequired]

        public bool GioiTinh { get; set; }
        [CustomRequired]

        public string DanToc { get; set; } = null!;
        [CustomRequired]

        public string NoiSinh { get; set; } = null!;
        [CustomRequired]

        public DateTime NgaySinh { get; set; }
        [CustomRequired]

        public string CCCD { get; set; } = null!;
        [CustomRequired]

        public string XepLoai { get; set; } = string.Empty;
        [CustomRequired]

        public string HoTenNguoiYeuCau { get; set; } = null!;
        [CustomRequired]

        public string SoDienThoaiNguoiYeuCau { get; set; } = null!;
        [CustomRequired]

        public string EmailNguoiYeuCau { get; set; } = null!;
        [CustomRequired]

        public string DiaChiNguoiYeuCau { get; set; } = null!;
        [CustomRequired]
        public string CCCDNguoiYeuCau { get; set; }
        public string? HinhAnhCCCD { get; set; }
        public string? DonYeuCau { get; set; }
        [CustomRequired]
        public IFormFile? FileDonYeuCau { get; set; }
        [CustomRequired]
        public IFormFile? FileHinhAnhCCCD { get; set; }
        [CustomRequired]
        public int SoLuongBanSao { get; set; } = 0;
        public string NguoiThucHien { get; set; } = string.Empty;
        [CustomRequired]
        public string LyDo { get; set; } = null!;
        public int PhuongThucNhan { get; set; }
        public string? DiaChiNhan { get; set; }
    }
}
