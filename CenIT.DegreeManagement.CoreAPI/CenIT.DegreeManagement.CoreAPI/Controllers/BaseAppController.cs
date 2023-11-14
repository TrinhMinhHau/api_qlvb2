using CenIT.DegreeManagement.CoreAPI.Attributes;
using CenIT.DegreeManagement.CoreAPI.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers
{
    [CustomAuthenticateAttribute]
    public class BaseAppController : ControllerBase
    {
        private AppCL appCL;
        public readonly IConfiguration _configuration;
        public string connectDB = "";

        public BaseAppController(ICacheService cacheService, IConfiguration configuration)
        {
            //Khoi tao cache
            appCL = new AppCL(cacheService);
            _configuration = configuration;
            connectDB = _configuration["ConnectionStrings:qlvanbang"]!;

            appCL.saveConfiguration(connectDB);
        }
    }
}
