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
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Phoi
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoiBanSaoController : BaseAppController
    {
        private PhoiBanSaoCL _cacheLayer;
        private SysConfigCL _sysConfigCL;
        private readonly IMapper _mapper;


        private ILogger<PhoiBanSaoController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;


        public PhoiBanSaoController(ICacheService cacheService, IConfiguration configuration, IMapper imapper, ShareResource shareResource, ILogger<PhoiBanSaoController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new PhoiBanSaoCL(cacheService, configuration);
            _sysConfigCL = new SysConfigCL(cacheService);
            _mapper = imapper;
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }

        /// <summary>
        /// Thêm  phôi bản sao
        /// API: /api/PhoiBanSao/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] PhoiBanSaoInputModel model)
        {
            var searchParam = new SearchParamModel();
            searchParam.PageSize = -1;
            var configs = _sysConfigCL.GetAllConfig(searchParam);
            List<CauHinhPhoiGocModel> cauHinhPhoiBanSaos = new List<CauHinhPhoiGocModel>();
            foreach (var config in configs)
            {
                Guid guid = Guid.NewGuid();
                if (config.ConfigKey.ToLower().Contains("phoi_") && !config.ConfigKey.ToLower().Contains("phoi_goc"))
                {
                    var value = config.ConfigValue.Split(",");
                    CauHinhPhoiGocModel cauHinhPhoi = new CauHinhPhoiGocModel()
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

                    cauHinhPhoiBanSaos.Add(cauHinhPhoi);
                }
            }

            if (model.FileImage != null)
            {
                string folderName = "PhoiBanSao";
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

            var response = await _cacheLayer.Create(model, cauHinhPhoiBanSaos);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.PhoiBanSao.ToStringValue()), model.TenPhoi);
            if (response == (int)PhoiEnum.ExistName)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue(), PhoiInfoEnum.Name.ToStringValue()), model.TenPhoi);
            if (response == (int)PhoiEnum.ExistInUse)
                return ResponseHelper.BadRequest(_localizer.GetExistInUseMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.PhoiBanSao.ToStringValue()), model.TenPhoi);
        }

        /// <summary>
        /// Chỉnh sửa  phôi bản sao
        /// API: /api/PhoiBanSao/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] PhoiBanSaoInputModel model, string lyDo)
        {
            if (model == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
            if (model.FileImage != null)
            {
                string folderName = "PhoiBanSao";
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
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.PhoiBanSao.ToStringValue()), model.TenPhoi);
            if (response == (int)PhoiEnum.ExistName)
                return ResponseHelper.BadRequest(PhoiInfoEnum.Name.ToStringValue() + _localizer.GetAlreadyExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()), model.TenPhoi);
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.PhoiBanSao.ToStringValue()), model.TenPhoi);
        }

        /// <summary>
        /// Xóas phôi bản sao
        /// API: /api/PhoiBanSao/Delete
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
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
        }

        /// <summary>
        /// Lấy thông tin phôi bản sao theo id
        /// API: /api/PhoiBanSao/GetById{id}
        /// </summary>
        /// <param name="idPhoi"></param>
        /// <returns></returns>
        [HttpGet("GetById{idPhoi}")]
        public IActionResult GetById(string idPhoi)
        {
            var data = _cacheLayer.GetById(idPhoi);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));

            return ResponseHelper.Ok(data);
        }


        /// <summary>
        /// Lấy danh sách phôi bản sao (search param)
        /// API: /api/PhoiBanSao/GetSearch
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        public IActionResult GetSearch([FromQuery] SearchParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearchPhoiBanSao(out total, model);

            var outputData = new
            {
                PhoiGocs = data,
                totalRow = total,
                searchParam = model,
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Lấy tất cả phôi bản sao
        /// API: /api/PhoiBanSao/GetAll
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAll();
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Cấu hình phôi bản áo
        /// API: /api/PhoiBanSao/Setting
        /// </summary>
        /// <param name="models"></param>
        /// <param name="nguoiThucHien"></param>
        /// <param name="idPhoiBanSao"></param>
        /// <returns></returns>
        [HttpPost("Setting")]
        public async Task<IActionResult> Setting(string idPhoiBanSao, string nguoiThucHien, [FromBody] List<CauHinhPhoiGocInputModel> models)
        {
            var cauHinhPhoiBanSaos = _mapper.Map<List<CauHinhPhoiGocModel>>(models);

            var response = await _cacheLayer.CauHinhBanSao(idPhoiBanSao, nguoiThucHien, cauHinhPhoiBanSaos);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest("Cấu hình thất bại");
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
            else
                return ResponseHelper.Success("Cấu hình thành công");
        }

        /// <summary>
        ///Hủy phôi bản sao
        /// API: /api/PhoiBanSao/HuyPhoi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("HuyPhoi")]
        public async Task<IActionResult> HuyPhoi([FromForm] string id)
        {
          
            var response = await _cacheLayer.HuyPhoi(id);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest("Hủy phôi thất bại");
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
            else
                return ResponseHelper.Success("Hủy thành công");
        }

        /// <summary>
        /// Lấy danh sách cấu hình phôi bản sao
        /// API: /api/PhoiBanSao/GetCauHinhBanSao
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetCauHinhBanSao")]
        public IActionResult GetCauHinhBanSao(string idPhoiBanSao)
        {
            var data = _cacheLayer.GetCauHinhBanSao(idPhoiBanSao);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));

            return ResponseHelper.Ok(data);
        }


        /// <summary>
        /// Lấy thông tin cấu hình phôi bản sao
        /// API: /api/PhoiBanSao/GetCauHinhPhoiBanSaoById
        /// </summary>
        /// <param name="idPhoiBanSao"></param>
        /// <param name="idCauHinhPhoi"></param>
        /// <returns></returns>
        [HttpGet("GetCauHinhPhoiBanSaoById")]
        public IActionResult GetCauHinhPhoiBanSaoById(string idPhoiBanSao, string idCauHinhPhoi)
        {
            var data = _cacheLayer.GetCauHinhBanSaoById(idPhoiBanSao, idCauHinhPhoi);
            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));

            return ResponseHelper.Ok(data);
        }


        /// <summary>
        /// Chỉnh sửa cấu hình phôi bản sao
        /// API: /api/PhoiBanSao/ModifyCauHinhBanSao
        /// </summary>
        /// <param name="idPhoiBanSao"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("ModifyCauHinhBanSao")]
        public async Task<IActionResult> ModifyCauHinhBanSao(string idPhoiBanSao, [FromForm] CauHinhPhoiGocInputModel model)
        {
            var response = await _cacheLayer.ModifyCauHinhBanSao(idPhoiBanSao, model);
            if (response == (int)PhoiEnum.Fail)
                return ResponseHelper.BadRequest("Cấu hình thất bai");
            if (response == (int)PhoiEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.PhoiBanSao.ToStringValue()));
        }


        /// <summary>
        /// Lấy thông tin phôi bản sao đang sử dụng theo trường
        /// API: /api/PhoiBanSao/GetPhoiDangSuDung
        /// </summary>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        [HttpGet("GetPhoiDangSuDung")]
        public IActionResult GetPhoiDangSuDung(string idTruong)
        {
            var data = _cacheLayer.GetPhoiDangSuDung(idTruong);
            if (data == null)
                return ResponseHelper.NotFound("Hiện chưa có phôi nào đang sử dụng");

            return ResponseHelper.Ok(data);
        }
    }
}
