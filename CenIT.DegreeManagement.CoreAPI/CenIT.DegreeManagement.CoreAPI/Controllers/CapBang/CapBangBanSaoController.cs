using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Processor.Mail;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.CapBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapBangBanSaoController : BaseAppController
    {
        private DonYeuCauCapBanSaoCL _donYeuCauCapBanSaoCL;
        private MessageCL _messageCL;
        private SysMessageConfigCL _sysMessageConfigCL;
        private SysUserCL _sysUserCL;
        private HocSinhCL _hocSinhCL;
        private ILogger<CapBangBanSaoController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly BackgroundJobManager _backgroundJobManager;
        private TruongCL _truongCL;

        private ISendMailService _sendMailService;

        private readonly string _nameDonYeuCau = "DonYeuCau";

        public CapBangBanSaoController(ICacheService cacheService, BackgroundJobManager backgroundJobManager , IConfiguration configuration, IFileService fileService, ISendMailService sendMailService, ShareResource shareResource, ILogger<CapBangBanSaoController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _donYeuCauCapBanSaoCL = new DonYeuCauCapBanSaoCL(cacheService, configuration);
            _hocSinhCL = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
            _fileService = fileService;
            _sendMailService = sendMailService;
            _messageCL = new MessageCL(cacheService);
            _sysMessageConfigCL = new SysMessageConfigCL(cacheService);
            _backgroundJobManager = backgroundJobManager;
            _truongCL = new TruongCL(cacheService, configuration);
            _sysUserCL = new SysUserCL(cacheService);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("CreateDonYeuCau")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDonYeuCau([FromForm] DonYeuCauCapBanSaoInputModel model)
        {
            if (model.FileDonYeuCau != null)
            {
                string folderName = "DonYeuCau";
                var fileResult = _fileService.SaveFilePDFOrWorld(model.FileDonYeuCau, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.DonYeuCau = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            if (model.FileHinhAnhCCCD != null)
            {
                string folderName = "CCCD";
                var fileResult = _fileService.SaveImage(model.FileHinhAnhCCCD, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.HinhAnhCCCD = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }


            if(model.PhuongThucNhan == 1 && string.IsNullOrEmpty(model.DiaChiNhan))
                return ResponseHelper.BadRequest("Địa chỉ nhận bằng không được để trống");

            var user = _sysUserCL.GetByUsername(model.NguoiThucHien);
            
            var response = await _donYeuCauCapBanSaoCL.CreateDonYeuCau(model, user.TruongID);
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(_nameDonYeuCau), model.HoTen);
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.InforStudentWrong)
                return ResponseHelper.BadRequest("Thông tin học sinh không có trong sổ gốc");
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.FullNameIncorrect)
                return ResponseHelper.BadRequest("Họ tên không chính xác");
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.NationIncorrect)
                return ResponseHelper.BadRequest("Dân tộc không chính xác");
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.GenderIncorrect)
                return ResponseHelper.BadRequest("Giới tính không chính xác");
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.ClassificationIncorrect)
                return ResponseHelper.BadRequest("Xếp loại không chính xác");
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.PlaceIncorrect)
                return ResponseHelper.BadRequest("Nơi sinh không chính xác");
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.BirthDayIncorrect)
                return ResponseHelper.BadRequest("Ngày sinh không chính xác");
            else
            {

                var updateSoVaoSo = new UpdateCauHinhSoVaoSoInputModel()
                {
                    SoDonYeuCau = 1,
                    Nam = response.Nam,
                    LoaiHanhDong = SoVaoSoEnum.SoVaoSoBanSao,
                    IdTruong = user.TruongID
                };

                _truongCL.UpdateCauHinhSoVaoSo(updateSoVaoSo);

                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(_nameDonYeuCau), model.HoTen);
            }

         
        }


        [HttpGet("GetSearchDonYeuCau")]
        [AllowAnonymous]
        public IActionResult GetSearchDonYeuCau([FromQuery] DonYeuCauCapBanSaoParamModel model)
        {
            int total;
            var user = _sysUserCL.GetByUsername(model.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            var data = _donYeuCauCapBanSaoCL.GetSearchDonYeuCau(out total, model, donVi);
            var outputData = new
            {
                DonYeuCaus = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetSearchDonYeuCauDaDuyet")]
        [AllowAnonymous]
        public IActionResult GetSearchDonYeuCauDaDuyet([FromQuery] HocSinhCapBanSaoParamModel model)
        {
            int total;
            var user = _sysUserCL.GetByUsername(model.NguoiThucHien);
            var donVi = _truongCL.GetById(user .TruongID);
            var data = _donYeuCauCapBanSaoCL.GetSearchDonYeuCauDaDuyet(out total, model, donVi);
            var outputData = new
            {
                DonYeuCaus = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetSerachDonYeuCapBanSao")]
        [AllowAnonymous]
        public IActionResult GetSerachDonYeuCapBanSao([FromQuery] DonYeuCauCapBanSaoParamModel modelSearch)
        {
            var user = _sysUserCL.GetByUsername(modelSearch.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            var data = _donYeuCauCapBanSaoCL.GetSerachDonYeuCapBanSao(modelSearch, donVi);

            return Ok(ResponseHelper.ResultJson(data));
        }


        [HttpGet("GetSearchLichSuDonYeuCau")]
        [AllowAnonymous]
        public IActionResult GetSearchLichSuDonYeuCau(string idHocSinh,[FromQuery] SearchParamModel modelSearch)
        {
            int total;
            var data = _donYeuCauCapBanSaoCL.GetSearchLichSuDonYeuCau(out total, idHocSinh, modelSearch);

            var outputData = new
            {
                DanhMucTotNghieps = data,
                totalRow = total,
                searchParam = modelSearch,
            };

            return ResponseHelper.Ok(outputData);
        }


        [HttpGet("GetById")]
        public IActionResult GetById(string id)
        {
            var data = _donYeuCauCapBanSaoCL.GetById(id);
            return data != null ? ResponseHelper.Ok(data)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDonYeuCau));
        }

        [HttpPost("TuChoiDoYeuCau")]
        public async Task<IActionResult> TuChoiDoYeuCau(string idDonYeuCauCapBanSao, string lyDoTuChoi, string nguoiThucHien)
        {
            var response = await _donYeuCauCapBanSaoCL.TuChoiDoYeuCau(idDonYeuCauCapBanSao, lyDoTuChoi, nguoiThucHien);
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.Success)
            {
       
                string actionName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ActionName!;
                string controllerName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ControllerName!;
                string action = EString.GenerateActionString(controllerName, actionName);
                var messageConfig = _sysMessageConfigCL.GetByActionName(action);

                if(messageConfig != null)
                {
                   
                    MailContent content = new MailContent
                    {
                        To = response.EmailNguoiYeuCau,
                        Subject = messageConfig.Title,
                        Body = string.Format(messageConfig.Body, response.HoTenNguoiYeuCau, lyDoTuChoi)
                    };

                    BackgroundJob.Enqueue(() => _backgroundJobManager.SendEmailInBackground(content));
                }

                return ResponseHelper.Success("Từ chối yêu cầu thành công");
            }


            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDonYeuCau));
            else
                return ResponseHelper.BadRequest("Từ chối thất bại");
        }

        [HttpPost("DuyetDonYeuCau")]
        public async Task<IActionResult> DuyetDonYeuCau([FromForm]DuyetDonYeuCauInputModel model)
        {

            var response = await _donYeuCauCapBanSaoCL.DuyetDonYeuCau(model);
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.Success)
            {
                string actionName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ActionName!;
                string controllerName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ControllerName!;
                string action = response.PhuongThucNhan == 1 ? EString.GenerateActionString(controllerName, actionName) + "_DVCong" : EString.GenerateActionString(controllerName, actionName) + "_TrucTiep" ;
                var messageConfig = _sysMessageConfigCL.GetByActionName(action);
                if (messageConfig != null)
                {

                    MailContent content = new MailContent
                    {
                        To = response.EmailNguoiYeuCau,
                        Subject = messageConfig.Title,
                        Body =
                        response.PhuongThucNhan == 1 ? string.Format(messageConfig.Body, response.HoTenNguoiYeuCau, response.MaDon, response.SoLuongBanSao, model.NgayNhan.Date, response.DiaChiNhan, model.LePhi)
                                : string.Format(messageConfig.Body, response.HoTenNguoiYeuCau, response.MaDon, response.SoLuongBanSao, model.NgayNhan.Date, model.LePhi)
                    };

                    BackgroundJob.Enqueue(() => _backgroundJobManager.SendEmailInBackground(content));
                }

                return ResponseHelper.Success("Duyệt yêu cầu thành công");
            }
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDonYeuCau));
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.NotExistPhoi)
                return ResponseHelper.NotFound("Phôi không tồn tại");
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.ExceedsPhoiGocLimit)
                return ResponseHelper.BadRequest("Số lượng học sinh vượt quá số lượng phôi");
            else
                return ResponseHelper.BadRequest("Duyệt thất bại");
          
        }

        [HttpPost("XacNhanIn")]
        public async Task<IActionResult> XacNhanIn(string idDonYeuCauCapBanSao, string nguoiThucHien)
        {
            var response = await _donYeuCauCapBanSaoCL.XacNhanIn(idDonYeuCauCapBanSao, nguoiThucHien);
            if (response == (int)DonYeuCauCapBanSaoEnum.Fail)
                return ResponseHelper.BadRequest("Xác nhận in bản sao thất bại");
            if (response == (int)DonYeuCauCapBanSaoEnum.ListEmpty)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDonYeuCau));
            else
                return ResponseHelper.Success("Xác nhận in bản sao thành công");
        }


        [HttpPost("XacNhanDaPhat")]
        public async Task<IActionResult> XacNhanDaPhat(string idDonYeuCauCapBanSao)
        {
            var response = await _donYeuCauCapBanSaoCL.XacNhanDaPhat(idDonYeuCauCapBanSao);
            if (response == (int)DonYeuCauCapBanSaoEnum.Fail)
                return ResponseHelper.BadRequest("Xác nhận đã phát bản sao thất bại");
            if (response == (int)DonYeuCauCapBanSaoEnum.ListEmpty)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDonYeuCau));
            else
                return ResponseHelper.Success("Xác nhận đã phát bản sao thành công");
        }

        [HttpGet("GetHocSinhDaDuaVaoSoBanSao")]
        [AllowAnonymous]
        public IActionResult GetHocSinhDaDuaVaoSoBanSao(string idHocSinh, string idDonYeuCau, string nguoiThucHien)
        {
            var user = _sysUserCL.GetByUsername(nguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            var data = _hocSinhCL.GetHocSinhDaDuaVaoSoBanSao(idHocSinh, idDonYeuCau, donVi);

            return data != null ? ResponseHelper.Ok(data)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDonYeuCau));
        }

    }
}
