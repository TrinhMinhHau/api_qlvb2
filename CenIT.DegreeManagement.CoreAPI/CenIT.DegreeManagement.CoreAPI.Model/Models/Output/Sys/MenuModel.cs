using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class MenuModel
    {
        public int MenuId { get; set; }
        public int ParentId { get; set; }
        public string NameMenu { get; set; } = string.Empty;
        public int LevelMenu { get; set; }
        public string Depth { get; set; } = string.Empty;
        public int FunctionActionId { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int Position { get; set; }
        public bool IsShow { get; set; }
        public string HienThi { get; set; } = string.Empty;

    }
}
