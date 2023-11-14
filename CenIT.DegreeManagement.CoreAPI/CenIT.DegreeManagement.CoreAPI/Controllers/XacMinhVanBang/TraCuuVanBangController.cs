using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.XacMinhVanBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraCuuVanBangController : BaseAppController
    {
        private readonly ShareResource _localizer;
        private HocSinhCL _cacheLayer;
        private XacMinhVanBangCL _cacheXacMinhVB;
        private TruongCL _truongCL;
        private DanTocCL _danTocCL;
        private MonThiCL _monThiCL;
        private HocSinhTmpCl _hocSinhTmpCl;
        private SysUserCL _sysUserCL;

        private readonly IMapper _mapper;
        private ILogger<TraCuuVanBangController> _logger;
        private readonly IFileService _fileService;

        public TraCuuVanBangController(ICacheService cacheService, IMapper mapper, IConfiguration configuration, IFileService fileService, ShareResource shareResource, ILogger<TraCuuVanBangController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _cacheXacMinhVB = new XacMinhVanBangCL(cacheService, configuration);
            _truongCL = new TruongCL(cacheService, configuration);
            _danTocCL = new DanTocCL(cacheService, configuration);
            _monThiCL = new MonThiCL(cacheService, configuration);
            _mapper = mapper;
            _hocSinhTmpCl = new HocSinhTmpCl(cacheService, configuration);
            _sysUserCL = new SysUserCL(cacheService);

        }

        [HttpGet("GetSearchHocSinhXacMinhVanBang")]
        public IActionResult GetSearchHocSinhXacMinhVanBang([FromQuery] HocSinhSearchXacMinhVBModel model)
        {
            int total;
            var user = _sysUserCL.GetByUsername(model.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);

            var data = _cacheLayer.GetSearchHocSinhXacMinhVB(out total, model, donVi.Id);

            var outputData = new
            {
                HocSinhs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Lấy thông tin học sinh theo cccd
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetByCccd")]
        public IActionResult GetByCccd(string cccd)
        {
            if (cccd == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()));
            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            return hocSinh != null ? ResponseHelper.Ok(hocSinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()), cccd);
        }
    }
}
