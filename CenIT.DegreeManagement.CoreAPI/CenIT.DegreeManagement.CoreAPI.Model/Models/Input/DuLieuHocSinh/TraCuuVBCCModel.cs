using CenIT.DegreeManagement.CoreAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh
{
    public class TraCuuVBCCModel :  SearchParamModel
    {
        public string IdNamThi { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public DateTime NgaySinh { get; set; }
        public string? Cccd { get; set; }
        public string? SoHieuVanBang { get; set; }
    }
    public class TraCuuDonYeuCau : SearchParamModel
    {
        public string IdNamThi { get; set; } = null!;
        public string? HoTen { get; set; } = null!;
        public DateTime NgaySinh { get; set; }
        public string? Cccd { get; set; }
        public string? Ma { get; set; }
    }
}
