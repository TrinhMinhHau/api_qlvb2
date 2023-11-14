using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class FunctionActionInputModel
    {
        public int FunctionActionId { get; set; }
        public int FunctionId { get; set; }
        public string Action { get; set; }
    }
}
