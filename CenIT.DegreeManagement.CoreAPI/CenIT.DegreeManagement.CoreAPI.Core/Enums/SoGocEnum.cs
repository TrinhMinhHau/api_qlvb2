using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum SoGocEnum
    {
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng tên")]
        ExistName = -2,
        [Description("Sổ không không tồn tại")]
        NotExist = -3,
    }

    public enum SoGocInfoEnum
    {
        [Description("Tên sổ")]
        Name,
    }
}
