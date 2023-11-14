using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Models
{
    public class SearchParamFilterDateModel : SearchParamModel
    {

        /// <summary>
        ///     Filter From Date 
        /// </summary>
        public DateTime? FromDate { get; set; } = null;
        /// <summary>
        ///     Filter To Date 
        /// </summary>
        public DateTime? ToDate { get; set; } = null;
        public string? Username { get; set; } = null;
        public string? Function { get; set; } = null;
        public string? Action { get; set; } = null;
    }
}
