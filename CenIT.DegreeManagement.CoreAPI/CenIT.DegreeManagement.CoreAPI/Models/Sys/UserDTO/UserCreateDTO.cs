using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CenIT.DegreeManagement.CoreAPI.Models.Sys.UserDTO
{
    public class UserCreateDTO
    {
        [CustomRequired]
        public string FullName { get; set; } = string.Empty;
        [CustomRequired] 
        public string UserName { get; set; } = string.Empty;
        [CustomRequired]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;
        [CustomRequired]
        [Email]
        public string Email { get; set; } = string.Empty;
        public bool? Gender { get; set; } = true;
        public DateTime? Birthday { get; set; } = null;
        [CustomPhone]
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? Cccd { get; set; } = null;
        public string? TruongId { get; set; }
        public string? Avatar { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public IFormFile? FileImage { get; set; }
    }
}
