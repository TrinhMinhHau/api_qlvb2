using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Sys
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseAppController
    {
        private MessageCL _cacheLayer;
        private ILogger<NotificationController> _logger;
        private readonly ShareResource _localizer;

        public NotificationController(ICacheService cacheService, ShareResource shareResource, IConfiguration configuration, ILogger<NotificationController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new MessageCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
        }

        /// <summary>
        /// Lấy danh sách tin nhắn (search param)
        /// API: /api/Notification/GetAllByParams
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByParams")]
        public IActionResult View([FromQuery] SearchParamFilterDateModel model)
        {
            var data = _cacheLayer.GetAllMessages(model);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy thông tin tin nhắn theo id
        /// API: /api/Notification/{id}
        /// </summary>
        ///  <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Detail(string id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Notification.ToStringValue()));

            var data = _cacheLayer.GetMessageById(id);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Notification.ToStringValue()));
        }


        /// <summary>
        /// Lấy thông tin tin nhắn theo id
        /// API: /api/Notification/GetAllMessagesByUserId
        /// </summary>
        ///  <param name="userId"></param>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllMessagesByUserId")]
        [AllowAnonymous]
        public IActionResult GetAllMessagesByUserId([FromQuery]int userId ,[FromQuery] SearchParamFilterDateModel model)
        {
            var data = _cacheLayer.GetAllMessagesByUserId(userId,model);
            return ResponseHelper.Ok(data);
        }
    }
}
