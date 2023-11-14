using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum MonThiEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng mã")]
        ExistCode = -2,
        [Description("Trùng tên")]
        ExistName = -3,
        [Description("Không tồn tại")]
        NotFound = -4,
    }

    public enum MonThiInfoEnum
    {
        [Description("Mã")]
        Code,
        [Description("Tên")]
        Name
    }
}
