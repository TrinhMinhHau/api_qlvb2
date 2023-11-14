using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Account
{
    public class ResponseLogin
    {
        public int UserId { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string TruongID { get; set; } = string.Empty;
        public DateTime ExpiredTime { get; set; }

    }
}
