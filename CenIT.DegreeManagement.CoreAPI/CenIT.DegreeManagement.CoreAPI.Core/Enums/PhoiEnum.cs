using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum PhoiEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng tên")]
        ExistName = -2,
        [Description("Trùng áp dụng")]
        ExistDate = -3,
        [Description("Không tìm thấy")]
        NotFound = -4,
        [Description("Đã có 1 phôi đang sử dụng ")]
        ExistInUse = -5,
        [Description("Đã In bằng")]
        Printed = -6,
        [Description("Trùng tiền tố phôi")]
        ExistNumber = -7,
    }

    public enum PhoiInfoEnum
    {
        [Description("Ten")]
        Name,
        [Description("NgayApDung")]
        DateApprove,
        [Description("TienToPhoi")]
        Number
    }
}
