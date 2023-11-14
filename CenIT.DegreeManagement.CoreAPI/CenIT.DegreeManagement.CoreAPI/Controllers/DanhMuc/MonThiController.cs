using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DanhMuc
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MonThiController : BaseAppController
    {
        private MonThiCL _cacheLayer;
        private ILogger<MonThiController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;


        public MonThiController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<MonThiController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new MonThiCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }

        /// <summary>
        /// Thêm môn thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] MonThiInputModel model)
        {
            var response = await _cacheLayer.Create(model);
            if (response == (int)MonThiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.MonThi.ToStringValue()), model.Ten);
            if (response == (int)MonThiEnum.ExistCode)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.MonThi.ToStringValue(), MonThiInfoEnum.Code.ToStringValue()), model.Ma);
            if (response == (int)MonThiEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.MonThi.ToStringValue(), MonThiInfoEnum.Name.ToStringValue()), model.Ten);
            else
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.MonThi.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Cập nhật môn thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] MonThiInputModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MonThi.ToStringValue()));

            var response = await _cacheLayer.Modify(model);

            if (response == (int)MonThiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.MonThi.ToStringValue()), model.Ten);
            if (response == (int)MonThiEnum.ExistCode)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.MonThi.ToStringValue(), EnumExtensions.ToStringValue(MonThiInfoEnum.Code)), model.Ma);
            if (response == (int)MonThiEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.MonThi.ToStringValue(), MonThiInfoEnum.Name.ToStringValue()), model.Ten);
            if (response == (int)MonThiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MonThi.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.MonThi.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Delete chỉ cần truyên ID và Người thực hiện là đc 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id, string nguoiThucHien)
        {
            var response = await _cacheLayer.Delete(id, nguoiThucHien);
            if (response == (int)MonThiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.MonThi.ToStringValue()));
            if (response == (int)MonThiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MonThi.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.MonThi.ToStringValue()));
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetSearch")]
        public IActionResult GetSearch([FromQuery] SearchParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearch(out total, model);
            var outputData = new
            {
                MonThis = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetById")]
        public IActionResult GetById(string? id)
        {
            var data = _cacheLayer.GetById(id);
            return data != null ? ResponseHelper.Ok(data)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.MonThi.ToStringValue()), id);
        }
    }
}
