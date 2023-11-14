using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class AccessHistoryModel
    {
        public int RowIndex { get; set; }
        public int UserId { get; set; } = 0;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime? AccessTime { get; set; } = null;
        public string Function { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public int TotalRow { get; set; } = 0;
    }
}
