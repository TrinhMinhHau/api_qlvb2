using CenIT.DegreeManagement.CoreAPI.Core.Attributes;

namespace CenIT.DegreeManagement.CoreAPI.Models.Account
{
    public class LoginDTO
    {
        [CustomRequired]
        public string Username { get; set; } = string.Empty;
        [CustomRequired]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;
    }
}
