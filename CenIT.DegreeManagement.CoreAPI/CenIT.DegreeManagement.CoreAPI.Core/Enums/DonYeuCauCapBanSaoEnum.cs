using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum DonYeuCauCapBanSaoEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Thông tin học sin sai")]
        InforStudentWrong = -2,
        [Description("Phôi không tồn tại")]
        NotExistPhoi = -3,
        [Description("Số lượng học sinh vượt quá số lượng phôi gốc")]
        ExceedsPhoiGocLimit = -4,
        [Description("Đơn yêu cầu không tồn tại")]
        NotFound = -5,
        [Description("Học sinh không tồn tại")]
        NotExistHocSinh = -6,
        [Description("Họ tên không chính xác")]
        FullNameIncorrect = -7,
        [Description("Dân tộc không chính xác")]
        NationIncorrect = -8,
        [Description("Gioi tính không chính xác")]
        GenderIncorrect = -9,
        [Description("Xếp loại không chính xác")]
        ClassificationIncorrect = -10,
        [Description("Nơi sinh không chính xác")]
        PlaceIncorrect = -11,
        [Description("Danh sách rõng")]
        ListEmpty = -12,
        [Description("Ngày sinh không chính xác")]
        BirthDayIncorrect = -13,
    }
}
