using CenIT.DegreeManagement.CoreAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang
{
    public class CapLaiVanBangModel : BaseModel
    {
        public string? Id { get; set; }
        public string? LyDo { get; set; }
        public string PathFileVanBan { get; set; }
    }
}
