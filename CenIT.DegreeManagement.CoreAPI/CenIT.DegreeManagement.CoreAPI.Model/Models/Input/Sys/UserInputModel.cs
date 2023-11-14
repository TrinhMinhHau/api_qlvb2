using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys
{
    public class UserInputModel
    {
        public int UserId { get; set; } = 0;
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public bool? Gender { get; set; } = null;
        public DateTime? Birthday { get; set; } = null;
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? Cccd { get; set; } = null;
        public string? TruongId { get; set; }
        public bool IsUpdateProfile { get; set; } = false;
        public string? Avatar { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public IFormFile? FileImage { get; set; }

    }
}
