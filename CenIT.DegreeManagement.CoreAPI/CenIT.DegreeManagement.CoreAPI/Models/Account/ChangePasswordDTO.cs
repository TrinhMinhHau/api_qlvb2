using CenIT.DegreeManagement.CoreAPI.Core.Attributes;

namespace CenIT.DegreeManagement.CoreAPI.Models.Account
{
    public class ChangePasswordDTO
    {
        [CustomRequired]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;
        [CustomRequired]
        [StrongPassword]
        public string NewPassword { get; set; } = string.Empty;
        [StrongPassword]
        [ConfirmPassword("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
