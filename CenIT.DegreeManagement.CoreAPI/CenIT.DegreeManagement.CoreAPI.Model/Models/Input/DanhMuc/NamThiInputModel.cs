using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc
{
    public class NamThiInputModel
    {
        public string? Id { get; set; }
        [CustomRequired]
        [YearValidation]
        public string? Ten { get; set; }
        [CustomRequired]
        public string? NguoiThucHien { get; set; } 
    }
}
