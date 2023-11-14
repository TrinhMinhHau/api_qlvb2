using CenIT.DegreeManagement.CoreAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search
{
    public class TraCuuHocHinhTotNghiepSearchModel : SearchParamModel
    {
        public string? HoTen { get; set; }
        public string? CCCD { get; set; } = string.Empty;
        public string NguoiThucHien { get; set; } = string.Empty;

    }
}
