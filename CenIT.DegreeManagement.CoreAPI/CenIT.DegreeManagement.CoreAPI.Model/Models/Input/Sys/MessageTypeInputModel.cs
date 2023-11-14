using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class MessageTypeInputModel
    {
        public int TypeId { get; set; } = 0;

        [Required]
        public string TypeName { get; set; } = string.Empty;
        public string UserAction { get; set; } = string.Empty;

    }
}
