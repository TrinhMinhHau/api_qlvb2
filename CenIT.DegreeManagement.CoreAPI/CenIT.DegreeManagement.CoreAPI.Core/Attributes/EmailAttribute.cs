using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Attributes
{
    public class EmailAttribute : ValidationAttribute
    {
        private readonly string errorMessage;
        public EmailAttribute()
        {
            this.errorMessage = "#EmailInValid";
        }

        public override bool IsValid(object value)
        {
            if (value is string email)
            {
                // Kiểm tra định dạng email bằng Regular Expression
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (Regex.IsMatch(email, emailPattern))
                {
                    return true; // Email hợp lệ
                }
                else
                {
                    ErrorMessage = errorMessage;
                    return false; // Email không hợp lệ
                }
            }

            return false;
        }
    }
}
