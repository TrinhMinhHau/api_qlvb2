using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum XacMinhVanBangEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Học sinh không tồn tại")]
        HocSinhNotExist = -2,
    }
}
