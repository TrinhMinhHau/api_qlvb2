using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class ConfigModel
    {
        public int RowIndex { get; set; }
        public int ConfigId { get; set; } = 0;
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string ConfigDesc { get; set; } = string.Empty;
        public int TotalRow { get; set; }
    }
}
