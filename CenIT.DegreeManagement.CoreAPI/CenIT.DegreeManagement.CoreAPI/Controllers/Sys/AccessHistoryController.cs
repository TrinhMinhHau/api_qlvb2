using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Sys
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessHistoryController : BaseAppController
    {
        private SysAccessHistoryCL _cacheLayer;
        private ILogger<AccessHistoryController> _logger;
        private readonly ShareResource _localizer;

        public AccessHistoryController(ICacheService cacheService, IConfiguration configuration, ILogger<AccessHistoryController> logger, ShareResource shareResource) : base(cacheService, configuration)
        {
            _cacheLayer = new SysAccessHistoryCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
        }

        /// <summary>
        /// Lấy danh sách lịch sử truy cập theo search param
        /// API: /api/AccessHistory/GetAllByParams
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByParams")]
        public IActionResult View([FromQuery] SearchParamFilterDateModel model)
        {
            var data = _cacheLayer.GetAllAccessHistory(model);

            return ResponseHelper.Ok(data);
        }


        [HttpPost]
        [Route("ExportAccessHistory")]
        public IActionResult ExportAccessHistory([FromBody] AccessHistorySearchModel model)
        {
            var data = _cacheLayer.GetAllAccessHistoryByUsernameOrDate(model);

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy danh sách người dùng đã từng truy cập
        /// API: /api/AccessHistory/GetAllUserInAccessHistory
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllUserInAccessHistory")]
        public IActionResult GetAllUserInAccessHistory()
        {
            var data = _cacheLayer.GetAllUserInAccessHistory();

            return ResponseHelper.Ok(data);
        }
    }
}
