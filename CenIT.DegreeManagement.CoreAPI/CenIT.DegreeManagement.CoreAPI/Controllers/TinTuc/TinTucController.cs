using CenIT.DegreeManagement.CoreAPI.Caching.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.TinTuc
{
    [Route("api/[controller]")]
    [ApiController]
    public class TinTucController : BaseAppController
    {
        private TinTucCL _cacheLayer;
        private ILogger<TinTucController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;

        private string _nameController = "TinTuc";
        public TinTucController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<TinTucController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new TinTucCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }

        /// <summary>
        /// Thêm tin tức
        /// API: /api/TinTuc/Create
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] TinTucInputModel model)
        {

            if (model.FileImage != null)
            {
                string folderName = "TinTuc";
                var fileResult = _fileService.SaveImage(model.FileImage, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.HinhAnh = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }
            var response = await _cacheLayer.Create(model);
            if (response == (int)TinTucEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.TinTuc.ToStringValue()), model.TieuDe);
            if (response == (int)TinTucEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.TinTuc.ToStringValue(), TinTucInfoEnum.Title.ToStringValue()), model.TieuDe);
            if (response == (int)TinTucEnum.NotExistLoaiTin)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.LoaiTin.ToStringValue()));
            return ResponseHelper.Success(_localizer.GetAddSuccessMessage(_nameController), model.TieuDe);
        }

        /// <summary>
        /// Cập nhật tin tức
        /// API: /api/TinTuc/Update
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] TinTucInputModel model)
        {
            if (model.FileImage != null)
            {
                string folderName = "TinTuc";
                var fileResult = _fileService.SaveImage(model.FileImage, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.HinhAnh = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }
            var response = await _cacheLayer.Modify(model);
            if (response == (int)TinTucEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.TinTuc.ToStringValue()), model.TieuDe);
            if (response == (int)TinTucEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.TinTuc.ToStringValue(), TinTucInfoEnum.Title.ToStringValue()), model.TieuDe);
            if (response == (int)TinTucEnum.NotFound)
                return ResponseHelper.BadRequest(_localizer.GetNotExistMessage(NameControllerEnum.TinTuc.ToStringValue()));
            if (response == (int)TinTucEnum.NotExistLoaiTin)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.LoaiTin.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.TinTuc.ToStringValue()), model.TieuDe);
        }

        /// <summary>
        /// Lấy thông tin tin tức theo id
        /// API: /api/TinTuc/GetById/{idTinTuc}
        /// </summary>
        ///  <param name="idTinTuc"></param>
        /// <returns></returns>
        [HttpGet("GetById{idTinTuc}")]
        public IActionResult GetById(string idTinTuc)
        {
            var data = _cacheLayer.GetById(idTinTuc);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.TinTuc.ToStringValue()));

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Xóa tin tin tức theo id
        /// API: /api/TinTuc/Delete
        /// </summary>
        ///  <param name="idTinTuc"></param>
        ///  <param name="nguoiThucHien"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string idTinTuc, string nguoiThucHien)
        {
            var response = await _cacheLayer.Delete(idTinTuc, nguoiThucHien);
            if (response == (int)TinTucEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(_nameController));
            if (response == (int)TinTucEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(_nameController));
        }

        /// <summary>
        /// Lấy tất cả tin tức
        /// API: /api/TinTuc/GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAll();
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy danh sách tin tức theo searchparam
        /// API: /api/TinTuc/GetSearch
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        public IActionResult GetSearch([FromQuery] SearchParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearchTinTuc(out total, model);

            var outputData = new
            {
                tinTucs = data,
                totalRow = total,
                searchParam = model,
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Hiển thị tin tức
        /// API: /api/TinTuc/Show
        /// </summary>
        ///  <param name="idTinTuc"></param>
        /// <returns></returns>
        [HttpDelete("Show")]
        public async Task<IActionResult> Show(string idTinTuc)
        {
            var response = await _cacheLayer.HideTinTuc(idTinTuc, false);
            if (response == (int)TinTucEnum.Fail)
                return ResponseHelper.BadRequest("Hiển thị tin tức thất bại");
            if (response == (int)TinTucEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            else
                return ResponseHelper.Success("Hiển thị tin tức thành công");
        }

        /// <summary>
        /// Ẩn tin tức
        /// API: /api/TinTuc/Hide
        /// </summary>
        ///  <param name="idTinTuc"></param>
        /// <returns></returns>
        [HttpDelete("Hide")]
        public async Task<IActionResult> Hide(string idTinTuc)
        {
            var response = await _cacheLayer.HideTinTuc(idTinTuc, true);
            if (response == (int)TinTucEnum.Fail)
                return ResponseHelper.BadRequest("Ẩn tin tức thất bại");
            if (response == (int)TinTucEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            else
                return ResponseHelper.Success("Ẩn tin tức thành công");
        }
    }
}
