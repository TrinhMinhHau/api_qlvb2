using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum HinhThucDaoTaoEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng mã")]
        ExistCode = -2,
        [Description("Trùng tên")]
        ExistName = -3,
        [Description("Đã sử dụng")]
        NotFound = -4,
        [Description("Đã sử dụng")]
        Used = -5
    }

    public enum HinhThucDaoTaoInfoEnum
    {
        [Description("Ma")]
        Code,
        [Description("Ten")]
        Name
    }
}
