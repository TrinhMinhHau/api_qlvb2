using System.ComponentModel.DataAnnotations;

namespace CenIT.DegreeManagement.CoreAPI.Models.Sys.ConfigDTO
{
    public class ConfigCreateDTO
    {
        [Required]
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string ConfigDesc { get; set; } = string.Empty;
        [Required]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
