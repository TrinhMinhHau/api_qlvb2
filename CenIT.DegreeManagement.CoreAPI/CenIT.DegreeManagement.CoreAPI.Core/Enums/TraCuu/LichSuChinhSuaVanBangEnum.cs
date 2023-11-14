using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu
{
    public enum LichSuChinhSuaVanBangEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Học sinh không tồn tại")]
        NotExist = -2,
        [Description("Số Hiệu đã tồn tại")]
        SoHieuExist = -3,
        [Description("Số vào sổ cấp đã tồn tại")]
        SoVaoSoExist = -4,
        [Description("Không có thông tin chỉnh sửa")]
        NotEdit = -5,


    }

    public enum LichSuHuyBoVanBangEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Học sinh không tồn tại")]
        NotExist = -2,
    }

    public enum LichSuCapLaiVanBangEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Học sinh không tồn tại")]
        NotExist = -2,
    }
}
