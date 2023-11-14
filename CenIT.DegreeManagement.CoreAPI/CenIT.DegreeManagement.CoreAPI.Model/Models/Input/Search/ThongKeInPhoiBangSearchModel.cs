using CenIT.DegreeManagement.CoreAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search
{
    public class ThongKeInPhoiBangSearchModel : SearchParamModel
    {
        public string IdNamThi { get; set; } = null!;
        public string? MaHeDaoTao { get; set; }
        public string NguoiThucHien { get; set; }
    }
}
