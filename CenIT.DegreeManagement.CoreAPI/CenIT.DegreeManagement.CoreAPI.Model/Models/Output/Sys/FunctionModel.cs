using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class FunctionModel
    {
        public int RowIndex { get; set; }
        public int FunctionId { get; set; } = 0;
        public string Description { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int TotalRow { get; set; }
    }
}
