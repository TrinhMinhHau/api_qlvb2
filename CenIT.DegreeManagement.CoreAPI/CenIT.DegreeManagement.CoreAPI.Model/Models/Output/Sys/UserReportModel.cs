using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class UserReportModel
    {
        public int RowIndex { get; set; }
        public int ReportId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool HasPermission { get; set; }
        public int TotalRow { get; set; } = 0;
    }
}
