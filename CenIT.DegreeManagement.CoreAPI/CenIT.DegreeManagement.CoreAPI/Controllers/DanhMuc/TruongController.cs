using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
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
    public class TruongController : BaseAppController
    {
        private TruongCL _truongCl;
        private ILogger<TruongController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;
        private SysUserCL _sysUserCL;


        public TruongController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<TruongController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _truongCl = new TruongCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService; 
            _sysUserCL = new SysUserCL(cacheService);

        }

        /// <summary>
        /// Thêm trường 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] TruongInputModel model)
        {
            var response = await _truongCl.Create(model);
            if (response == (int)TruongEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.Truong.ToStringValue()), model.Ten);
            if (response == (int)TruongEnum.ExistCode)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Truong.ToStringValue(), TruongInfoEnum.Code.ToStringValue()), model.Ma);
            if (response == (int)TruongEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Truong.ToStringValue(),TruongInfoEnum.Name.ToStringValue()), model.Ten);
            if (response == (int)TruongEnum.ExistURL)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Truong.ToStringValue(), TruongInfoEnum.Url.ToStringValue()), model.URL);
            if (response == (int)TruongEnum.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HinhThucDaoTao.ToStringValue()));
            if (response == (int)TruongEnum.NotExistHDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HeDaoTao.ToStringValue()));
            if (response == (int)TruongEnum.ExistSo)
                return ResponseHelper.NotFound(_localizer.GetAlreadyExistMessage("Đã có 1 sở giáo dục và đào tạo"));
            else
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.Truong.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Cập nhật trường 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromForm] TruongInputModel model)
        {
            var response = await _truongCl.Modify(model);
            if (response == (int)TruongEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.Truong.ToStringValue()), model.Ten);
            if (response == (int)TruongEnum.ExistCode)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Truong.ToStringValue(), TruongInfoEnum.Code.ToStringValue()), model.Ma);
            if (response == (int)TruongEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Truong.ToStringValue(), TruongInfoEnum.Name.ToStringValue()), model.Ten);
            if (response == (int)TruongEnum.ExistURL)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.Truong.ToStringValue(), TruongInfoEnum.Url.ToStringValue()), model.URL);
            if (response == (int)TruongEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Truong.ToStringValue()));
            if (response == (int)TruongEnum.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HinhThucDaoTao.ToStringValue()));
            if (response == (int)TruongEnum.NotExistHDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HeDaoTao.ToStringValue()));
            if (response == (int)TruongEnum.ExistSo)
                return ResponseHelper.NotFound(_localizer.GetAlreadyExistMessage("Đã có 1 phòng giáo dục và đào tạo"));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.Truong.ToStringValue()), model.Ten);
        }

        /// <summary>
        /// Xóa trường  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string id, string nguoiThucHien)
        {
            var response = await _truongCl.Delete(id, nguoiThucHien);
            if (response == (int)TruongEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.Truong.ToStringValue()));
            if (response == (int)TruongEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Truong.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.Truong.ToStringValue()));
        }

        /// <summary>
        /// Lấy trường theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetById")]
        [AllowAnonymous]
        public IActionResult GetById(string? id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Truong.ToStringValue()));

            var data = _truongCl.GetById(id);
            return data != null ? ResponseHelper.Ok(data)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Truong.ToStringValue()));
        }

        /// <summary>
        /// Lấy tất cả các trường
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var data = _truongCl.GetAll();
            return ResponseHelper.Ok(data);
        }


        [HttpGet("GetByMaHeDaoTao")]
        [AllowAnonymous]
        public IActionResult GetByMaHeDaoTao(string maHeDaoTao)
        {
            var data = _truongCl.GetByMaHeDaoTao(maHeDaoTao);
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetAllHavePhong")]
        public IActionResult GetAllHavePhong()
        {
            var data = _truongCl.GetAllHavePhong();
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy danh sách trường theo seatch param
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        public IActionResult GetSearch(string nguoiThucHien ,[FromQuery] SearchParamModel model)
        {
            var user = _sysUserCL.GetByUsername(nguoiThucHien);
            string idDonVi = "";
            if (!string.IsNullOrEmpty(user.TruongID) && CheckString.CheckBsonId(user.TruongID))
            {
                idDonVi = _truongCl.GetById(user.TruongID).Id;
            }

            int total;
            var data = _truongCl.GetSearch(out total, model, idDonVi);
            var outputData = new
            {
                Truongs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }
    }
}
