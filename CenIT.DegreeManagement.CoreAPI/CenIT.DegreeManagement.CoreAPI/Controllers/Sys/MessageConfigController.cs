using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Sys
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageConfigController : BaseAppController
    {

        private SysMessageConfigCL _cacheLayer;
        private ILogger<MessageConfigController> _logger;
        private readonly ShareResource _localizer;

        public MessageConfigController(ICacheService cacheService, ShareResource shareResource, IConfiguration configuration, ILogger<MessageConfigController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new SysMessageConfigCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
        }

        /// <summary>
        /// Lấy cấu hình tin nhắn theo hành động
        /// API: /api/MessageConfig/GetByActionName
        /// </summary>
        ///  <param name="actionName"></param>
        /// <returns></returns>
        [HttpGet("GetByActionName")]
        public IActionResult GetByActionName(string actionName)
        {
            if (actionName == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MessageConfig.ToStringValue()));

            var data = _cacheLayer.GetByActionName(actionName);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MessageConfig.ToStringValue()));
        }

        /// <summary>
        /// Lấy danh sách cấu hình tin nhắn (search param)
        /// API: /api/MessageConfig/GetAllByParams
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByParams")]
        public IActionResult View([FromQuery] SearchParamModel model)
        {
            var data = _cacheLayer.GetAll(model);
            return ResponseHelper.Ok(data);
        }


        /// <summary>
        /// Lấy thông tin cấu hình tin nhắn theo id
        /// API: /api/MessageConfig/GetThongKeInPhoiBang
        /// </summary>
        ///  <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Detail(int? id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MessageConfig.ToStringValue()));

            var data = _cacheLayer.GetById(id);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MessageConfig.ToStringValue()));
        }

        /// <summary>
        /// Chỉnh sửa thông tin cấu hình tin nhắn theo id
        /// API: /api/MessageConfig/Update
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(MessageConfigInputModel model)
        {
            if (model.MessageConfiId == (int)MessageConfigEnum.NotUpdate)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MessageConfig.ToStringValue()));
            }

            var response = _cacheLayer.Save(model);
            if (response == (int)MessageConfigEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.MessageConfig.ToStringValue()), model.ActionName);

            }
            else if (response == (int)MessageConfigEnum.ExistMessgeCofig)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.MessageConfig.ToStringValue()), model.ActionName);
            }
            return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.MessageConfig.ToStringValue()), model.ActionName);

        }

        /// <summary>
        /// Xóa  cấu hình tin nhắn theo id
        /// API: /api/MessageConfig/Delete
        /// </summary>
        ///  <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var response = _cacheLayer.Delete(id);
            if (response == (int)MessageConfigEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.MessageConfig.ToStringValue()), id.ToString());
            }
            return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.MessageConfig.ToStringValue()), id.ToString());

        }

        /// <summary>
        /// Thêm cấu hình tin nhắn theo id
        /// API: /api/MessageConfig/Create
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody] MessageConfigInputModel model)
        {
            var response = _cacheLayer.Save(model);
            if (response == (int)MessageConfigEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.MessageConfig.ToStringValue()), model.ActionName);
            }
            else if (response == (int)MessageConfigEnum.ExistMessgeCofig)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.MessageConfig.ToStringValue()), model.ActionName);
            }
            return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.MessageConfig.ToStringValue()), model.ActionName);
        }
    }
}
