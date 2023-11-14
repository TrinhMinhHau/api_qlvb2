using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.QuanLySo
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhuLucController : BaseAppController
    {
        private ChinhSuaVanBangCL _cacheLayer;

        private ILogger<PhuLucController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;

        public PhuLucController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<PhuLucController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _cacheLayer = new ChinhSuaVanBangCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
        }

        [HttpGet("GetSerachPhuLuc")]
        [AllowAnonymous]
        public IActionResult GetSerachPhuLuc([FromQuery]PhuLucSoGocSearchModel searchModel)
        {
            int total;

            var phuLucs = _cacheLayer.GetSerachPhuLuc(out total, searchModel);

            var outputData = new
            {
                totalRow = total,
                PhuLuc = phuLucs,
                searchParam = searchModel
            };
            return ResponseHelper.Ok(outputData);
        }
    }
}
