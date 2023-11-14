using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor.SendNotification;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using Hangfire;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.CapBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapBangBanChinhController : BaseAppController
    {
        private HocSinhCL _cacheLayer;
        private DanhMucTotNghiepCL _danhMucTotNghiepCL;
        private ILogger<CapBangBanChinhController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;
        private readonly string _nameHocSinh = "HocSinh";
        private readonly string _nameDMTN = "DanhMucTotNghiep";
        private readonly string _nameTruong = "Truong";
        private readonly string _nameNamThi = "NamThi";
        private readonly FirebaseNotificationUtils _firebaseNotificationUtils;
        private readonly BackgroundJobManager _backgroundJobManager;
        private MessageCL _messageCL;
        private SysDeviceTokenCL _sysDeviceTokenCL;
        private SysMessageConfigCL _sysMessageConfigCL;
        private TruongCL _truongCL;

        public CapBangBanChinhController(ICacheService cacheService, IConfiguration configuration, BackgroundJobManager backgroundJobManager, FirebaseNotificationUtils firebaseNotificationUtils, ShareResource shareResource, ILogger<CapBangBanChinhController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService, configuration);
            _firebaseNotificationUtils = firebaseNotificationUtils;
            _backgroundJobManager = backgroundJobManager;
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
            _messageCL = new MessageCL(cacheService);
            _sysDeviceTokenCL = new SysDeviceTokenCL(cacheService);
            _sysMessageConfigCL = new SysMessageConfigCL(cacheService);
            _truongCL = new TruongCL(cacheService, configuration);
        }

        /// <summary>
        /// Lấy danh sách học sinh  đã duyệt, đã in (chức năng cấp bằng)
        /// API của phòng
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetHocSinhCapBangSearch")]
        public IActionResult GetHocSinhCapBangSearch([FromQuery] string? idTruong, [FromQuery] HocSinhParamModel model)
        {
            int total;
            var data = _cacheLayer.GetSearchHocSinhCapBangByPhong(out total, idTruong, model);
            var outputData = new
            {
                HocSinhs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetAllPreviewHocSinhVaoSoGoc")]
        public IActionResult GetAllPreviewHocSinhVaoSoGoc(string idTruong, string idDanhMucTotNghiep, [FromQuery] SearchParamModel model)
        {
           
            var response = _cacheLayer.GetAllPreviewHocSinhVaoSoGoc(idTruong, idDanhMucTotNghiep);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest("");
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameHocSinh));
            if (response.MaLoi == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDMTN));
            if (response.MaLoi == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameTruong));
            if (response.MaLoi == (int)HocSinhEnum.NotExistYear)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameNamThi));

            var filteredHocSinhs = FilterHocSinhs(response.HocSinhs, model); // Áp dụng tìm kiếm và phân trang

            var totalRow = filteredHocSinhs.Count(); // Tổng số học sinh sau khi áp dụng tìm kiếm

            //Áp dụng phân trang dựa trên StartIndex và PageSize
            var pagedHocSinhs = filteredHocSinhs.Skip(model.StartIndex).Take(model.PageSize);

            var outputData = new
            {
                HocSinhs = pagedHocSinhs,
                totalRow = totalRow,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }


        [HttpPost("PutIntoSoGoc")]
        [AllowAnonymous]
        public async Task<IActionResult> PutIntoSoGoc([FromForm] SoGocInputModel model)
        {
            var response = await _cacheLayer.PutIntoSoGoc(model);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetPutInToErrorMessage(_nameHocSinh));
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameHocSinh));
            if (response.MaLoi == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.BadRequest(_localizer.GetNotExistMessage(_nameTruong));
            if (response.MaLoi == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameDMTN));
            if (response.MaLoi == (int)HocSinhEnum.NotExistSoGoc)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage("SoGoc"));
            else
            {
                var updateSoVaoSo = new UpdateCauHinhSoVaoSoInputModel()
                {
                    DinhDangSoThuTuSoGoc = response.SoluongHocSinh,
                    Nam = response.Nam,
                    LoaiHanhDong = SoVaoSoEnum.SoVaoSoGoc,
                    IdTruong = model.IdTruong
                };

                var result = _truongCL.UpdateCauHinhSoVaoSo(updateSoVaoSo);
                await _danhMucTotNghiepCL.CapNhatTrangThaiDaInBang(model.IdDanhMucTotNghiep);
                return ResponseHelper.Success(_localizer.GetPutInToSuccessMessage(_nameHocSinh));
            }    
               
        }

        [HttpGet("GetDanhSachHocSinhDaDuaVaoSo")]
        [AllowAnonymous]
        public IActionResult GetDanhSachHocSinhDaDuaVaoSo(string idTruong, string idDanhMucTotNghiep)
        {
            var data = _cacheLayer.GetAllHocSinhDaDuaVaoSo(idTruong, idDanhMucTotNghiep);

            List<HocSinhInBangDTO> hocSinhs = _mapper.Map<List<HocSinhInBangDTO>>(data);

            return ResponseHelper.Ok(hocSinhs);

        }


        [HttpPost("GetListHocSinhDaDuaVaoSo")]
        [AllowAnonymous]
        public IActionResult GetListHocSinhDaDuaVaoSo(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            var data = _cacheLayer.GetListHocSinhDaDuaVaoSo(idTruong, idDanhMucTotNghiep, listCCCD);

            List<HocSinhInBangDTO> hocSinhs = _mapper.Map<List<HocSinhInBangDTO>>(data);

            return ResponseHelper.Ok(hocSinhs);
        }

        [HttpGet("GetHocSinhDaDuaVaoSoGocById")]
        [AllowAnonymous]
        public IActionResult GetHocSinhDaDuaVaoSoGocById(string idHocSinh)
        {
            var data = _cacheLayer.GetHocSinhDaDuaVaoSoGocById(idHocSinh);

            HocSinhInBangDTO hocSinhs = _mapper.Map<HocSinhInBangDTO>(data);

            return ResponseHelper.Ok(hocSinhs);
        }

        /// <summary>
        /// Xác nhận in 
        /// </summary>
        /// <param name="listCCCD"></param> 
        /// <returns></returns>
        [HttpPost("XacNhanInBang")]
        public async Task<IActionResult> XacNhanInBang(string idTruong,string idDanhMucTotNghiep ,List<string> listCCCD)
        {
            var response = await _cacheLayer.XacNhanInBang(idDanhMucTotNghiep, idTruong, listCCCD);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest("Xác nhận in thất bại");
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameHocSinh));
            if (response.MaLoi == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDMTN));
            if (response.MaLoi == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameTruong));
            else
            {
                var deviceTokens = _sysDeviceTokenCL.GetByIdDonVi(idTruong);
                string actionName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ActionName!;
                string controllerName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ControllerName!;
                string action = EString.GenerateActionString(controllerName, actionName);
                var messageConfig = _sysMessageConfigCL.GetByActionName(action);
                if (messageConfig != null)
                {
                    var jsonData = new
                    {
                        IdDanhMucTotNghiep = idDanhMucTotNghiep,
                        IdTruong = idTruong,
                        TrangThai = (int)TrangThaiHocSinhEnum.DaInBang,
                        MaHinhThucDaoTao = response.MaHinhThucDaoTao,
                        MaHeDaoTao = response.MaHeDaoTao,
                        IdNamThi = response.IdNamThi
                    };
                    string jsonString = JsonConvert.SerializeObject(jsonData);
                    MessageInputModel message = new MessageInputModel()
                    {
                        IdMessage = Guid.NewGuid().ToString(),
                        Action = action,
                        MessageType = MessageTypeEnum.TruongGuiPhong.ToStringValue(),
                        SendingMethod = SendingMethodEnum.Notification.ToStringValue(),
                        Title = messageConfig.Title,
                        Content = messageConfig.Body,
                        Color = messageConfig.Color,
                        Recipient = null,
                        Url = messageConfig.URL,
                        IDDonVi = idTruong,
                        ValueRedirect = jsonString
                    };

                    var sendMessage = _messageCL.Save(message);
                    BackgroundJob.Enqueue(() => _backgroundJobManager.SendNotificationInBackground(message.Title, message.Content, deviceTokens));
                }
                return ResponseHelper.Success("Đã xác nhận in thành công");

            }

        }

        [HttpPost("XacNhanInBangTatCa")]
        public async Task<IActionResult> XacNhanInBangTatCa(string idTruong, string idDanhMucTotNghiep)
        {
            var response = await _cacheLayer.XacNhanInBangTatCa(idTruong, idDanhMucTotNghiep);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest("Xác nhận in thất bại");
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameHocSinh));
            if (response.MaLoi == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDMTN));
            if (response.MaLoi == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameTruong));
            else
            {

                var deviceTokens = _sysDeviceTokenCL.GetByIdDonVi(idTruong);
                string actionName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ActionName!;
                string controllerName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ControllerName!;
                string action = EString.GenerateActionString(controllerName, actionName);
                var messageConfig = _sysMessageConfigCL.GetByActionName(action);
                if(messageConfig != null)
                {
                    var jsonData = new
                    {
                        IdDanhMucTotNghiep = idDanhMucTotNghiep,
                        IdTruong = idTruong,
                        TrangThai = (int)TrangThaiHocSinhEnum.DaInBang,
                        MaHinhThucDaoTao = response.MaHinhThucDaoTao,
                        MaHeDaoTao = response.MaHeDaoTao,
                        IdNamThi = response.IdNamThi
                    };
                    string jsonString = JsonConvert.SerializeObject(jsonData);
                    MessageInputModel message = new MessageInputModel()
                    {
                        IdMessage = Guid.NewGuid().ToString(),
                        Action = action,
                        MessageType = MessageTypeEnum.TruongGuiPhong.ToStringValue(),
                        SendingMethod = SendingMethodEnum.Notification.ToStringValue(),
                        Title = messageConfig.Title,
                        Content = messageConfig.Body,
                        Color = messageConfig.Color,
                        Recipient = null,
                        Url = messageConfig.URL,
                        IDDonVi = idTruong,
                        ValueRedirect = jsonString
                    };

                    var sendMessage = _messageCL.Save(message);
                    BackgroundJob.Enqueue(() => _backgroundJobManager.SendNotificationInBackground(message.Title, message.Content, deviceTokens));
                }
                return ResponseHelper.Success("Đã xác nhận in thành công");
            }

        }

        [HttpPost("CapBangTatCa")]
        public async Task<IActionResult> CapBangTatCa(string idTruong, string idDanhMucTotNghiep)
        {
            var response = await _cacheLayer.CapBangTatCa(idTruong, idDanhMucTotNghiep);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetApproveErrorMessage(_nameHocSinh));
            if (response == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameHocSinh));
            else
                return ResponseHelper.Success("Đã cấp bằng thành công");
        }

        [HttpPost("CapBang")]
        public async Task<IActionResult> CapBang(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            var response = await _cacheLayer.CapBang(idTruong, idDanhMucTotNghiep, listCCCD);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetApproveErrorMessage(_nameHocSinh));
            if (response == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameHocSinh));
            else
                return ResponseHelper.Success("Đã cấp bằng thành công");
        }

        private List<HocSinhModel> FilterHocSinhs(List<HocSinhModel> hocSinhs, SearchParamModel model)
        {
            hocSinhs = hocSinhs.Where(hs =>
                hs.HoTen.ToLower().Contains(model.Search.ToLower())).ToList();

            switch (model.Order)
            {
                case "0": // Sắp xếp theo mã học sinh
                    hocSinhs = model.OrderDir == "ASC" ? hocSinhs.OrderBy(hs => hs.STT).ToList() : hocSinhs.OrderByDescending(hs => hs.HoTen).ToList();
                    break;
            }

            return hocSinhs;
        }
    }
}
