using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using Microsoft.AspNetCore.Mvc;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Resources;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Sys
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionActionController : BaseAppController
    {
        private SysFunctionActionCL _cacheLayer;
        private ILogger<UserController> _logger;
        private readonly ShareResource _localizer;

        public FunctionActionController(ICacheService cacheService, ShareResource shareResource, IConfiguration configuration, ILogger<UserController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new SysFunctionActionCL(cacheService);
            _logger = logger;
            _localizer = shareResource;

        }

        /// <summary>
        /// Lấy danh sách dành động theo search param
        /// API: /api/FunctionAction/GetAllByParams
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
        /// Lấy tất cả dành động
        /// API: /api/FunctionAction/GetAllByParams
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
        /// Lấy danh sách dành động theo functionId (searchparram)
        /// API: /api/FunctionAction/GetActionsByFunctionId/{id}
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetActionsByFunctionId/{id}")]
        public IActionResult GetActionsByFunctionId(int id,[FromQuery] SearchParamModel model)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.FunctionAction.ToStringValue()));

            var data = _cacheLayer.GetActionsByFunctionId(id,model);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.FunctionAction.ToStringValue()));
        }

        /// <summary>
        /// Lấy thông tin dành động theo id 
        /// API: /api/FunctionAction/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.FunctionAction.ToStringValue()));

            var data = _cacheLayer.GetByID(id);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.FunctionAction.ToStringValue()));
        }

        /// <summary>
        /// Thêm hành động
        /// API: /api/FunctionAction/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IActionResult Create(FunctionActionInputModel model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var response = _cacheLayer.Save(model);
            if (response > 0)
            {
                return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.FunctionAction.ToStringValue()), model.Action);
            }
            else if (response == (int)FunctionActionEnum.ExistFunctionAction)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.FunctionAction.ToStringValue()), model.Action);
            }
            return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.FunctionAction.ToStringValue()), model.Action);
        }

        /// <summary>
        /// Chỉnh sửa hành động
        /// API: /api/FunctionAction/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(FunctionActionInputModel model)
        {
            if (model.FunctionActionId == (int)FunctionActionEnum.NotUpdate)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.FunctionAction.ToStringValue()));
            }

            var response = _cacheLayer.Save(model);
            if (response == (int)FunctionActionEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.FunctionAction.ToStringValue()), model.Action);
            }
            return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.FunctionAction.ToStringValue()), model.Action);

        }

        /// <summary>
        /// Xóa hành động
        /// API: /api/FunctionAction/Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var response = _cacheLayer.Delete(id);
            if (response == (int)FunctionActionEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.FunctionAction.ToStringValue()), id.ToString());
            }
            return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.FunctionAction.ToStringValue()), id.ToString());
        }
    }
}
