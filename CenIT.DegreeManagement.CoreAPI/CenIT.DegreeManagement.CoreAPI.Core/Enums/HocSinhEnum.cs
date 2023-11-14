using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum HocSinhEnum
    {
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Trùng Cccd")]
        ExistCccd = -2,
        [Description("Học sinh không tồn tại")]
        NotExist= -3,
        [Description("Không thể cập nhật vì danh sách học sinh rõng")]
        ListEmpty = -4,
        [Description("Không thể cập nhật vì học sinh đã duyệt")]
        HocSinhApproved = -5,
        [Description("Số lượng học sinh vượt quá số lượng phôi gốc")]
        ExceedsPhoiGocLimit = -6,
        [Description("Trường không tồn tại")]
        NotExistTruong = -7,
        [Description("Phôi không tồn tại")]
        NotExistPhoi = -8,
        [Description("Danh mục tốt nghiệp không tồn tại")]
        NotExistDanhMucTotNghiep = -9,
        [Description("Danh mục tốt nghiệp không tồn tại")]
        ExistSTT = -10,
        [Description("Đã xác nhận")]
        Confirmed = -11,
        [Description("Đã duyệt")]
        Approved = -12,
        [Description("Hình thức đào tạo không tồn tại")]
        NotExistHTDT = -13,
        [Description("Năm thi không tồn tại")]
        NotExistYear = -14,
        [Description("So gốc không tồn tại")]
        NotExistSoGoc = -15,
        [Description("Khóa thi không tồn tại")]
        NotExistKhoaThi = -16,
        [Description("Dân tộc không tồn tại")]
        NotExistDanToc = -17,
        [Description("Hình thức đào tạo của trường không khớp với danh mục tốt nghiệp")]
        NotMatchHtdt = -18,
    }

    public enum HocSinhInFoEnum
    {

        [Description("CCCD")]
        CCCD,
        [Description("Số thứ tự")]
        STT
    }

    public enum LoaiXepLoatEnum
    {
        [Description("Giỏi")]
        Gioi,
        [Description("Khá")]
        Kha,
        [Description("Trung Bình")]
        TrungBinh,
        [Description("Yếu")]
        Yeu
    }
}
