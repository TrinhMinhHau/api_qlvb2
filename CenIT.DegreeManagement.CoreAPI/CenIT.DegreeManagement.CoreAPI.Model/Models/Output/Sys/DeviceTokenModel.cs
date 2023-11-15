using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys
{
    public class DeviceTokenModel
    {
        public string DeviceToken { get; set; }
    }

    public class DeviceTokenManyModel
    {
        public string DeviceToken { get; set; }
        public string TruongId { get; set; }
    }

    public class DeviceTokenGroupTruongModel
    {
        public string TruongId { get; set; }
        public List<DeviceTokenModel> DeviceTokens { get; set; }
    }
}
