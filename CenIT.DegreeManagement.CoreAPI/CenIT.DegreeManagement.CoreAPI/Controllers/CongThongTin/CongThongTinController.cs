using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Caching.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Processor.SendNotification;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.CongThongTin
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CongThongTinController : BaseAppController
    {
        private DonYeuCauCapBanSaoCL _donYeuCauCapCL;
        private HocSinhCL _hocSinhCL;
        private PhoiGocCL _phoiGocCL;
        private TruongCL _truongCL;
        private SysConfigCL _sysConfigCL;
        private NamThiCL _namThiCl;
        private TinTucCL _tinTuc;
        private LoaiTinTucCL _loaiTinTuc;
        private HuyBoVangBangCL _cacheHuyBoVanBang;
        private ChinhSuaVanBangCL _chinhSuaVanBangCL;


        private readonly FirebaseNotificationUtils _firebaseNotificationUtils;
        private readonly BackgroundJobManager _backgroundJobManager;
        private SysDeviceTokenCL _sysDeviceTokenCL;
        private MessageCL _messageCL;
        private readonly IMapper _mapper;
        private ILogger<CongThongTinController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;
        private TruongCL _truong;
        private DanTocCL _danTocCL;
        private SysMessageConfigCL _sysMessageConfigCL;


        public CongThongTinController(ICacheService cacheService, BackgroundJobManager backgroundJobManager, FirebaseNotificationUtils firebaseNotificationUtils, IConfiguration configuration, ShareResource shareResource, ILogger<CongThongTinController> logger, IFileService fileService, IMapper imapper) : base(cacheService, configuration)
        {
            _donYeuCauCapCL = new DonYeuCauCapBanSaoCL(cacheService, configuration);
            _phoiGocCL = new PhoiGocCL(cacheService, configuration);
            _truongCL = new TruongCL(cacheService, configuration);
            _sysConfigCL = new SysConfigCL(cacheService);
            _namThiCl = new NamThiCL(cacheService, configuration);
            _danTocCL = new DanTocCL(cacheService, configuration);
            _tinTuc = new TinTucCL(cacheService, configuration);
            _loaiTinTuc = new LoaiTinTucCL(cacheService, configuration);
            _firebaseNotificationUtils = firebaseNotificationUtils;
            _backgroundJobManager = backgroundJobManager;
            _mapper = imapper;
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _hocSinhCL = new HocSinhCL(cacheService, configuration);
            _messageCL = new MessageCL(cacheService);
            _sysDeviceTokenCL = new SysDeviceTokenCL(cacheService);
            _truong = new TruongCL(cacheService, configuration);
            _sysMessageConfigCL = new SysMessageConfigCL(cacheService);
            _cacheHuyBoVanBang = new HuyBoVangBangCL(cacheService, configuration);
            _chinhSuaVanBangCL = new ChinhSuaVanBangCL(cacheService, configuration);

        }

        [HttpPost("CreateDonYeuCau")]
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

            if (model.PhuongThucNhan == 1 && string.IsNullOrEmpty(model.DiaChiNhan))
                return ResponseHelper.BadRequest("Địa chỉ nhận bằng không được để trống");

            var truong = _truongCL.GetById(model.IdTruong);

            var response = await _donYeuCauCapCL.CreateDonYeuCau(model, truong.IdCha);
            if (response.MaLoi == (int)DonYeuCauCapBanSaoEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.DonYeuCau.ToStringValue()), model.HoTen);
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
                string actionName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ActionName!;
                string controllerName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ControllerName!;
                string action = EString.GenerateActionString(controllerName, actionName);
                var messageConfig = _sysMessageConfigCL.GetByActionName(action);
                if(messageConfig != null)
                {
                    var phong = _truong.GetPhong(model.IdTruong);
                    var deviceTokens = _sysDeviceTokenCL.GetByIdDonVi(phong.Id);
                    var jsonData = new
                    {
                        Cccd = model.CCCD,
                        HoTen = model.HoTen,
                        Ma = response.MaDonYeuCau

                    };
                    string jsonString = JsonConvert.SerializeObject(jsonData);
                    MessageInputModel message = new MessageInputModel()
                    {
                        IdMessage = Guid.NewGuid().ToString(),
                        Action = action,
                        MessageType = MessageTypeEnum.NguoiDungGuiPhong.ToStringValue(),
                        SendingMethod = SendingMethodEnum.Notification.ToStringValue(),
                        Title = messageConfig.Title,
                        Content = string.Format(messageConfig.Body, model.HoTenNguoiYeuCau) ,
                        Color = messageConfig.Color,
                        Recipient = null,
                        Url = messageConfig.URL,
                        IDDonVi = phong.Id,
                        ValueRedirect = jsonString
                    };

                    var updateSoVaoSo = new UpdateCauHinhSoVaoSoInputModel()
                    {
                        SoDonYeuCau = 1,
                        Nam = response.Nam,
                        LoaiHanhDong = SoVaoSoEnum.SoVaoSoBanSao,
                        IdTruong = truong.IdCha
                    };

                    _truongCL.UpdateCauHinhSoVaoSo(updateSoVaoSo);

                    message.IdMessage = Guid.NewGuid().ToString();
                    var sendMessage = _messageCL.Save(message);
                    BackgroundJob.Enqueue(() => _backgroundJobManager.SendNotificationInBackground(message.Title, message.Content, deviceTokens));

                }
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.DonYeuCau.ToStringValue()), model.HoTen);
            }
        }



        [HttpGet("GetSearchVBCC")]
        public IActionResult GetSearchVBCC([FromQuery] TraCuuVBCCModel model)
        {
            int total;
            var data = _hocSinhCL.GetSearchVBCC(out total, model);
            var outputData = new
            {
                hocSinhs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }


        [HttpGet("GetSearchDonYeuCau")]
        [AllowAnonymous]
        public IActionResult GetSearchDonYeuCau([FromQuery] TraCuuDonYeuCau model)
        {
            int total;
            var data = _donYeuCauCapCL.GetSearchDonYeuCauCongThongTin(out total, model);
            var outputData = new
            {
                donYeuCaus = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetPhoiGocById")]
        public IActionResult GetPhoiGocById(string idPhoiGoc)
        {
            var data = _phoiGocCL.GetById(idPhoiGoc);
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetAllConfig")]
        public IActionResult GetAllConfig()
        {
            var searchParam = new SearchParamModel();
            searchParam.PageSize = -1;
            var data = _sysConfigCL.GetAllConfig(searchParam);
            return ResponseHelper.Ok(data);
        }


        [HttpGet("GetHocSinhByCCCD")]
        public IActionResult GetHocSinhByCCCD(string cccd)
        {
            var data = _hocSinhCL.GetHocSinhByCccd(cccd);
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetPhongById")]
        public IActionResult GetPhongById(string id)
        {
            var data = _truongCL.GetById(id);
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetAllTruong")]
        public async Task<IActionResult> GetAllTruong()
        {
            var data = _truongCL.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetAllNamThi")]
        public IActionResult GetAllNamThi()
        {
            var data = _namThiCl.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetAllDanToc")]
        public IActionResult GetAllDanToc()
        {
            var data = _danTocCL.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetPhong")]
        public IActionResult GetPhong()
        {
            var data = _truongCL.GetDonViQuanLySo();
            return ResponseHelper.Ok(data);
        }

        #region

        [HttpGet("GetSearchHuyBoVanBang")]
        [AllowAnonymous]
        public IActionResult GetSearchHuyBoVanBang([FromQuery] SearchParamModel model)
        {
            int total;

            var data = _cacheHuyBoVanBang.GetSerachHuyBoVanBang(out total, model);

            var outputData = new
            {
                LichSus = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetSerachPhuLucCapLaiVanBang")]
        [AllowAnonymous]
        public IActionResult GetSerachPhuLucCapLaiVanBang([FromQuery] SearchParamModel model)
        {
            int total;

            var data = _chinhSuaVanBangCL.GetSerachPhuLucCapLaiVanBang(out total, model);

            var outputData = new
            {
                LichSus = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetSerachPhuLucChinhSuaVanBang")]
        [AllowAnonymous]
        public IActionResult GetSerachPhuLucChinhSuaVanBang([FromQuery] SearchParamModel model)
        {
            int total;

            var data = _chinhSuaVanBangCL.GetSerachPhuLucChinhSuaVanBang(out total, model);

            var outputData = new
            {
                LichSus = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        #endregion

        #region Tin tức
        [HttpGet("GetAllTinTuc")]
        public IActionResult GetAllTinTuc()
        {
            var data = _tinTuc.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetSearchTinTucByIdLoaiTin")]
        public IActionResult GetSearchTinTucByIdLoaiTin([FromQuery] SearchParamModel model, string idLoaiTin)
        {
            int total;
            var data = _tinTuc.GetSearchTinTucByIdLoaiTin(out total, idLoaiTin, model);
            var outputData = new
            {
                tinTuc = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }


        [HttpGet("GetSearchTinTuc")]
        public IActionResult GetSearchTinTuc([FromQuery] SearchParamModel model)
        {
            int total;
            var data = _tinTuc.GetSearchTinTuc(out total, model, true);
            List<TinTucListModel> tinTucs = _mapper.Map<List<TinTucListModel>>(data);

            var outputData = new
            {
                tinTuc = tinTucs,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetAllLoaiTinTuc")]
        public IActionResult GetAllLoaiTinTuc()
        {
            var data = _loaiTinTuc.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetTinTucById")]
        public IActionResult GetTinTucById(string idTinTuc)
        {
            var data = _tinTuc.GetById(idTinTuc);
            return ResponseHelper.Ok(data);
        }


        [HttpGet("GetLatest4News")]
        public IActionResult GetLatest4News()
        {
            var data = _tinTuc.GetLatest4News();
            List<TinTucListModel> tinTucs = _mapper.Map<List<TinTucListModel>>(data);

            return ResponseHelper.Ok(tinTucs);
        }
        #endregion
    }
}
