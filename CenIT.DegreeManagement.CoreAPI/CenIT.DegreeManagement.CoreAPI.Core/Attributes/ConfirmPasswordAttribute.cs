using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Attributes
{
    public class ConfirmPasswordAttribute : ValidationAttribute
    {
        private readonly string passwordPropertyName;

        public ConfirmPasswordAttribute(string passwordPropertyName)
        {
            this.passwordPropertyName = passwordPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string confirmPassword)
            {
                // Lấy thông tin về thuộc tính mật khẩu từ context
                var propertyInfo = validationContext.ObjectType.GetProperty(passwordPropertyName);
                if (propertyInfo != null)
                {
                    // Lấy giá trị của thuộc tính mật khẩu từ context
                    var passwordValue = propertyInfo.GetValue(validationContext.ObjectInstance);

                    // Kiểm tra giá trị xác nhận mật khẩu và mật khẩu có giống nhau không
                    if (string.Equals(confirmPassword, passwordValue?.ToString()))
                    {
                        return ValidationResult.Success; // Xác nhận mật khẩu hợp lệ
                    }
                }
            }

            return new ValidationResult("#ConfirmPasswordError"); // Xác nhận mật khẩu không hợp lệ
        }
    }
}
