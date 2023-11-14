using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class FunctionActionModel
    {
        public int RowIndex { get; set; }
        public int FunctionActionId { get; set; }
        public int FunctionId { get; set; }
        public string Action { get; set; }
        public string Function { get; set; }
        public bool IsDeleted { get; set; }
        public int TotalRow { get; set; }

    }
}
