using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.XacMinhVanBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapLaiVanBangController : BaseAppController
    {
        private readonly ShareResource _localizer;
        private HocSinhCL _cacheLayer;
        private ChinhSuaVanBangCL _cacheChinhSuaVanBang;

        private readonly IMapper _mapper;
        private ILogger<CapLaiVanBangController> _logger;
        private readonly IFileService _fileService;
        public CapLaiVanBangController(ICacheService cacheService, IMapper mapper, IConfiguration configuration, IFileService fileService, ShareResource shareResource, ILogger<CapLaiVanBangController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _cacheChinhSuaVanBang = new ChinhSuaVanBangCL(cacheService, configuration);
            _mapper = mapper;
        }


        [HttpGet("GetSearchLichSuCapLaiVanBang")]
        [AllowAnonymous]
        public IActionResult GetSearchLichSuChinhSuaVanBang(string cccd, [FromQuery] SearchParamModel model)
        {
            int total;

            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            HocSinhDTO hocSinhMp = _mapper.Map<HocSinhDTO>(hocSinh);
            var data = _cacheChinhSuaVanBang.GetSearchLichSuCapLaiVanBang(out total, hocSinhMp.Id, model);

            var outputData = new
            {
                LichSus = data,
                totalRow = total,
                HocSinh = hocSinhMp,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }


    }
}
