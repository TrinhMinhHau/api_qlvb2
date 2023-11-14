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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DanhMuc
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanTocController : BaseAppController
    {
        private DanTocCL _cacheLayer;
        private ILogger<DanTocController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;
        public DanTocController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<DanTocController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new DanTocCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] DanTocInputModel model)
        {
            var response = await _cacheLayer.Create(model);
            if (response == (int)DanTocEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.DanToc.ToStringValue()), model.Ten);
            if (response == (int)DanTocEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.DanToc.ToStringValue(), DanTocInfoEnum.Name.ToStringValue()), model.Ten);
            else
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.DanToc.ToStringValue()), model.Ten);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] DanTocInputModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanToc.ToStringValue()));

            var response = await _cacheLayer.Modify(model);

            if (response == (int)DanTocEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.DanToc.ToStringValue()), model.Ten);
            if (response == (int)DanTocEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.DanToc.ToStringValue(), DanTocInfoEnum.Name.ToStringValue()), model.Ten);
            if (response == (int)DanTocEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanToc.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.DanToc.ToStringValue()), model.Ten);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id, string nguoiThucHien)
        {
            var response = await _cacheLayer.Delete(id, nguoiThucHien);
            if (response == (int)DanTocEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.DanToc.ToStringValue()));
            if (response == (int)DanTocEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanToc.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.DanToc.ToStringValue()));
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
                DanTocs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetById")]
        public IActionResult GetById(string id)
        {
            var data = _cacheLayer.GetById(id);
            return data != null ? ResponseHelper.Ok(data) : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanToc.ToStringValue()));
        }
    }
}
