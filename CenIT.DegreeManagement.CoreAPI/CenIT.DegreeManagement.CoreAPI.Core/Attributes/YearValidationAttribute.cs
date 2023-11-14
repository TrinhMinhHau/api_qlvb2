using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Attributes
{
    public class YearValidationAttribute : ValidationAttribute
    {
        private readonly string errorMessage;
        public YearValidationAttribute()
        {
            this.errorMessage = "#StrongYear";
        }

        public override bool IsValid(object value)
        {
            if (value is string yearString)
            {
                if (int.TryParse(yearString, out int year))
                {
                    // Kiểm tra xem năm có trong khoảng hợp lệ hay không (ví dụ: 1900 - 2099)
                    if (year >= 1900 && year <= 2099)
                    {
                        return true; // Năm hợp lệ
                    }
                }
            }

            ErrorMessage = errorMessage;
            return false; // Năm không hợp lệ
        }
    }
}
