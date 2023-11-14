using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums.XepLoai
{
    public enum XepLoaiHanhKiem
    {
        [Description("Tốt")]
        Excellent,
        [Description("Khá")]
        Good,
        [Description("Trung Bình")]
        Average,
        [Description("Yếu")]
        Weak
    }
}
