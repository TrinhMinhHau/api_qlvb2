using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CenIT.DegreeManagement.CoreAPI.Models.Account
{
    public class ResetPasswordDTO
    {
        [CustomRequired]
        [Email]
        public string Email { get; set; } = string.Empty;
        [CustomRequired]
        public string Token { get; set; } = string.Empty;
        [CustomRequired]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;
        [CustomRequired]
        [ConfirmPassword("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
