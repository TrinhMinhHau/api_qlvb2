using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DanhMuc
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeDaoTaoController : BaseAppController
    {
        private HeDaoTaoCL _cacheLayer;
        private ILogger<HeDaoTaoController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;

        public HeDaoTaoController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<HeDaoTaoController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new HeDaoTaoCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }


        /// <summary>
        /// Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] HeDaoTaoInputModel model)
        {
            var response = await _cacheLayer.Create(model);
            if (response == (int)HeDaoTaoEnum.Fail) 
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.HeDaoTao.ToStringValue()), model.Ten);
            if (response == (int)HeDaoTaoEnum.ExistCode) 
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.HeDaoTao.ToStringValue(), HeDaoTaoInfoEnum.Code.ToStringValue()), model.Ma);
            if (response == (int)HeDaoTaoEnum.ExistName) 
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.HeDaoTao.ToStringValue(), HeDaoTaoInfoEnum.Name.ToStringValue()), model.Ten);
            else 
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.HeDaoTao.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] HeDaoTaoInputModel model)
        {
            if(string.IsNullOrEmpty(model.Id))
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HeDaoTao.ToStringValue()));

            var response = await _cacheLayer.Modify(model);

            if (response == (int)HeDaoTaoEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.HeDaoTao.ToStringValue()), model.Ten);
            if (response == (int)HeDaoTaoEnum.ExistCode)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.HeDaoTao.ToStringValue(), HeDaoTaoInfoEnum.Code.ToStringValue()), model.Ma);
            if (response == (int)HeDaoTaoEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.HeDaoTao.ToStringValue(), HeDaoTaoInfoEnum.Name.ToStringValue()), model.Ten);
            if (response == (int)HeDaoTaoEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HeDaoTao.ToStringValue()));
            else 
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.HeDaoTao.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Delete chỉ cần truyên ID và Người thực hiện là đc 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id, string nguoiThucHien)
        {
            var response = await _cacheLayer.Delete(id, nguoiThucHien);
            if (response == (int)HeDaoTaoEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.HeDaoTao.ToStringValue()));
            if (response == (int)HeDaoTaoEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HeDaoTao.ToStringValue()));
            else 
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.HeDaoTao.ToStringValue()));
        }

        /// <summary>
        /// Lay tat ca he dao tao
        /// </summary>
        /// <returns></returns>
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
                HeDaoTaos = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetById")]
        public IActionResult GetById(string id)
        {
            var data = _cacheLayer.GetById(id);
            return data != null ? ResponseHelper.Ok(data) : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HeDaoTao.ToStringValue()));
        }
    }
}
