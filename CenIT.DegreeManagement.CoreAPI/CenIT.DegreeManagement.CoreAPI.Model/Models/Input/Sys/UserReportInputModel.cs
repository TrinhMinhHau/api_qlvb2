using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class UserReportInputModel
    {
        public int UserId { get; set; } = 0;
        public string ReportIds { get; set; } = "";
    }
}
