using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum EnumDanhMucTotNghiep
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng tiêu đề")]
        ExistName = -2,
        [Description("Trùng năm thi và hình thức đào tạo")]
        ExistYearAndHTDT = -3,
        [Description("Không tìm thấy")]
        NotFound = -4,
        [Description("Đã khóa")]
        Locked = -5,
        [Description("Đã In bằng")]
        Printed = -6,
        [Description("Không tồn tại năm thi")]
        NotExistNamThi = -7,
        [Description("Không tồn tại hình thức đào tạo")]
        NotExistHTDT = -8,
        [Description("Năm thi không khớp với ngày cấp bằng")]
        YearNotMatchDate = -9,
    }

    public enum DanhMucTotNghiepInfoEnum
    {
        [Description("Tiêu đề")]
        TieuDe,
        [Description("Ngày cấp bằng")]
        NgayCapBang,
        [Description("Id năm thi và id hình thức đào tạo")]
        IdNamVaIdHTDT
    }

    public enum TrangThaiDanhMucTotNghiepEnum
    {
        DangSuDung = 0,
        DaInBang = 1,
    }
}
