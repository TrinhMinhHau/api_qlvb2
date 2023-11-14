using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.TinTuc
{
    public class TinTucInputModel
    {
        public string? Id { get; set; }
        public string TieuDe { get; set; }
        public string MoTaNgan { get; set; }
        public string NoiDung { get; set; }
        public string IdLoaiTinTuc { get; set; }
        public string? HinhAnh { get; set; }
        public IFormFile? FileImage { get; set; }
        public string NguoiThucHien { get; set; }
    }
}
