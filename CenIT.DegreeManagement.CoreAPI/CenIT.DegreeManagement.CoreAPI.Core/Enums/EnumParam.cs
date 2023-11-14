using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum EnumParam
    {
        [Description("@_search")]
        search,
        [Description("@_order")]
        order,
        [Description("@_order_dir")]
        order_dir ,
        [Description("@_page_index")]
        page_index,
        [Description("@_page_size")]
        page_size,
    }
}
