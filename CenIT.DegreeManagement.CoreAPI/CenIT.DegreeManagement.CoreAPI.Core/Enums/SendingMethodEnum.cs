using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Enums
{
    public enum SendingMethodEnum
    {
        [Description("NOTIFICATION")]
        Notification,
        [Description("MAIL")]
        Mail,
        [Description("SMS")]
        Sms,
    }
}
