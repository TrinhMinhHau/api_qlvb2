using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Attributes
{
    public class StrongPasswordAttribute  : ValidationAttribute
    {
        private readonly string errorMessage;
        public StrongPasswordAttribute()
        {
            this.errorMessage = "#StrongPassword";
        }

        public override bool IsValid(object value)
        {
            if (value is string password)
            {
                // Kiểm tra xem mật khẩu có đủ điều kiện không
                if (password.Length >= 8 &&
                    Regex.IsMatch(password, @"\d") &&    // Kiểm tra có số
                    Regex.IsMatch(password, "[A-Z]") && // Kiểm tra có chữ cái viết hoa
                    Regex.IsMatch(password, "[!@#$%^&*]") //// Kiểm tra có ký tự đặc biệt
                    )    
                {
                    return true; // Mật khẩu hợp lệ
                }
                else
                {
                    ErrorMessage = errorMessage;
                    return false; // Mật khẩu không hợp lệ
                }
            }

            return false;
        }
    }
}
