using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class MenuInputModel
    {
        public int MenuId { get; set; } = 0;
        public string NameMenu { get; set; } = string.Empty;
        public int? Position { get; set; }
        public int? ParentId { get; set; }
        public string? Link { get; set; } = null;
        public string? Icon { get; set; } = null;
        public int? FunctionActionId { get; set; }
        public bool IsShow { get; set; }

    }
}
