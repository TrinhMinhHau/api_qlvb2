using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class NotificationModel
    {
        public int RowIndex { get; set; }
        public string IdMessage { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public string SendingMethod { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Status { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public int TotalRow { get; set; }
    }
}

