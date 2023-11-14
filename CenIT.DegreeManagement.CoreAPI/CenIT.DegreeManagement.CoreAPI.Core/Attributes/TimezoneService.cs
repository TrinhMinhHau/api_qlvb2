using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Attributes
{
    public class TimezoneService
    {
        public TimeSpan LocalOffset { get; } = TimeSpan.FromHours(7); // Đổi thành giá trị chênh lệch thời gian của bạn
    }
}
