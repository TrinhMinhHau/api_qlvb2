using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class PermissionInputModel
    {
        public int RoleId { get; set; }
        public int[] FunctionActionId { get; set; }
    }
}
