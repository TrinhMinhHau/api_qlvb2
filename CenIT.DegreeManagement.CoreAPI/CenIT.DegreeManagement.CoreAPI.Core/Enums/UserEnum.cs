using System.ComponentModel;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum UserEnum
    {
        [Description("API Không phải là cập nhật")]
        NotUpdate = 0,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Email đã tồn tại")]
        ExistEmail = -8,
        [Description("Người dùng đã tồn tại")]
        ExistUser = -9,
        [Description("Người dùng không tồn tại")]
        NotExíst = -10,
    }

    public enum UserInfoEnum
    {
        [Description("Email")]
        Email
    }
}
