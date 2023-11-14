using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DanhMuc
{
    [Route("api/[controller]")]
    [ApiController]
    public class CauHinhTruongController : BaseAppController
    {
        private TruongCL _cacheLayer;
        private ILogger<CauHinhTruongController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;

        private readonly string _nameController = "CauHinhTruong";

        public CauHinhTruongController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<CauHinhTruongController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new TruongCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }

        [HttpPost("UpdateDonViQuanLy")]
        public IActionResult UpdateDonViQuanLy(string idTruong, [FromForm] CauHinhDonViQuanLyInputModel model)
        {
            if (model.FileImage != null)
            {
                string folderName = "Logo";
                var fileResult = _fileService.SaveImage(model.FileImage, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.LogoDonvi = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            CauHinhInputModel cauHinh = new CauHinhInputModel()
            {
                LogoDonvi = model.LogoDonvi,
                MaCoQuanCapBang = model.MaCoQuanCapBang,
                TenCoQuanCapBang = model.TenCoQuanCapBang,
                TenDiaPhuongCapBang = model.TenDiaPhuongCapBang,
                TenUyBanNhanDan = model.TenUyBanNhanDan,
                HoTenNguoiKySoGoc = model.HoTenNguoiKySoGoc,
                SoBatDau = model.SoBatDau,
                SoKyTu = model.SoKyTu,
                TienToBanSao = model.TienToBanSao,
                FileImage = model.FileImage
            };


            var response = _cacheLayer.SaveCauHinh(idTruong, cauHinh);
            if (response == (int)CauHinhTruongEnum.Fail) return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(_nameController));
            else return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(_nameController));
        }

        [HttpPost("UpdateTruong")]
        public IActionResult UpdateTruong(string idTruong, [FromForm] CauHinhTruongInputModel model)
        {
            if (model.FileImage != null)
            {
                string folderName = "Logo";
                var fileResult = _fileService.SaveImage(model.FileImage, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.LogoDonvi = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            CauHinhInputModel cauHinh = new CauHinhInputModel()
            {
                 LogoDonvi  = model.LogoDonvi,
                 HieuTruong = model.HieuTruong,
                TenDiaPhuong = model.TenDiaPhuong,
                NgayBanHanh = model.NgayBanHanh,
                FileImage = model.FileImage
            };

            var response = _cacheLayer.SaveCauHinh(idTruong, cauHinh);
            if (response == (int)CauHinhTruongEnum.Fail) return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(_nameController));
            else return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(_nameController));
        }

        [HttpGet("GetByTruongId")]
        public IActionResult GetByTruongId(string idTruong)
        {
            if (idTruong == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            var cauHinh = _cacheLayer.GetCauHinhByIdTruong(idTruong);
            return cauHinh != null ? ResponseHelper.Ok(cauHinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
        }


        [HttpGet("GetByMaTruong")]
        public IActionResult GetByMaTruong(string idTruong)
        {
            if (idTruong == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            var cauHinh = _cacheLayer.GetCauHinhByMaTruong(idTruong);
            return cauHinh != null ? ResponseHelper.Ok(cauHinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
        }
    }
}
