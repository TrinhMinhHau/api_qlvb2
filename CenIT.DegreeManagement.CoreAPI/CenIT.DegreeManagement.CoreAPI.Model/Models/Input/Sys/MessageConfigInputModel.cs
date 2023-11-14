using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class MessageConfigInputModel
    {
        public int MessageConfiId { get; set; } = 0;
        public string ActionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string UserAction { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

    }
}
