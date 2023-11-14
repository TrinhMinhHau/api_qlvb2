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
    public class RoleController : BaseAppController
    {
        private SysRoleCL _cacheLayer;
        private SysPermissionCL _cachePermission;
        private ILogger<RoleController> _logger;
        private readonly ShareResource _localizer;

        public RoleController(ICacheService cacheService,ShareResource shareResource, IConfiguration configuration, ILogger<RoleController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new SysRoleCL(cacheService);
            _cachePermission = new SysPermissionCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
        }

        /// <summary>
        /// Lấy danh sách nhóm quyền (search param)
        /// API: /api/Role/GetAllByParams
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
        /// Lấy danh sách quyền theo roleid
        /// API: /api/Role/GetPermissionByRoleID/{id}
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPermissionByRoleID/{id}")]
        public IActionResult GetPermissionById(int id,[FromQuery] SearchParamModel model)
        {
            var data = _cachePermission.GetPermissionByRoleID(id,model);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy thông tin nhóm quyền thei id
        /// API: /api/Role/GetPermissionByRoleID/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpGet("{id}")]
        public IActionResult Detail(int? id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Role.ToStringValue()));

            var data = _cacheLayer.GetByID(id);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Role.ToStringValue()));
        }


        /// <summary>
        /// Thêm nhóm quyền
        /// API: /api/Role/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody]RoleInputModel model)
        {

            var response = _cacheLayer.Save(model);
            if (response == (int)RoleEnum.Fail) 
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.Role.ToStringValue()), model.Name);
            }
            else if(response == (int)RoleEnum.ExistRole)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Role.ToStringValue()), model.Name);
            }
            return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.Role.ToStringValue()), model.Name);

        }

        /// <summary>
        /// Thêm quyền
        /// API: /api/Role/CreatePermission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreatePermission")]
        public IActionResult CreatePermission([FromBody] PermissionInputModel model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var response = _cachePermission.Save(model);
            if (response == (int)RoleEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.Role.ToStringValue()));
            }
            else if (response == (int)RoleEnum.ExistRole)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Role.ToStringValue()));
            }
            return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.Role.ToStringValue()));

        }

        /// <summary>
        /// Chỉnh sửa nhóm quyền
        /// API: /api/Role/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(RoleInputModel model)
        {
            if (model.RoleId == (int)RoleEnum.NotUpdate)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Role.ToStringValue()));
            }

            var response = _cacheLayer.Save(model);
            if (response == (int)RoleEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.Role.ToStringValue()), model.Name);

            }
            else if (response == (int)RoleEnum.ExistRole)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Role.ToStringValue()), model.Name);
            }
            return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.Role.ToStringValue()), model.Name);

        }

        /// <summary>
        /// Xóa nhóm quyền
        /// API: /api/Role/Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var response = _cacheLayer.Delete(id);
            if (response == (int)RoleEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.Role.ToStringValue()), id.ToString());
            }
            return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.Role.ToStringValue()), id.ToString());

        }
    }
}
