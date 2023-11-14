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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.QuanLySo
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoCapPhatBangController : BaseAppController
    {
        private SoCapPhatBangCL _cacheLayer;
        private TruongCL _truongCL;
        private DanhMucTotNghiepCL _danhMucTotNghiepCL;


        private ILogger<SoCapPhatBangController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;

        public SoCapPhatBangController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<SoCapPhatBangController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _cacheLayer = new SoCapPhatBangCL(cacheService, configuration);
            _truongCL = new TruongCL(cacheService, configuration);
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách học sinh theo  sổ cấp phát bằng
        /// API: /api/SoCapPhatBang/GetHocSinhTheoSoCapPhatBang
        /// </summary>
        /// <param name="paramModel"></param>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpGet("GetHocSinhTheoSoCapPhatBang")]
        [AllowAnonymous]
        public IActionResult GetHocSinhTheoSoCapPhatBang([FromQuery]SoCapPhatBangSearchParam modelSearch)
        {
            var truong = _truongCL.GetById(modelSearch.IdTruong);
            var dmtn = _danhMucTotNghiepCL.GetById(modelSearch.IdDanhMucTotNghiep);
            string soCapPhatBang = "";
            if (truong == null || dmtn == null)
            {
                return Ok(ResponseHelper.ResultJson(soCapPhatBang));
            }

            soCapPhatBang = _cacheLayer.GetHocSinhTheoSoCapPhatBang(truong, dmtn, modelSearch);

            return Ok(ResponseHelper.ResultJson(soCapPhatBang));
        }

    }
}
