using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi
{
    public class PhoiBanSaoInputModel
    {
        public string? Id { get; set; }
        [CustomRequired]
        public string TenPhoi { get; set; } = null!;
        [CustomRequired]
        public int SoLuongPhoi { get; set; } = 0;
        public string? AnhPhoi { get; set; }
        public string MaHeDaoTao { get; set; }
        [CustomRequired]
        public string NguoiThucHien { get; set; } = null!;
        [CustomRequired]
        public DateTime NgayMua { get; set; }
        public IFormFile? FileImage { get; set; }
    }
}
