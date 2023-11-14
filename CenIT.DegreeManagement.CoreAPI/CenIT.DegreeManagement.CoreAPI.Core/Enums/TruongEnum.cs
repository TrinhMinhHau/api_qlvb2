using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum TruongEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng Mã")]
        ExistCode = -2,
        [Description("Trùng Tên")]
        ExistName = -3,
        [Description("Trùng url")]
        ExistURL = -4,
        [Description("Không tồn tại")]
        NotFound = -5,
        [Description("Không tồn tại hình thức đào tạo")]
        NotExistHTDT = -6,
        [Description("Không tồn tại hệ đào tạo")]
        NotExistHDT = -7,
        [Description("Đã có 1 sở giáo dục và đào tạo")]
        ExistSo = -8

    }

    public enum TruongInfoEnum
    {
        [Description("Ten")]
        Name,
        [Description("Trùng ")]
        DateApprove,
        [Description("Ma")]
        Code,
        [Description("Url")]
        Url,

    }
}
