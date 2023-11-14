using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum TinTucEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng tiêu đề")]
        ExistName = -2,
        [Description("Không tìm thấy")]
        NotFound = -3,
        [Description("Loại tin tức không tồn tại")]
        NotExistLoaiTin = -4,
    }

    public enum TinTucInfoEnum
    {
        [Description("TieuDe")]
        Title
    }
}
