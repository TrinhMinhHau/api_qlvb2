using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.CapBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapPhatBangController  : BaseAppController
    {
        private HocSinhCL _cacheLayer;
        private PhoiGocCL _phoiGocCL;
        private DanhMucTotNghiepCL _danhMucTotNghiepCL;

        private ILogger<CapPhatBangController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CapPhatBangController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<CapPhatBangController> logger, IMapper mapper, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _phoiGocCL = new PhoiGocCL(cacheService, configuration);
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService, configuration);

            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
            _fileService = fileService;
        }

        [HttpGet("GetSearchHocSinhCapPhatBang/{idTruong}")]
        public IActionResult GetSearchHocSinhCapPhatBang(string idTruong, [FromQuery] HocSinhCapPhatBangParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearchHocSinhCapPhatBangByTruong(out total, idTruong, model);
            var outputData = new
            {
                HocSinhs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpPost("CapPhatBang")]
        public async Task<IActionResult> CapPhatBang([FromForm] ThongTinPhatBangInputModel model)
        {
            if (model.FileAnhCCCD != null)
            {
                string folderName = "CCCD";
                var fileResult = _fileService.SaveImage(model.FileAnhCCCD, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.AnhCCCD = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            if (model.FileUyQuyen != null)
            {
                string folderName = "GiayUyQuyen";
                var fileResult = _fileService.SaveFilePDFOrWorld(model.FileUyQuyen, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.GiayUyQuyen = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            var response = await _cacheLayer.CapPhatBang(model);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest("Cấp phát bằng thất bại");
            if (response == (int)HocSinhEnum.NotExist)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("HocSinh")); 
            else
                return ResponseHelper.Success("Cấp phát bằng thành công");
        }

        [HttpGet("GetByCccd")]
        public IActionResult GetByCccd(string cccd)
        {
            if (cccd == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage("HocSinh"));
            var hocSinh = _cacheLayer.GetHocSinhPhatBangByCccd(cccd);
            return hocSinh != null ? ResponseHelper.Ok(hocSinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage("HocSinh"), cccd);
        }

    }
}
