using CenIT.DegreeManagement.CoreAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search
{
    public class ThongKePhatBangSearchModel : SearchParamModel
    {
        public string? IdTruong { get; set; }
        public string? IdNamThi { get; set; } = null!;
        public string? MaHeDaoTao { get; set; } = null!;
        public string? NguoiThucHien { get; set; } = null!;

    }
}
