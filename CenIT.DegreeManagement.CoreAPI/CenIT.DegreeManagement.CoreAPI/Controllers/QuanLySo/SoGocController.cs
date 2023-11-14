using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.QuanLySo
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SoGocController : BaseAppController
    {
        private SoGocCL _cacheLayer;
        private TruongCL _truongCL;
        private DanhMucTotNghiepCL _danhMucTotNghiepCL;
        private DanTocCL _danTocCL;
        private MonThiCL _monThiCL;
        private HocSinhTmpCl _hocSinhTmpCl;
        private HocSinhCL _hocSinhCache;

        private ILogger<SoGocController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;

        private readonly string _nameController = "SoGoc";
        public SoGocController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<SoGocController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _cacheLayer = new SoGocCL(cacheService, configuration); 
            _truongCL = new TruongCL(cacheService, configuration);
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
            _danTocCL = new DanTocCL(cacheService, configuration);
            _monThiCL = new MonThiCL(cacheService, configuration);
            _mapper = mapper;
            _hocSinhTmpCl = new HocSinhTmpCl(cacheService, configuration);
        }


        /// <returns></returns>
        [HttpGet("GetHocSinhTheoSoGoc")]
        [AllowAnonymous]
        public IActionResult GetHocSinhTheoSoGoc([FromQuery]SoGocSearchParam soGocSearch)
        {
            var truong = _truongCL.GetById(soGocSearch.IdTruong);
            var dmtn = _danhMucTotNghiepCL.GetById(soGocSearch.IdDanhMucTotNghiep);
            string soGoc = "";
            if(truong == null || dmtn == null)
            {
                return Ok(ResponseHelper.ResultJson(soGoc));
            }

            soGoc = _cacheLayer.GetHocSinhTheoSoGoc(truong, dmtn, soGocSearch);

            return Ok(ResponseHelper.ResultJson(soGoc));
        }

        [HttpGet("GetbyIdDanhMucTotNghiep")]
        public IActionResult GetbyIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var soGoc = _cacheLayer.GetbyIdDanhMucTotNghiep(idDanhMucTotNghiep);
            return soGoc != null ? ResponseHelper.Ok(soGoc)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
        }
    }
}
