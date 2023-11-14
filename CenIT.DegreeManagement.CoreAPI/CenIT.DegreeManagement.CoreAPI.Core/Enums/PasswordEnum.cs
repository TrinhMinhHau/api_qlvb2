using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum PasswordEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Mật khẩu hiện tại không đúng")]
        CurrentPasswordIncorrect = -1,
        [Description("Người dùng đã tồn tại")]
        ExistUser = -9,
        [Description("Thất bại")]
        Fail = -10
    }
}
