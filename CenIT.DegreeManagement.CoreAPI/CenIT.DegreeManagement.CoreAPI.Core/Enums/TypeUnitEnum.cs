using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum TypeUnitEnum
    {
        [Description("LOAIDONVI_TRUONG")]
        Truong = 0,
        [Description("LOAIDONVI_SO")]
        So = 1,
        [Description("LOAIDONVI_PHONG")]
        Phong = 2
    }
}
