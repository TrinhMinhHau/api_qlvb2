using CenIT.DegreeManagement.CoreAPI.Attributes;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DanhMuc
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoaThiController : BaseAppController
    {
        private NamThiCL _cacheLayer;
        private ILogger<KhoaThiController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;

        public KhoaThiController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<KhoaThiController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new NamThiCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }

        /// <summary>
        /// Thêm khóa thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateKhoaThi(string idNamThi, [FromForm] KhoaThiInputModel model)
        {
            var response = await _cacheLayer.CreateKhoaThi(idNamThi, model);
            if (response == (int)KhoaThiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.KhoaThi.ToStringValue()), model.Ten);
            if (response == (int)KhoaThiEnum.ExistDate)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(KhoaThiInfoEnum.Date.ToStringValue() + NameControllerEnum.KhoaThi.ToStringValue()), model.Ngay.ToString("dd/MM/yyyy"));
            if (response == (int)KhoaThiEnum.NotExistNamThi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.NamThi.ToStringValue()));
            else 
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.KhoaThi.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Cập nhật khóa thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public IActionResult UpdateKhoaThi(string idNamThi, [FromForm] KhoaThiInputModel model)
        {
           var response = _cacheLayer.ModifyKhoaThi(idNamThi, model);
            if (response == (int)KhoaThiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.KhoaThi.ToStringValue()), model.Ten);
            if (response == (int)KhoaThiEnum.ExistDate)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(KhoaThiInfoEnum.Date.ToStringValue() + NameControllerEnum.KhoaThi.ToStringValue()), model.Ngay.ToString("dd/MM/yyyy"));
            else return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.KhoaThi.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Xóa khóa thi
        /// </summary>
        /// <param name="IdNamThi"></param>
        /// <param name="IdKhoaThi"></param>
        /// <param name="UserAction"></param>
        /// 
        /// <returns></returns>
        [HttpDelete("Delete")]
        public IActionResult DeleteKhoaThi(string IdNamThi, string IdKhoaThi, string UserAction)
        {
            var response = _cacheLayer.DeleteKhoaThi(IdNamThi, IdKhoaThi, UserAction);
            return response > 0 ? ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.KhoaThi.ToStringValue()))
                : ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.KhoaThi.ToStringValue()));
        }

        /// <summary>
        /// Lấy tất cả khỏa thi
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAllKhoaThi();
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy khóa thi theo IdNamThi
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <returns></returns>
        [HttpGet("GetAllByNamThiId")]
        public IActionResult GetAllByNamThiId(string idNamThi)
        {
            var data = _cacheLayer.GetKhoaThisByNam(idNamThi);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy khóa thi theo idDanhMucTotNghiep
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpGet("GetAllByIdDanhMucTotNghiep")]
        public IActionResult GetAllByIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var data = _cacheLayer.GetKhoaThiByIdDanhMucTotNghiep(idDanhMucTotNghiep);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy khóa thi theo idKhoaThi và idNamthi
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idKhoaThi"></param>
        /// <returns></returns>
        [HttpGet("GetById")]
        public IActionResult GetById(string idNamThi, string idKhoaThi)
        {
            if (idKhoaThi == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.KhoaThi.ToStringValue()));
            var khoaThi = _cacheLayer.GetKhoaThiById(idNamThi, idKhoaThi);
            return khoaThi != null ? ResponseHelper.Ok(khoaThi)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.KhoaThi.ToStringValue()), idKhoaThi);
        }

        /// <summary>
        /// Lấy danh sách khóa thi theo SearchParamModel
        /// </summary>
        /// <param name="model"></param>
        /// <param name="IdNamThi"></param>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        public IActionResult GetSearch([FromQuery] SearchParamModel model, string IdNamThi)
        {
            int total;
            var data = _cacheLayer.GetSearchKhoaThiByNamThiId(out total, model, IdNamThi);
            var outputData = new
            {
                KhoaThis = data,
                totalRow = total,
                searchParam = model,
                IdNamThi,
            };
            return ResponseHelper.Ok(outputData);
        }
    }
}
