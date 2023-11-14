using CenIT.DegreeManagement.CoreAPI.Core.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc
{
    public class HinhThucDaoTaoModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Ma { get; set; } = null!;
        public string Ten { get; set; } = null!;
    }
}
