using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class RoleInputModel
    {
        public int RoleId { get; set; } = 0;

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
