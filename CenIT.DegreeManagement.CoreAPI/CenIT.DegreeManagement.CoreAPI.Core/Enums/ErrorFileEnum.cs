using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum ErrorFileEnum
    {
        [Description("Lưu file thất bại")]
        SaveFileError = 0,
        [Description("Định dạng file ảnh không hợp lệ")]
        InvalidFileImage = -1,
    }
}
