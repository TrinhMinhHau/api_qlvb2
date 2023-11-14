using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DanhMuc
{
    [Route("api/[controller]")]
    [ApiController]
    public class NamThiController : BaseAppController
    {

        private NamThiCL _cacheLayer;
        private SysConfigCL _sysConfigCL;

        private ILogger<NamThiController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;


        public NamThiController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<NamThiController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new NamThiCL(cacheService, configuration);
            _sysConfigCL = new SysConfigCL(cacheService);

            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }

        /// <summary>
        /// Thêm năm thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] NamThiInputModel model)
        {
            var searchParam = new SearchParamModel() { PageSize = -1 };
            var configs = _sysConfigCL.GetAllConfig(searchParam);
            var soGoc = new SoGocModel();
            var soCapBanSao = new SoCapBanSaoModel();
            var soCapPhatBang = new SoCapPhatBangModel();

            AssignValues(soGoc, soCapBanSao, soCapPhatBang, configs);

            var response = await _cacheLayer.Create(model, soGoc, soCapBanSao, soCapPhatBang);
           
            if (response == (int)NamThiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.NamThi.ToStringValue()), model.Ten);
            if (response == (int)NamThiEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.NamThi.ToStringValue(), NamThiInfoEnum.Name.ToStringValue()), model.Ten);
            else 
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.NamThi.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Cập nhật năm thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] NamThiInputModel model)
        {
            var response = await _cacheLayer.Modify(model);
            if (response == (int)NamThiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.NamThi.ToStringValue()), model.Ten);
            if (response == (int)NamThiEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.NamThi.ToStringValue(), NamThiInfoEnum.Name.ToStringValue()), model.Ten);
            if (response == (int)NamThiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.NamThi.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.NamThi.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Xóa năm thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("Detete")]
        public async Task<IActionResult> Delete(string id, string nguoiThucHien)
        {
            var response = await _cacheLayer.Delete(id, nguoiThucHien);
            if (response == (int)NamThiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.NamThi.ToStringValue()));
            if (response == (int)NamThiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.NamThi.ToStringValue()));
            else
                return ResponseHelper.BadRequest(_localizer.GetDeleteSuccessMessage(NameControllerEnum.NamThi.ToStringValue()));
        }

        /// <summary>
        /// Lấy danh sách Năm Thi theo search param
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        public IActionResult GetSearch([FromQuery] SearchParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearch(out total, model);
            var outputData = new
            {
                NamThis = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Lấy tất cả năm thi
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAll();
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy năm thi theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetById")]
        public IActionResult GetById(string id)
        {
            if(id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.NamThi.ToStringValue()));

            var data = _cacheLayer.GetById(id);
            return data != null ? ResponseHelper.Ok(data)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.NamThi.ToStringValue()));
        }

        private void AssignValues(SoGocModel soGocModel, SoCapBanSaoModel soCapBanSaoModel, SoCapPhatBangModel soCapPhatBangModel,List<ConfigModel> configs)
        {
            var propertyMappingSoGoc = new Dictionary<string, Action<SoGocModel, string>>
            {
                { "SO_NGUOIKYBANG", (m, v) => m.NguoiKyBang = v },
                { "SO_DIAPHUONGCAPBANG", (m, v) => m.DiaPhuongCapBang = v },
                { "SO_COQUANCAPBANG", (m, v) => m.CoQuanCapBang = v },
                { "SO_UYBANNHANDAN", (m, v) => m.UyBanNhanDan = v },
            };

            var propertyMappingSoCapBanSao = new Dictionary<string, Action<SoCapBanSaoModel, string>>
            {
                { "SO_NGUOIKYBANG", (m, v) => m.NguoiKyBang = v },
                { "SO_DIAPHUONGCAPBANG", (m, v) => m.DiaPhuongCapBang = v },
                { "SO_COQUANCAPBANG", (m, v) => m.CoQuanCapBang = v },
                { "SO_UYBANNHANDAN", (m, v) => m.UyBanNhanDan = v },
            };

            var propertyMappingSoCapPhatBang = new Dictionary<string, Action<SoCapPhatBangModel, string>>
            {
                { "SO_NGUOIKYBANG", (m, v) => m.NguoiKyBang = v },
                { "SO_DIAPHUONGCAPBANG", (m, v) => m.DiaPhuongCapBang = v },
                { "SO_COQUANCAPBANG", (m, v) => m.CoQuanCapBang = v },
                { "SO_UYBANNHANDAN", (m, v) => m.UyBanNhanDan = v },
            };


            foreach (var c in configs)
            {
                if (propertyMappingSoGoc.TryGetValue(c.ConfigKey, out Action<SoGocModel, string> setter))
                {
                    setter(soGocModel, c.ConfigValue);
                }

                if (propertyMappingSoCapBanSao.TryGetValue(c.ConfigKey, out Action<SoCapBanSaoModel, string> set))
                {
                    set(soCapBanSaoModel, c.ConfigValue);
                }

                if (propertyMappingSoCapPhatBang.TryGetValue(c.ConfigKey, out Action<SoCapPhatBangModel, string> s))
                {
                    s(soCapPhatBangModel, c.ConfigValue);
                }
            }
        }
    }
}
