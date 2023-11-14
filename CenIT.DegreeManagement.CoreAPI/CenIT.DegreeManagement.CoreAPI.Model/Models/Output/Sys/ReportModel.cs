using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class ReportModel
    {
        public int RowIndex { get; set; }
        public int ReportId { get; set; } = 0;
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsDeleted { get; set; }
        public int TotalRow { get; set; }
    }
}
