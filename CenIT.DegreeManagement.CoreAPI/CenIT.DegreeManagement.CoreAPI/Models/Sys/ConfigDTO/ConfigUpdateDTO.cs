using System.ComponentModel.DataAnnotations;

namespace CenIT.DegreeManagement.CoreAPI.Models.Sys.ConfigDTO
{
    public class ConfigUpdateDTO
    {
        [Required]
        public int ConfigId { get; set; } = 0;
        [Required]
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string ConfigDesc { get; set; } = string.Empty;
        [Required]
        public string LastModifiedBy { get; set; } = string.Empty;
    }
}
