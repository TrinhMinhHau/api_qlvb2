using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Attributes
{
    public class CustomPhoneAttribute : ValidationAttribute
    {
        private readonly string errorMessage;
        public CustomPhoneAttribute()
        {
            this.errorMessage = "#PhoneInValid";
        }

        public override bool IsValid(object value)
        {
            if (value is string phoneNumber)
            {
                // Kiểm tra định dạng số điện thoại bằng Regular Expression
                string phonePattern = @"^\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})$";
                if (Regex.IsMatch(phoneNumber, phonePattern))
                {
                    return true; // Số điện thoại hợp lệ
                }
                else
                {
                    ErrorMessage = errorMessage;
                    return false; // Số điện thoại không hợp lệ
                }
            }

            return false;
        }
    }
}
