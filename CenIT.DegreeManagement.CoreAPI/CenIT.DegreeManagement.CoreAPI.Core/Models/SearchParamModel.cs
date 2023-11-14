using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Models
{
    public class SearchParamModel
    {
        public string? Search { get; set; } = "";

        /// <summary>
        ///     Sort Column
        /// </summary>
        public string? Order { get; set; } = "0";

        /// <summary>
        ///     Sort Type : DESC | ASC
        /// </summary>
        public string? OrderDir { get; set; } = "ASC";

        /// <summary>
        ///     Current Row Index
        /// </summary>
        public int StartIndex { get; set; } = 0;

        /// <summary>
        ///     Page size
        /// </summary>
        public int PageSize { get; set; } = 5;

        ///// <summary>
        /////     Output: total rows
        ///// </summary>
        //public int TotalRecord { get; set; }
    }
}
