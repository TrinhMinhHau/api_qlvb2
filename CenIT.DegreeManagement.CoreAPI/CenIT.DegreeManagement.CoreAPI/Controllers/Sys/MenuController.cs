using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Account;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Sys
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : BaseAppController
    {
        private SysMenuCL _cacheLayer;
        private ILogger<MenuController> _logger;
        private readonly ShareResource _localizer;

        public MenuController(ICacheService cacheService, IConfiguration configuration, ILogger<MenuController> logger, ShareResource shareResource) : base(cacheService, configuration)
        {
            _cacheLayer = new SysMenuCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
        }


        /// <summary>
        /// Thêm menu
        /// API: /api/Menu/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody] MenuInputModel model)
        {
            var response = _cacheLayer.SaveMenu(model);
            if (response == (int)MenuEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.Menu.ToStringValue()), model.NameMenu);
            }
            else if (response == (int)MenuEnum.ExistMenu)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(_localizer.GetAddErrorMessage(NameControllerEnum.Menu.ToStringValue()), model.NameMenu));
            }
            return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.Menu.ToStringValue()), model.NameMenu);
        }

        /// <summary>
        /// Lấy tất cả menu
        /// API: /api/Menu/GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public IActionResult View()
        {
            var data = _cacheLayer.GetAllMenu();
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy thông tin menu theo id
        /// API: /api/Menu/GetById/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Detail(int? id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Menu.ToStringValue()));

            var data = _cacheLayer.GetByID(id);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Menu.ToStringValue()));
        }

        /// <summary>
        /// Chỉnh sửa menu
        /// API: /api/Menu/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update([FromBody]MenuInputModel model)
        {
            if (model.MenuId == (int)MenuEnum.NotUpdate)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Menu.ToStringValue()));
            }

            var response = _cacheLayer.SaveMenu(model);
            if (response == (int)MenuEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.Menu.ToStringValue()), model.NameMenu);
            }
            else if (response == (int)MenuEnum.ExistMenu)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Menu.ToStringValue()), model.NameMenu);
            } else if (response == (int)MenuEnum.NotExist)
            {
                return ResponseHelper.BadRequest(_localizer.GetNotExistMessage(NameControllerEnum.Menu.ToStringValue()));
            }
            return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.Menu.ToStringValue()), model.NameMenu);

        }

        /// <summary>
        /// Xóa menu
        /// API: /api/Menu/Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var response = _cacheLayer.DeleteMenu(id);
            if (response == (int)MenuEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.Menu.ToStringValue()), id.ToString());
            }
            return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.Menu.ToStringValue()), id.ToString());
        }


        /// <summary>
        /// Láy danh sách menu theo username
        /// API: /api/Menu/{username}
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("{username}")]
        public IActionResult GetMenuByUsername([CustomRequired]string username)
        {
            var userLoginInfo = HttpContext.Items["UserLoginInfo"] as UserLoginInfoModel;
            if (userLoginInfo == null)
                return ResponseHelper.Unauthorized(_localizer.GetInvalidTokenMessage());

            var data = _cacheLayer.GetMenuByUsername(username, userLoginInfo.Token);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Menu.ToStringValue()));
        }
    }
}
