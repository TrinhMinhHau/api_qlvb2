using System.ComponentModel;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum FunctionEnum
    {
        [Description("API Không phải là cập nhật")]
        NotUpdate = 0,
        [Description("Thất bại")]
        Fail = -1,
        [Description("Function đã tồn tại")]
        ExistFunction = -9,
    }
}
