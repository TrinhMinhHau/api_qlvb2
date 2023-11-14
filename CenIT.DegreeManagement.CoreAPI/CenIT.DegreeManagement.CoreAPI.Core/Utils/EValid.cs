using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace CenIT.DegreeManagement.CoreAPI.Core.Utils
{
    public static class EValid
    {
        /// <summary>
        /// Kiểm tra Email
        /// </summary>
        /// <param name="emailString"></param>
        /// <returns></returns>
        public static bool IsEmail(string emailString)
        {
            return Regex.IsMatch(emailString, @"^[a-z][a-z0-9_\.]{5,32}@[a-z0-9]{2,}(\.[a-z0-9]{2,4}){1,2}$");
        }

        /// <summary>
        /// Độ mạnh của mật khẩu
        /// </summary>
        /// <param name="passString"></param>
        /// <returns></returns>
        public static bool IsPowerPass(string passString)
        {
            return Regex.IsMatch(passString, @"^(?=(.*\d){1})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{8,}$");
        }

        /// <summary>
        /// Xác thực địa chỉ IPv4
        /// </summary>
        /// <param name="IPv4String"></param>
        /// <returns></returns>
        public static bool IPv4(string IPv4String)
        {
            return Regex.IsMatch(IPv4String, @"/\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b/");
        }

        /// <summary>
        /// Xác thực địa chỉ IPv6
        /// </summary>
        /// <param name="IPv6String"></param>
        /// <returns></returns>
        public static bool IPv6(string IPv6String)
        {
            return Regex.IsMatch(IPv6String, @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))");
        }

        /// <summary>
        /// Kiểm tra tên miền hợp lệ
        /// </summary>
        /// <param name="domainString"></param>
        /// <returns></returns>
        public static bool IsValidDomain(string domainString)
        {
            return Regex.IsMatch(domainString, @"/https?:\/\/(?:[-\w]+\.)?([-\w]+)\.\w+(?:\.\w+)?\/?.*/i");
        }

        /// <summary>
        /// Xác thực số điện thoại
        /// </summary>
        /// <param name="domainString"></param>
        /// <returns></returns>
        public static bool IsValidNumberPhone(string numberString)
        {
            return Regex.IsMatch(numberString, @"^\+?\d{1,3}?[- .]?\(?(?:\d{2,3})\)?[- .]?\d\d\d[- .]?\d\d\d\d$");
        }

        /// <summary>
        /// Số thẻ tín dụng
        /// </summary>
        /// <param name="domainString"></param>
        /// <returns></returns>
        public static bool IsValidNumberBank(string numberString)
        {
            return Regex.IsMatch(numberString, @"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$");
        }
        /// <summary>
        /// Số tài khoản yếu
        /// </summary>
        /// <param name="textPassword"></param>
        /// <returns></returns>
        public static bool IsValidNamePassword(string textPassword)
        {
            List<string> TextPass = new List<string>()
            {
"Kha@1234","12345678","Nhatrang","password","Robot","jennife","mustang","superman","fuckyou","hockey","george","sunshine","joshua","cowboy","princess","golfer","chelsea","121212","austin","summer","sexy","computer","amanda","starwars","monkey","bigdog"
            };

            if (TextPass.Contains(textPassword))
                return true;
            return false;
        }

        public static bool IsHTML(this string input)
        {
            try
            {
                XElement x = XElement.Parse("<wrapper>" + input + "</wrapper>");
                return !(x.DescendantNodes().Count() == 1 && x.DescendantNodes().First().NodeType == XmlNodeType.Text);
            }
            catch (XmlException)
            {
                return true;
            }
        }
    }
}
