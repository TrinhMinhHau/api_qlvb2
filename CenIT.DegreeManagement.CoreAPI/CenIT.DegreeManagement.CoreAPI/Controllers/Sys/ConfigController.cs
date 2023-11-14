using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Models.Sys.ConfigDTO;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Sys
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : BaseAppController
    {
        private SysConfigCL _cacheLayer;
        private ILogger<ConfigController> _logger;
        private readonly ShareResource _localizer;
        
        public ConfigController(ICacheService cacheService, ShareResource shareResource, IConfiguration configuration, ILogger<ConfigController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new SysConfigCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
        }


        /// <summary>
        /// Lấy danh sách cấu hình theo search param
        /// API: /api/Config/GetAllByParams
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByParams")]
        public IActionResult View([FromQuery] SearchParamModel model)
        {
            var data = _cacheLayer.GetAllConfig(model);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy tất cả cấu hình
        /// API: /api/Config/GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            SearchParamModel model = new SearchParamModel();
            model.PageSize = -1;
            var data = _cacheLayer.GetAllConfig(model);
            return ResponseHelper.Ok(data);
        }


        /// <summary>
        /// Thêm cấu hình
        /// API: /api/Config/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody] ConfigCreateDTO model)
        {
            ConfigInputModel data = ModelProvider.MapModelFromModel<ConfigCreateDTO, ConfigInputModel>(model);
            var response = _cacheLayer.Save(data);
            if (response == (int)ConfigEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.Config.ToStringValue()), model.ConfigKey);
            }
            else if (response == (int)ConfigEnum.ExistConfig)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Config.ToStringValue()), model.ConfigKey);
            }
            return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.Config.ToStringValue()), model.ConfigKey);

        }

        /// <summary>
        /// Chỉnh sửa cấu hình
        /// API: /api/Config/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update([FromBody]ConfigUpdateDTO model)
        {
            if (model.ConfigId == (int)ConfigEnum.NotUpdate)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Config.ToStringValue()));
            }

            ConfigInputModel data = ModelProvider.MapModelFromModel<ConfigUpdateDTO, ConfigInputModel>(model);
            var response = _cacheLayer.Save(data);
            if (response == (int)ConfigEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.Config.ToStringValue()), model.ConfigKey);
            }
            else if (response == (int)ConfigEnum.ExistConfig)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Config.ToStringValue()), model.ConfigKey);
            } 
            else if (response == (int)ConfigEnum.NotExist)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Config.ToStringValue()));
            }
            return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.Config.ToStringValue()), model.ConfigKey);

        }


        /// <summary>
        /// Xóa cấu hình
        /// API: /api/Config/Update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var response = _cacheLayer.Delete(id);
            if (response == (int)ConfigEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.Config.ToStringValue()));
            }
            return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.Config.ToStringValue()));

        }

        /// <summary>
        /// Lấy thông tin cấu hình theo id
        /// API: /api/Config/Update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Detail(int? id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Config.ToStringValue()));

            var data = _cacheLayer.GetConfigById(id);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Config.ToStringValue()));
        }

    }
}
