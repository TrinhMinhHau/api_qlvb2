using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum LoaiTinTucEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng tiêu đề")]
        ExistName = -2,
        [Description("Không tìm thấy")]
        NotFound = -4,
    }

    public enum LoaiTinTucInfoEnum
    {
        [Description("TieuDe")]
        Title
    }
}
