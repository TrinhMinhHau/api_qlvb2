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
    public class FunctionController : BaseAppController
    {
        private SysFunctionCL _cacheLayer;
        private readonly ShareResource _localizer;
        private ILogger<FunctionController> _logger;

        private readonly string _nameController = "Function";

        public FunctionController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<FunctionController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new SysFunctionCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
        }


        /// <summary>
        /// Lấy danh sách chức năng (search param)
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
        /// Lấy thông tin chức năng theo id
        /// API: /api/FunctionAction/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Detail(int? id)
        {

            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Function.ToStringValue()));

            var data = _cacheLayer.GetByID(id);
            if (data != null)
            {
                return ResponseHelper.Ok(data); 
            }

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Function.ToStringValue()));
        }

        /// <summary>
        /// Thêm chức năng
        /// API: /api/FunctionAction/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IActionResult Create(FunctionInputModel model)
        {
            var response = _cacheLayer.Save(model);
            if (response == (int)FunctionEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.Function.ToStringValue()), model.Name);
            }
            else if(response == (int)FunctionEnum.ExistFunction)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Function.ToStringValue()), model.Name);
            }
            return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.Function.ToStringValue()), model.Name);

        }

        /// <summary>
        /// Chỉnh sửa chức năng
        /// API: /api/FunctionAction/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update(FunctionInputModel model)
        {
            var response = _cacheLayer.Save(model);
            if(model.FunctionId == (int)FunctionEnum.NotUpdate)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Function.ToStringValue()));
            }
            else if (response == (int)FunctionEnum.ExistFunction)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Function.ToStringValue()), model.Name);
            }

            if (response == (int)FunctionEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.Function.ToStringValue()), model.Name);
            }
            return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.Function.ToStringValue()), model.Name);

        }


        /// <summary>
        /// Xóa chức năng
        /// API: /api/FunctionAction/Update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var response = _cacheLayer.Delete(id);
            if (response == (int)FunctionEnum.Fail)
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.Function.ToStringValue()));
            return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.Function.ToStringValue()));
        }
    }
}
