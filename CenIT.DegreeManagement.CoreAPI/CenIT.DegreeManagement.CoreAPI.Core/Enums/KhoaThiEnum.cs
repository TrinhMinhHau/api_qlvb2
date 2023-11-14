using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum KhoaThiEnum
    {
        [Description("Thất bại")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng ngày")]
        ExistDate = -2,
        [Description("Năm thi không tồn tại")]
        NotExistNamThi = -3,
        [Description("Danh mục tốt nghiệp không tồn tại")]
        NotExistDMTN = -4,
    }

    public enum KhoaThiInfoEnum
    {
        [Description("Ngay")]
        Date
    }
}
