using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang
{
    public class CapLaiVangBangInputModel
    {
        public string IdHocSinh { get; set; }
        public string? PathFileVanBan { get; set; }
        public IFormFile FileVanBan { get; set; }
        public string NguoiThucHien { get; set; }
        public string LyDo { get; set; }
    }
}
