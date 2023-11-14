using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum MessageTypeEnum
    {
        [Description("TRUONG_PHONG")]
        TruongGuiPhong,
        [Description("PHONG_TRUONG")]
        PhongGuiTruong,
        [Description("NGUOIDUNG_PHONG")]
        NguoiDungGuiPhong,
    }
}
