using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Helpers
{
    public class HoTenHelper
    {
        public static string GopHoTen(string ho, string ten)
        {
            return ho + " " + ten;
        }

        public static void TachHoTen(string hoTen, out string ho, out string ten)
        {
            int index = hoTen.LastIndexOf(' ');
            if (index > 0)
            {
                ho = hoTen.Substring(0, index).Trim();
                ten = hoTen.Substring(index + 1).Trim();
            }
            else
            {
                ho = hoTen;
                ten = string.Empty;
            }
        }
    }
}
