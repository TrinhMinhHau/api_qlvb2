using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum HanhDongLogPhoiGocEnum
    {
        [Description("THÊM MỚI")]
        CREATE,
        [Description("CẬP NHẬT")]
        UPDATE,
        [Description("XÓA")]
        DELETE,
        [Description("CẬT NHẬT CẤU HÌNH")]
        UPDATE_CONFIG,
        [Description("THÊM CẤU HÌNH")]
        CREATE_CONFIG,
    }
}
