using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Attributes;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Phoi
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoiGocController : BaseAppController
    {
        private PhoiGocCL _cacheLayer;
        private SysConfigCL _sysConfigCL;

        private ILogger<PhoiGocController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public PhoiGocController(ICacheService cacheService, IConfiguration configuration, IMapper imapper, ShareResource shareResource, ILogger<PhoiGocController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new PhoiGocCL(cacheService, configuration);
            _sysConfigCL = new SysConfigCL(cacheService);

            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _mapper = imapper;
        }

        /// <summary>
        /// Thêm phôi gốc
        /// API: /api/PhoiGoc/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] PhoiGocInputModel model)
        {
            var searchParam = new SearchParamModel();
            searchParam.PageSize = -1;
            // Lấy danh sách cấu hình phôi
            var configs =  _sysConfigCL.GetAllConfig(searchParam);
            List<CauHinhPhoiGocModel> cauHinhPhoiGocs = new List<CauHinhPhoiGocModel>();
            foreach (var config in configs)
            {
                Guid guid = Guid.NewGuid();
                if (config.ConfigKey.ToLower().Contains("phoi_") && !config.ConfigKey.ToLower().Contains("phoi_sao"))
                {
                    var value = config.ConfigValue.Split(",");
                    CauHinhPhoiGocModel cauHinhPhoiGoc = new CauHinhPhoiGocModel()
                    {
                        Id = guid.ToString(),
                        MaTruongDuLieu = config.ConfigKey.Substring(5),
                        KieuChu = value[0].Trim(),
                        DinhDangKieuChu = value[1].Trim(),
                        CoChu = value[2].Trim(),
                        ViTriTren = int.TryParse(value[3].Trim(), out int viTriTren) ? viTriTren : 0,
                        ViTriTrai = int.TryParse(value[4].Trim(), out int viTriTrai) ? viTriTrai : 0,
                        MauChu = value[5].Trim(),
                    };

                    cauHinhPhoiGocs.Add(cauHinhPhoiGoc);
                }
            }

            if (model.FileImage != null)
            {
                string folderName = "PhoiGoc";
                var fileResult = _fileService.SaveImage(model.FileImage, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.AnhPhoi = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }
            
            var response = await _cacheLayer.Create(model, cauHinhPhoiGocs);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.PhoiGoc.ToStringValue()), model.TenPhoi);
            if (response == (int)PhoiEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.PhoiGoc.ToStringValue(), PhoiInfoEnum.Name.ToStringValue()), model.TenPhoi);
            if (response == (int)PhoiEnum.ExistNumber)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.PhoiGoc.ToStringValue(), PhoiInfoEnum.Number.ToStringValue()), model.SoHieuPhoi);
            if (response == (int)PhoiEnum.ExistDate)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.PhoiGoc.ToStringValue(), PhoiInfoEnum.DateApprove.ToStringValue()), model.NgayApDung.ToString("dd/mm/yyyy"));
            if (response == (int)PhoiEnum.ExistInUse)
                return ResponseHelper.BadRequest((_localizer.GetExistInUseMessage(NameControllerEnum.PhoiGoc.ToStringValue())));
            else
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.PhoiGoc.ToStringValue()), model.TenPhoi);
        }

        /// <summary>
        /// Chỉnh sửa phôi gốc
        /// API: /api/PhoiGoc/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] PhoiGocInputModel model, string lyDo)
        {
            if (model == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            if (model.FileImage != null)
            {
                string folderName = "PhoiGoc";
                var fileResult = _fileService.SaveImage(model.FileImage, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.AnhPhoi = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            var response = await _cacheLayer.Modify(model, lyDo);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.PhoiGoc.ToStringValue()), model.TenPhoi);
            if (response == (int)PhoiEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.PhoiGoc.ToStringValue(), PhoiInfoEnum.Name.ToStringValue()), model.TenPhoi);
            if (response == (int)PhoiEnum.ExistNumber)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.PhoiGoc.ToStringValue(), PhoiInfoEnum.Number.ToStringValue()), model.SoHieuPhoi);
            if (response == (int)PhoiEnum.ExistDate)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.PhoiGoc.ToStringValue(), PhoiInfoEnum.DateApprove.ToStringValue()), model.NgayApDung.ToString("dd/mm/yyyy"));
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.PhoiGoc.ToStringValue()), model.TenPhoi);
        }

        /// <summary>
        /// Xóa phôi gốc
        /// API: /api/PhoiGoc/Delete
        /// </summary>
        /// <param name="idPhoiGoc"></param>
        /// <param name="nguoiThucHien"></param>
        /// <param name="lyDo"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string idPhoiGoc, string nguoiThucHien, string lyDo)
        {
            var response = await _cacheLayer.Delete(idPhoiGoc, nguoiThucHien, lyDo);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
        }

        /// <summary>
        /// Lấy thông tin phôi gốc theo id
        /// API: /api/PhoiGoc/{idPhoi}
        /// </summary>
        /// <param name="idPhoi"></param>
        /// <returns></returns>
        [HttpGet("GetById{idPhoi}")]
        public IActionResult GetById(string idPhoi)
        {
            var data = _cacheLayer.GetById(idPhoi);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy tất cả phôi gốc
        /// API: /api/PhoiGoc/GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAll();
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy danh sách phôi gốc (search param)
        /// API: /api/PhoiGoc/GetSearch
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        public IActionResult GetSearch([FromQuery] SearchParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearchPhoiGoc(out total, model);
          
            var outputData = new
            {
                PhoiGocs = data,
                totalRow = total,
                searchParam = model,
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Cấu hình phôi gốc
        /// API: /api/PhoiGoc/Setting
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost("Setting")]
        public async Task<IActionResult> Setting(string idPhoiGoc, string nguoiThucHien, [FromBody] List<CauHinhPhoiGocInputModel> models)
        {
            var cauHinhPhoiGocs = _mapper.Map<List<CauHinhPhoiGocModel>>(models);

            var response = await _cacheLayer.CauHinhPhoiGoc(idPhoiGoc, nguoiThucHien, cauHinhPhoiGocs);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetSettingErrorMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetSettingSuccessMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
        }

        /// <summary>
        /// Hủy phôi gốc
        /// API: /api/PhoiGoc/HuyPhoi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("HuyPhoi")]
        public async Task<IActionResult> HuyPhoi([FromForm] HuyPhoiGocInputModel model)
        {
            if (model.FileBienBan != null)
            {
                string folderName = "BienBanHuyPhoi";
                var fileResult = _fileService.SaveFilePDFOrWorld(model.FileBienBan, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.FileBienBanHuyPhoi = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            var response = await _cacheLayer.HuyPhoi(model);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetCancelErrorMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetCancelSuccessMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
        }

        /// <summary>
        /// Lấy thông tin phôi gốc đang sửa dụng theo idTruong
        /// API: /api/PhoiGoc/GetPhoiDangSuDung
        /// </summary>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        [HttpGet("GetPhoiDangSuDung")]
        public IActionResult GetPhoiDangSuDung(string idTruong)
        {
            var data = _cacheLayer.GetPhoiDangSuDung(idTruong);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy danh sách cấu hình phôi gốc theo idPhoiGoc
        /// API: /api/PhoiGoc/GetCauHinhPhoiGoc
        /// </summary>
        /// <param name="idPhoiGoc"></param>
        /// <returns></returns>
        [HttpGet("GetCauHinhPhoiGoc")]
        public IActionResult GetCauHinhPhoiGoc(string idPhoiGoc)
        {
            var data = _cacheLayer.GetCauHinhPhoiGoc(idPhoiGoc);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy danh sách phôi gốc đã hủy (search param)
        /// API: /api/PhoiGoc/GetSearchPhoiDaHuy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetSearchPhoiDaHuy")]
        public IActionResult GetSearchPhoiDaHuy([FromQuery] SearchParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearchPhoiDaHuy(out total, model);

            var outputData = new
            {
                PhoiGocs = data,
                totalRow = total,
                searchParam = model,
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Lấy danh sách cấu hình phôi gốc
        /// API: /api/PhoiGoc/GetCauHinhPhoiGocById
        /// </summary>
        /// <param name="idPhoiGoc"></param>
        /// <param name="idCauHinhPhoi"></param>
        /// <returns></returns>
        [HttpGet("GetCauHinhPhoiGocById")]
        public IActionResult GetCauHinhPhoiGocById(string idPhoiGoc, string idCauHinhPhoi)
        {
            var data = _cacheLayer.GetCauHinhPhoiGocById(idPhoiGoc, idCauHinhPhoi);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Chỉnh sửa cấu hình phôi gốc
        /// API: /api/PhoiGoc/ModifyCauHinhPhoiGoc
        /// </summary>
        /// <param name="idPhoiGoc"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("ModifyCauHinhPhoiGoc")]
        public async Task<IActionResult> ModifyCauHinhPhoiGoc(string idPhoiGoc, [FromForm] CauHinhPhoiGocInputModel model)
        {
            var response = await _cacheLayer.ModifyCauHinhPhoiGoc(idPhoiGoc, model);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetSettingErrorMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetSettingSuccessMessage(NameControllerEnum.PhoiGoc.ToStringValue()));
        }

    }
}
