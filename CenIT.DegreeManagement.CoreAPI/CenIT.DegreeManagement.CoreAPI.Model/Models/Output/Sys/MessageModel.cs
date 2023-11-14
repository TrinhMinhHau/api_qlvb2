using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class MessageModel
    {
        public string IdMessage { get; set; }
        public string Action { get; set; }
        public string MessageType { get; set; }
        public string SendingMethod { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Color { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public string Recipient { get; set; }
        public string URL { get; set; }
        public string ValueRedirect { get; set; }
        public DateTime Time { get; set; }
    }
}
