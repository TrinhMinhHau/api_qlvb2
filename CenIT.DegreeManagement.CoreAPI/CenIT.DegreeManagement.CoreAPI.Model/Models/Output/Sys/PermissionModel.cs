using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class PermissionModel
    {
        public int RowIndex { get; set; }
        public int FunctionId { get; set; }
        public string FunctionName { get; set; } = string.Empty;
        public int ActionId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public bool HasPermission { get; set; }
        public int TotalRow { get; set; } = 0;
    }
}
