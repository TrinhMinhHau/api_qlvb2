using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums.XepLoai
{
    public enum XepLoaiHocLucEnum
    {
        [Description("Giỏi")]
        Excellent,
        [Description("Khá")]
        Good,
        [Description("Trung Bình")]
        Average,
        [Description("Yếu")]
        Weak,
        [Description("Kém")]
        Poor
    }
}
