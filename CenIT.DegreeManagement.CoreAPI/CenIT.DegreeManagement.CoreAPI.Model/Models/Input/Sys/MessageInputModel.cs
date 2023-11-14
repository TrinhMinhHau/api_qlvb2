using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class MessageInputModel
    {
        public string IdMessage { get; set; }
        public string Action { get; set; }
        public string MessageType { get; set; }
        public string SendingMethod { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Color { get; set; }
        public string Url { get; set; }
        public string? Recipient { get; set; }
        public string? ValueRedirect { get; set; }
        public bool IsNotification { get; set; } = true;
        public string? IDDonVi { get; set; }
    }
}
