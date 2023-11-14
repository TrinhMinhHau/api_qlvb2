using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CenIT.DegreeManagement.CoreAPI.Attributes
{
    public class PasswordFormatAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            string password = value.ToString();
            // Kiểm tra mật khẩu có ít nhất 8 ký tự và chứa ít nhất một chữ số và một ký tự viết hoa
            return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\d).{8,}$");
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} không đúng định dạng. Mật khẩu phải có ít nhất 8 ký tự và chứa ít nhất một chữ số và một ký tự viết hoa.";
        }
    }
}
