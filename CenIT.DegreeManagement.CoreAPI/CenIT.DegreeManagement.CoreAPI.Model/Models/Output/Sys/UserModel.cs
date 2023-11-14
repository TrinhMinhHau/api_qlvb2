using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class UserModel
    {
        public int RowIndex { get; set; }
        public int UserId { get; set; } = 0;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public bool? Gender { get; set; } = null;
        public DateTime? Birthday { get; set; } = null;
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? Cccd { get; set; } = null;
        public string? TruongID { get; set; } = null;
        public bool IsActive { get; set; }
        public int TotalRow { get; set; } = 0;
    }
}
