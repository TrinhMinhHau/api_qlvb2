using CenIT.DegreeManagement.CoreAPI.Caching.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.TinTuc
{
    [Route("api/[controller]")]
    [ApiController]

    public class LoaiTinTucController : BaseAppController
    {
        private LoaiTinTucCL _cacheLayer;
        private ILogger<LoaiTinTucController> _logger;
        private readonly ShareResource _localizer;


        public LoaiTinTucController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<LoaiTinTucController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new LoaiTinTucCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
        }

        /// <summary>
        /// Thêm loại tin tức
        /// API: /api/LoaiTinTuc/Create
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] LoaiTinTucInputModel model)
        {
            var response = await _cacheLayer.Create(model);
            if (response == (int)LoaiTinTucEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.LoaiTin.ToStringValue()), model.TieuDe);
            if (response == (int)LoaiTinTucEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.LoaiTin.ToStringValue(), LoaiTinTucInfoEnum.Title.ToStringValue()), model.TieuDe);
            return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.LoaiTin.ToStringValue()), model.TieuDe);
        }

        /// <summary>
        /// Chỉnh sửa loại tin tức
        /// API: /api/LoaiTinTuc/Update
        /// </summary>
        ///  <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async  Task<IActionResult> Update([FromForm] LoaiTinTucInputModel model)
        {
            var response =  await _cacheLayer.Modify(model);
            if (response == (int)LoaiTinTucEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.LoaiTin.ToStringValue()), model.TieuDe);
            if (response == (int)LoaiTinTucEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.LoaiTin.ToStringValue(), LoaiTinTucInfoEnum.Title.ToStringValue()), model.TieuDe);
            if (response == (int)LoaiTinTucEnum.NotFound)
                return ResponseHelper.BadRequest(_localizer.GetNotExistMessage(NameControllerEnum.LoaiTin.ToStringValue()));
            else 
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.LoaiTin.ToStringValue()), model.TieuDe);
        }

        /// <summary>
        /// Lấy thông tin loại tin tức theo id
        /// API: /api/LoaiTinTuc/GetById{idLoaiTinTuc}
        /// </summary>
        ///  <param name="idLoaiTinTuc"></param>
        /// <returns></returns>
        [HttpGet("GetById{idLoaiTinTuc}")]
        public IActionResult GetById(string idLoaiTinTuc)
        {
            var data = _cacheLayer.GetById(idLoaiTinTuc);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.LoaiTin.ToStringValue()));

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Xóa loại tin tức
        /// API: /api/LoaiTinTuc/Delete
        /// </summary>
        ///  <param name="idLoaiTinTuc"></param>
        ///  <param name="nguoiThucHien"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string idLoaiTinTuc, string nguoiThucHien)
        {
            var response = await _cacheLayer.Delete(idLoaiTinTuc, nguoiThucHien);
            if (response == (int)LoaiTinTucEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.LoaiTin.ToStringValue()));
            if (response == (int)LoaiTinTucEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.LoaiTin.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.LoaiTin.ToStringValue()));
        }

        /// <summary>
        /// Lấy tất cả loại tin tức
        /// API: /api/LoaiTinTuc/GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAll();
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy danh sách tin tức theo search param
        /// API: /api/LoaiTinTuc/GetSearch
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        public IActionResult GetSearch([FromQuery] SearchParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearchLoaiTinTuc(out total, model);

            var outputData = new
            {
                loaiTinTucs = data,
                totalRow = total,
                searchParam = model,
            };
            return ResponseHelper.Ok(outputData);
        }
    }
}