using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Utils
{
    public static class CheckString
    {
        public static bool CheckBsonId(string str)
        {
            bool check = ObjectId.TryParse(str, out _);
            return check;
        }
    }
}
