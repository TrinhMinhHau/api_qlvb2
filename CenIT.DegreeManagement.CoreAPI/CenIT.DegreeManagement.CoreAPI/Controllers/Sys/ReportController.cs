using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Sys
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ReportController : BaseAppController
    {
        private SysReportsCL _cacheLayer;
        private ILogger<ReportController> _logger;
        private readonly ShareResource _localizer;

        public ReportController(ICacheService cacheService, ShareResource shareResource, IConfiguration configuration, ILogger<ReportController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new SysReportsCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
        }

        /// <summary>
        /// Lấy danh sách báo cáo (search param)
        /// API: /api/Report/GetAllByParams
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByParams")]
        public IActionResult View([FromQuery] SearchParamModel model)
        {
            var data = _cacheLayer.GetAll(model);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy tất cả báo cáo (search param)
        /// API: /api/Report/GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            SearchParamModel model = new SearchParamModel();
            model.PageSize = -1;
            var data = _cacheLayer.GetAll(model);
            return ResponseHelper.Ok(data);
        }


        /// <summary>
        /// Thêm báo cáo 
        /// API: /api/Report/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody] ReportInputModel model)
        {

            var response = _cacheLayer.Save(model);
            if (response == (int)ReportEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.Report.ToStringValue()), model.Name);
            }
            else if (response == (int)ReportEnum.ExistReport)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Report.ToStringValue()), model.Name);
            }
            return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.Report.ToStringValue()), model.Name);

        }

        /// <summary>
        /// Chỉnh sửa báo cáo 
        /// API: /api/Report/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update([FromBody]ReportInputModel model)
        {
            if (model.ReportId == (int)ReportEnum.NotUpdate)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Report.ToStringValue()));
            }

            var response = _cacheLayer.Save(model);
            if (response == (int)ReportEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.Report.ToStringValue()), model.Name);

            }
            else if (response == (int)ReportEnum.ExistReport)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Report.ToStringValue()), model.Name);
            }
            return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.Report.ToStringValue()), model.Name);

        }

        /// <summary>
        /// Xóa báo cáo 
        /// API: /api/Report/Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var response = _cacheLayer.Delete(id);
            if (response == (int)ReportEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.Report.ToStringValue()), id.ToString());
            }
            return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.Report.ToStringValue()), id.ToString());

        }


        /// <summary>
        /// Lấy thông tin báo cáo theo id
        /// API: /api/Report/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Detail(int? id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Report.ToStringValue()));

            var data = _cacheLayer.GetByID(id);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Report.ToStringValue()));
        }

    }
}
