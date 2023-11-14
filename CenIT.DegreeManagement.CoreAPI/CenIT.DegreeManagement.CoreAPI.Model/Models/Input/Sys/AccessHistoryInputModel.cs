using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class AccessHistoryInputModel 
    {
        public string UserName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Function { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
    }
}
