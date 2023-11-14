using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum NamThiEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng Tên")]
        ExistName = -2,
        [Description("Không tồn tại")]
        NotFound = -3
    }

    public enum NamThiInfoEnum
    {
        [Description("Ten")]
        Name
    }
}
