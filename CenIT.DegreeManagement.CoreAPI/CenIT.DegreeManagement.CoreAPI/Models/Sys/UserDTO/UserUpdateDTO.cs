using System.ComponentModel.DataAnnotations;

namespace CenIT.DegreeManagement.CoreAPI.Models.Sys.UserDTO
{
    public class UserUpdateDTO
    {
        [Required]
        public int UserId { get; set; } = 0;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public bool? Gender { get; set; } = true;
        public DateTime? Birthday { get; set; } = null;
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? Cccd { get; set; } = null;
        public string? TruongId { get; set; }
        public string? Avatar { get; set; } = string.Empty;
        public IFormFile? FileImage { get; set; }
    }
}
