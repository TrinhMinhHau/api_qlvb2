using System.ComponentModel.DataAnnotations;


namespace CenIT.DegreeManagement.CoreAPI.Core.Attributes
{
    public class CustomRequiredAttribute : ValidationAttribute
    {
        private readonly string errorMessage;
        public CustomRequiredAttribute()
        {
            this.errorMessage = "#Required";
        }

        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                ErrorMessage = errorMessage;
                return false;
            }

            return true;
        }

    }
}
