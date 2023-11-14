using System.ComponentModel.DataAnnotations;

namespace CenIT.DegreeManagement.CoreAPI.Models.Sys.UserDTO
{
    public class UserUpdateProfileDTO
    {
        [Required]
        public int UserId { get; set; } = 0;
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        public bool? Gender { get; set; } = null;
        public DateTime? Birthday { get; set; } = null;
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? Cccd { get; set; } = null;
        public string? Avatar { get; set; } = string.Empty;
        public IFormFile? FileImage { get; set; }
    }

    public class TestModel
    {
        public string? Avatar { get; set; } = string.Empty;
        public IFormFile? FileImage { get; set; }
    }
}
