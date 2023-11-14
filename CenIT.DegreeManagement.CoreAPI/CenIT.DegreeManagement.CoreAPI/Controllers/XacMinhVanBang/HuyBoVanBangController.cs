using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
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
    public class HuyBoVanBangController : BaseAppController
    {
        private readonly ShareResource _localizer;
        private HocSinhCL _cacheLayer;
        private HuyBoVangBangCL _cacheHuyBoVanBang;
        private readonly IMapper _mapper;
        private ILogger<HuyBoVanBangController> _logger;
        private readonly IFileService _fileService;
        public HuyBoVanBangController(ICacheService cacheService, IMapper mapper, IConfiguration configuration, IFileService fileService, ShareResource shareResource, ILogger<HuyBoVanBangController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _cacheHuyBoVanBang = new HuyBoVangBangCL(cacheService, configuration);
            _mapper = mapper;
        }

        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] HuyBoVangBangInputModel model)
        {

            #region Save File

            if (model.FileVanBan != null)
            {
                string folderName = "DonYeuCau/VanBanHuyBo";
                var fileResult = _fileService.SaveFile(model.FileVanBan, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileVanBan = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            #endregion

            var data = await _cacheHuyBoVanBang.Create(model);
            if (data == (int)LichSuHuyBoVanBangEnum.Fail)
                return ResponseHelper.BadRequest("Hủy bỏ văn bằng thất bại");
            if (data == (int)LichSuHuyBoVanBangEnum.NotExist)
                return ResponseHelper.BadRequest("Không tồn tại");
            return ResponseHelper.Success("Hủy bỏ văn bằng thành công");
        }

        [HttpGet("GetSearchLichSuHuyBoVanBang")]
        [AllowAnonymous]
        public IActionResult GetSearchLichSuHuyBoVanBang(string idHocSinh, [FromQuery] SearchParamModel model)
        {
            int total;

            var data = _cacheHuyBoVanBang.GetSerachHuyBoVanBangByIdHocSinh(out total, idHocSinh, model);

            var outputData = new
            {
                LichSus = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        //[HttpGet("GetHuyBoVanBangById")]
        //[AllowAnonymous]
        //public IActionResult GetHuyBoVanBangById(string cccd, string idLichSuChinhSua)
        //{
        //    var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
        //    HocSinhDTO hocSinhMp = _mapper.Map<HocSinhDTO>(hocSinh);
        //    var data = _cacheHuyBoVanBang.GetHuyBoVanBangById(cccd, idLichSuChinhSua);

        //    var outputData = new
        //    {
        //        HocSinhs = hocSinhMp,
        //        LichSus = data,
        //    };
        //    return ResponseHelper.Ok(outputData);
        //}
    }
}
