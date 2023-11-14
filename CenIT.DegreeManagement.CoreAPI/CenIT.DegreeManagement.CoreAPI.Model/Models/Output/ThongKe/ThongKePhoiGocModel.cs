using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe
{
    public class ThongKePhoiGocModel
    {
        [BsonId]
        public string Id { get; set; }
        public string TenPhoi { get; set; }
        public int DaIn { get; set; }
        public int ChuaIn { get; set; }
    }

    public class ThongKeHocSinhTotNghiepTheoTruongModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int TongSoHocSinhTotNghiep { get; set; }
    }
}
