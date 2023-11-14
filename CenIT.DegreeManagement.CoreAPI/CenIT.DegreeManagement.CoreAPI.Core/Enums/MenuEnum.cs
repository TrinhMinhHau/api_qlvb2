using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum MenuEnum
    {
        [Description("API Không phải là cập nhật")]
        NotUpdate = 0,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Menu đã tồn tại")]
        ExistMenu = -9,
        [Description("Menu không tồn tại")]
        NotExist = -10,
    }
}
