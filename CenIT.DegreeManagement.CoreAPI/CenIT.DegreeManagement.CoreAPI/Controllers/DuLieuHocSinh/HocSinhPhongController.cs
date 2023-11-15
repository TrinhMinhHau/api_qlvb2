using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor.SendNotification;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Resources;
using CenIT.DegreeManagement.CoreAPI.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using static CenIT.DegreeManagement.CoreAPI.Core.Helpers.DataTableValidatorHelper;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using Hangfire;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.CauHinh;
using CenIT.DegreeManagement.CoreAPI.Processor.ExcelProcess;
using ClosedXML.Excel;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DuLieuHocSinh
{
    [Route("api/[controller]")]
    [ApiController]
    public class HocSinhPhongController : BaseAppController
    {
        private HocSinhCL _cacheLayer;
        private PhoiGocCL _phoiGocCL;
        private DanhMucTotNghiepCL _danhMucTotNghiepCL;
        private DanTocCL _danTocCL;
        private MonThiCL _monThiCL;
        private SysMessageConfigCL _sysMessageConfigCL;
        private SysConfigCL _sysConfigCL;
        private readonly FirebaseNotificationUtils _firebaseNotificationUtils;
        private readonly BackgroundJobManager _backgroundJobManager;
        private ILogger<HocSinhPhongController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;
        private readonly AppPaths _appPaths;
        private readonly string _prefixNameFile = "error_import.xlsx";
        private readonly string _nameController = "HocSinh";
        private readonly string _nameDMTN = "DanhMucTotNghiep";
        private readonly string _nameTruong = "Truong";
        private readonly string _nameHTDT = "HinhThucDaoTao";
        private SysDeviceTokenCL _sysDeviceTokenCL;
        private MessageCL _messageCL;
        private readonly IFileService _fileService;
        private readonly IExcelProcess _excelProcess;

        public HocSinhPhongController(ICacheService cacheService, IConfiguration configuration, IExcelProcess excelProcess,
           IFileService fileService, AppPaths appPaths, BackgroundJobManager backgroundJobManager, FirebaseNotificationUtils firebaseNotificationUtils, ShareResource shareResource, ILogger<HocSinhPhongController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _phoiGocCL = new PhoiGocCL(cacheService, configuration);
            _sysDeviceTokenCL = new SysDeviceTokenCL(cacheService);
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService, configuration);
            _danTocCL = new DanTocCL(cacheService, configuration);
            _monThiCL = new MonThiCL(cacheService, configuration);
            _sysMessageConfigCL = new SysMessageConfigCL(cacheService);
            _firebaseNotificationUtils = firebaseNotificationUtils;
            _backgroundJobManager = backgroundJobManager;
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
            _messageCL = new MessageCL(cacheService);
            _sysConfigCL = new SysConfigCL(cacheService);
            _appPaths = appPaths;
            _fileService = fileService;
            _excelProcess = excelProcess;
        }

        /// <summary>
        /// Thêm từng học sinh 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] HocSinhInputModel model)
        {
            var response = await _cacheLayer.Create(model, true);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(_nameController), model.HoTen);
            if (response == (int)HocSinhEnum.ExistCccd)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage("CCCD " + _nameController), model.HoTen);
            if (response == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("DanhMucTotNghiep"));
            if (response == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("Truong"));
            if (response == (int)HocSinhEnum.NotExistKhoaThi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("KhoaThi"));
            if (response == (int)HocSinhEnum.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameHTDT));
            if (response == (int)HocSinhEnum.NotMatchHtdt)
                return ResponseHelper.NotFound("Hình thức đào tạo của trường không khớp với danh mục tốt nghiệp");
            else
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(_nameController), model.HoTen);
        }

        /// <summary>
        /// Cập nhật học sinh
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] HocSinhInputModel model)
        {
            if (model.Id == null)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            }
            var response = await _cacheLayer.Modify(model);
            //Cập nhật thất bại
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(_nameController), model.HoTen);
            //Cập nhật thất bại
            if (response == (int)HocSinhEnum.ExistCccd)
                return ResponseHelper.BadRequest(HocSinhInFoEnum.CCCD.ToStringValue() + _localizer.GetAlreadyExistMessage(_nameController), model.CCCD);
            if (response == (int)HocSinhEnum.NotExist)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            if (response == (int)HocSinhEnum.HocSinhApproved)
                return ResponseHelper.NotFound(_localizer.GetApprovedMessage(_nameController));
            if (response == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("DanhMucTotNghiep"));
            if (response == (int)HocSinhEnum.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("HinhThucDaoTao"));
            if (response == (int)HocSinhEnum.NotMatchHtdt)
                return ResponseHelper.NotFound("Hình thức đào tạo của trường không khớp với danh mục tốt nghiệp");
            if (response == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("Truong"));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(_nameController), model.HoTen);
        }

        /// <summary>
        /// Lấy danh sách học sinh  chờ duyệt
        /// API của phòng
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetSearchByPhong")]
        public IActionResult GetSearchByPhong([FromQuery] string? idTruong, [FromQuery] HocSinhParamModel model)
        {
            var data = _cacheLayer.GetSearchHocSinhChoDuyetByPhong(idTruong, model);

            return Ok(ResponseHelper.ResultJson(data));
        }

        [HttpGet("GetByCccd")]
        [AllowAnonymous]
        public IActionResult GetByCccd(string cccd)
        {
            if (cccd == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            return hocSinh != null ? ResponseHelper.Ok(hocSinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController), cccd);
        }

        /// <summary>
        /// Duyệt tất cả học sinh theo trường và danh mục tốt nghiêp
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("ApproveAll")]
        public async Task<IActionResult> ApproveAll(string idTruong, string idDanhMucTotNghiep)
        {

            var response = await _cacheLayer.DuyetTatCaHocSinh(idTruong, idDanhMucTotNghiep);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetApproveAllErrorMessage(_nameController));
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameController));
            if (response.MaLoi == (int)HocSinhEnum.ExceedsPhoiGocLimit)
                return ResponseHelper.BadRequest(_localizer.GetExceedsPhoiGocLimitMessage());
            if (response.MaLoi == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDMTN));
            if (response.MaLoi == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameTruong));
            if (response.MaLoi == (int)HocSinhEnum.NotExistPhoi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("PhoiGoc"));

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
                    TrangThai = (int)TrangThaiHocSinhEnum.DaDuyet
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
            if (response.MaLoi == (int)HocSinhEnum.Success && response.DaDuyet == false)
            {
                await _danhMucTotNghiepCL.CapNhatSoLuongHocSinh(idDanhMucTotNghiep, response.SoluongHocSinh);
                await _danhMucTotNghiepCL.CapNhatSoLuongTruongDaDuyet(idDanhMucTotNghiep);
                await _phoiGocCL.CapNhatThongSoPhoi(response.SoBatDau, response.IdPhoi, response.SoluongHocSinh);

                return ResponseHelper.Success(_localizer.GetApproveAllSuccessMessage(_nameController));
            }
            else
                await _danhMucTotNghiepCL.CapNhatSoLuongHocSinh(idDanhMucTotNghiep, response.SoluongHocSinh);
                await _phoiGocCL.CapNhatThongSoPhoi(response.SoBatDau, response.IdPhoi, response.SoluongHocSinh);
            return ResponseHelper.Success(_localizer.GetApproveAllSuccessMessage(_nameController));
        }

        [HttpPost("Approve")]
        [AllowAnonymous]
        public async Task<IActionResult> Approve(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            var response = await _cacheLayer.DuyetDanhSachHocSinh(idTruong, idDanhMucTotNghiep, listCCCD);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetApproveErrorMessage(_nameController));
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameController));
            if (response.MaLoi == (int)HocSinhEnum.ExceedsPhoiGocLimit)
                return ResponseHelper.BadRequest(_localizer.GetExceedsPhoiGocLimitMessage());
            if (response.MaLoi == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameDMTN));
            if (response.MaLoi == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameTruong));
            if (response.MaLoi == (int)HocSinhEnum.NotExistPhoi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("PhoiGoc"));

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
                    TrangThai = (int)TrangThaiHocSinhEnum.DaDuyet
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
            
            if (response.MaLoi == (int)HocSinhEnum.Success && response.DaDuyet == false)
            {
                await _danhMucTotNghiepCL.CapNhatSoLuongHocSinh(idDanhMucTotNghiep, response.SoluongHocSinh);
                await _danhMucTotNghiepCL.CapNhatSoLuongTruongDaDuyet(idDanhMucTotNghiep);
                await _phoiGocCL.CapNhatThongSoPhoi(response.SoBatDau, response.IdPhoi, response.SoluongHocSinh);

                return ResponseHelper.Success(_localizer.GetApproveSuccessMessage(_nameController));
            }
            else
                await _danhMucTotNghiepCL.CapNhatSoLuongHocSinh(idDanhMucTotNghiep, response.SoluongHocSinh);
                await _phoiGocCL.CapNhatThongSoPhoi(response.SoBatDau, response.IdPhoi, response.SoluongHocSinh);
            return ResponseHelper.Success(_localizer.GetApproveAllSuccessMessage(_nameController));
        }

        /// <summary>
        /// Trả lại danh sách 
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpPost("GiveBackAll")]
        public async Task<IActionResult> GiveBackAll(string idTruong, string idDanhMucTotNghiep)
        {
            var response = await _cacheLayer.TraLaiTatCaHocSinh(idTruong, idDanhMucTotNghiep);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetGiveBackErrorMessage(_nameController));
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameController));
            await _danhMucTotNghiepCL.CapNhatSoLuongTruongDaGui(idDanhMucTotNghiep, true);

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
                    TrangThai = (int)TrangThaiHocSinhEnum.ChuaXacNhan
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
            if (response.MaLoi == (int)HocSinhEnum.Success && response.DaGui == false)
                await _danhMucTotNghiepCL.CapNhatSoLuongTruongDaGui(idDanhMucTotNghiep, true);
            return ResponseHelper.Success(_localizer.GetGiveBackSuccessMessage(_nameController));
        }

        /// <summary>
        /// Trả lại danh sách 
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpPost("GiveBack")]
        [AllowAnonymous]
        public async Task<IActionResult> GiveBack(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            var response = await _cacheLayer.TraLaiDanhSachHocSinh(idTruong, idDanhMucTotNghiep, listCCCD);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetGiveBackErrorMessage(_nameController));
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(_nameController));
           

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
                    TrangThai = (int)TrangThaiHocSinhEnum.ChuaXacNhan
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

            if (response.MaLoi == (int)HocSinhEnum.Success && response.DaGui == false)
                await _danhMucTotNghiepCL.CapNhatSoLuongTruongDaGui(idDanhMucTotNghiep, true);
            return ResponseHelper.Success(_localizer.GetGiveBackSuccessMessage(_nameController));
        }

        [HttpPost("ImportHocSinh")]
        [AllowAnonymous]
        public async Task<IActionResult> ImportHocSinh([FromForm] HocSinhImportModel model)
        {
            try
            {

                var deleteFileError = _fileService.DeleteFile(model.NguoiThucHien + "_" + _prefixNameFile, _appPaths.UploadFileExcelError_HocSinhTruong);
                var deleteFileSuccess = _fileService.DeleteFile(model.NguoiThucHien + "_" + _prefixNameFile, _appPaths.UploadFileExcelSuccess_HocSinhTruong);

                if (deleteFileError == 0 && deleteFileSuccess == 0)
                {
                    return ResponseHelper.BadRequest("Error Delete file");
                }

                //Kiểm tra định dạng file excel
                string[] allowedExtensions = { ".xlsx", ".csv", ".xls" };
                var checkFile = FileHelper.CheckValidFile(model.FileExcel, allowedExtensions);
                if (checkFile.Item1 == (int)ErrorFileEnum.InvalidFileImage)
                {
                    return ResponseHelper.BadRequest(checkFile.Item2);
                }

                //Đọc file excel
                DataTable dataFromFile = _excelProcess.ReadExcelData(model.FileExcel);

                var monThis = _monThiCL.GetAllMaMonThi();
                var danTocs = _danTocCL.GetAllTenDanToc();
                var arrCCCD = _cacheLayer.GetAllArrCCCD();
                var config = _sysConfigCL.GetConfigByKey(CodeConfigEnum.AutomaticallyGraded.ToStringValue());
                bool isAutoGraded = false;
                if (config != null)
                {
                    isAutoGraded = config.ConfigValue == "true" ? true : false;
                }

                //// Kiểm tra dữ liệu của file excel
                var validationRules = HocSinhValidationRules.GetRulesHocSinhNew(monThis, danTocs, arrCCCD, isAutoGraded);
                var checkValue = CheckValidDataTable(dataFromFile, validationRules);

                if (checkValue.Item2 > 0)
                {
                    var columnConfigs = CreateExcelColumnConfigs();
                    // lưu file error excel và trả về đường dẫn
                    string excelFilePath = _excelProcess.SaveDataTableErrorToExcel(checkValue.Item1, model.NguoiThucHien, _prefixNameFile
                      , columnConfigs, _appPaths.UploadFileExcelError_HocSinhTruong, _appPaths.GetFileExcelError_HocSinhTruong);
                    var outputData = new
                    {
                        Path = excelFilePath,
                    };
                    return ResponseHelper.BadRequestHaveData("Thêm danh sách học sinh thất bại. Vui lòng tải tệp lỗi để kiểm tra và thử lại!", outputData);
                }

                //Xếp loại tự động
                dataFromFile = GraduationType.CalculateGraduationFields(dataFromFile);

                //Map từ datatable sang List<HocSinhImportViewModel>
                List<HocSinhImportViewModel> hocSinhImports = ModelProvider.CreateListFromTable<HocSinhImportViewModel>(dataFromFile);
                hocSinhImports.ForEach(hocSinh =>
                {
                    hocSinh.IdTruong = model.IdTruong;
                    hocSinh.IdDanhMucTotNghiep = model.IdDanhMucTotNghiep;
                    hocSinh.NgayTao = DateTime.Now;
                    hocSinh.NguoiTao = model.NguoiThucHien;
                    hocSinh.IdKhoaThi = model.IdKhoaThi;
                    hocSinh.KetQuaHocTaps = new List<KetQuaHocTapModel>
                    {
                        new KetQuaHocTapModel { MaMon = hocSinh.MonNV, Diem = hocSinh.DiemMonNV ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.MonTO, Diem = hocSinh.DiemMonTO ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon3, Diem = hocSinh.DiemMon3 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon4, Diem = hocSinh.DiemMon4 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon5, Diem = hocSinh.DiemMon5 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon6, Diem = hocSinh.DiemMon6 ?? 0 }
                    };
                });

                List<HocSinhModel> hocSinhs = _mapper.Map<List<HocSinhModel>>(hocSinhImports);
                // gọi cache
                var response = await _cacheLayer.ImportHocSinh(hocSinhs, model.IdTruong, model.IdDanhMucTotNghiep);
                var columnConfigSucces = CreateExcelColumnSuccess();
                // lưu file error excel và trả về đường dẫn
                string excelPath = _excelProcess.SaveDataTableErrorToExcel(dataFromFile, model.NguoiThucHien, _prefixNameFile
                  , columnConfigSucces, _appPaths.UploadFileExcelSuccess_HocSinhTruong, _appPaths.GetFileExcelSuccess_HocSinhTruong);

                var data = new
                {
                    Path = excelPath,
                };

                if (response.ErrorCode == (int)HocSinhEnum.Fail)
                    return ResponseHelper.BadRequestHaveData("Thêm danh sách học sinh thất bại", data);
                else
                    return ResponseHelper.SuccessHaveData("Thêm danh sách học sinh thành công", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResponseHelper.Error500(ex.Message);
            }
        }

        private List<ExcelColumnConfig> CreateExcelColumnConfigs()
        {
            var columnConfigs = new List<ExcelColumnConfig>
            {
                new ExcelColumnConfig { Label = "STT", Description = "Số thứ tự", Danger=true},
                new ExcelColumnConfig { Label = "HoTen", Description = "Họ và tên", Danger=true },
                new ExcelColumnConfig { Label = "CCCD", Description = "CCCD", Danger=true },
                new ExcelColumnConfig { Label = "GioiTinh", Description = "Giới tính", Danger=true },
                new ExcelColumnConfig { Label = "NgaySinh", Description = "Ngày sinh", Danger=true },
                new ExcelColumnConfig { Label = "NoiSinh", Description = "Nơi sinh", Danger=true },
                new ExcelColumnConfig { Label = "DanToc", Description = "Dân Tộc", Danger=true },
                new ExcelColumnConfig { Label = "DiaChi", Description = "Địa chỉ", Danger=true },
                new ExcelColumnConfig { Label = "SoHieuVanBang", Description = "Số hiệu văn bằng", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "SoVaoSoCapBang", Description = "Số vào sổ cấp bằng", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "HoiDongThi", Description = "Hội đồng thi", Danger=true },
                new ExcelColumnConfig { Label = "Lop", Description = "Lớp", Danger=true },
                new ExcelColumnConfig { Label = "HanhKiem", Description = "Hạnh Kiểm", Danger=true },
                new ExcelColumnConfig { Label = "DiemTB", Description = "Điểm Trung Bình", Danger=true },
                new ExcelColumnConfig { Label = "HocLuc", Description = "Học Lực", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "KetQua", Description = "Kết quả tốt nghiệp", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "XepLoai", Description = "Xếp loại tốt nghiệp", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "LanDauTotNghiep", Description = "Lần đầu tốt nghiệp" },
                new ExcelColumnConfig { Label = "MonNV", Description = "Môn Ngữ văn" },
                new ExcelColumnConfig { Label = "MonTO", Description = "Môn Toán" },
                new ExcelColumnConfig { Label = "Mon3", Description = "Môn 3" },
                new ExcelColumnConfig { Label = "Mon4", Description = "Môn 4" },
                new ExcelColumnConfig { Label = "Mon5", Description = "Môn 5" },
                new ExcelColumnConfig { Label = "Mon6", Description = "Môn 6" },
                new ExcelColumnConfig { Label = "DiemMonNV", Description = "Điểm môn NV" },
                new ExcelColumnConfig { Label = "DiemMonTO", Description = "Điểm môn Toán" },
                new ExcelColumnConfig { Label = "DiemMon3", Description = "Điểm môn 3" },
                new ExcelColumnConfig { Label = "DiemMon4", Description = "Điểm môn 4" },
                new ExcelColumnConfig { Label = "DiemMon5", Description = "Điểm môn 5" },
                new ExcelColumnConfig { Label = "DiemMon6", Description = "Điểm môn 6" },
                new ExcelColumnConfig { Label = "DiemKK", Description = "Điểm khuyến khích" },
                new ExcelColumnConfig { Label = "DienXT", Description = "Diện xét tuyển" },
                new ExcelColumnConfig { Label = "DiemXTN", Description = "Điểm xét tốt nghiệp" },
                new ExcelColumnConfig { Label = "MaDKThi", Description = "Mã đăng ký thi" },
                new ExcelColumnConfig { Label = "TenDKThi", Description = "Tên đăng ký thi" },
                new ExcelColumnConfig { Label = "GhiChu", Description = "Ghi chú" },
                new ExcelColumnConfig { Label = "Message", Description = "Thông báo lỗi" },

            };

            return columnConfigs;
        }

        private List<ExcelColumnConfig> CreateExcelColumnSuccess()
        {
            var columnConfigs = new List<ExcelColumnConfig>
            {
                new ExcelColumnConfig { Label = "STT", Description = "Số thứ tự", Danger=true},
                new ExcelColumnConfig { Label = "HoTen", Description = "Họ và tên", Danger=true },
                new ExcelColumnConfig { Label = "CCCD", Description = "CCCD", Danger=true },
                new ExcelColumnConfig { Label = "GioiTinh", Description = "Giới tính", Danger=true },
                new ExcelColumnConfig { Label = "NgaySinh", Description = "Ngày sinh", Danger=true },
                new ExcelColumnConfig { Label = "NoiSinh", Description = "Nơi sinh", Danger=true },
                new ExcelColumnConfig { Label = "DanToc", Description = "Dân Tộc", Danger=true },
                new ExcelColumnConfig { Label = "DiaChi", Description = "Địa chỉ", Danger=true },
                new ExcelColumnConfig { Label = "SoHieuVanBang", Description = "Số hiệu văn bằng", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "SoVaoSoCapBang", Description = "Số vào sổ cấp bằng", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "HoiDongThi", Description = "Hội đồng thi", Danger=true },
                new ExcelColumnConfig { Label = "Lop", Description = "Lớp", Danger=true },
                new ExcelColumnConfig { Label = "HanhKiem", Description = "Hạnh Kiểm", Danger=true },
                new ExcelColumnConfig { Label = "DiemTB", Description = "Điểm Trung Bình", Danger=true },
                new ExcelColumnConfig { Label = "HocLuc", Description = "Học Lực", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "KetQua", Description = "Kết quả tốt nghiệp", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "XepLoai", Description = "Xếp loại tốt nghiệp", BackgroundYellow = true },
                new ExcelColumnConfig { Label = "LanDauTotNghiep", Description = "Lần đầu tốt nghiệp" },
                new ExcelColumnConfig { Label = "MonNV", Description = "Môn Ngữ văn" },
                new ExcelColumnConfig { Label = "MonTO", Description = "Môn Toán" },
                new ExcelColumnConfig { Label = "Mon3", Description = "Môn 3" },
                new ExcelColumnConfig { Label = "Mon4", Description = "Môn 4" },
                new ExcelColumnConfig { Label = "Mon5", Description = "Môn 5" },
                new ExcelColumnConfig { Label = "Mon6", Description = "Môn 6" },
                new ExcelColumnConfig { Label = "DiemMonNV", Description = "Điểm môn NV" },
                new ExcelColumnConfig { Label = "DiemMonTO", Description = "Điểm môn Toán" },
                new ExcelColumnConfig { Label = "DiemMon3", Description = "Điểm môn 3" },
                new ExcelColumnConfig { Label = "DiemMon4", Description = "Điểm môn 4" },
                new ExcelColumnConfig { Label = "DiemMon5", Description = "Điểm môn 5" },
                new ExcelColumnConfig { Label = "DiemMon6", Description = "Điểm môn 6" },
                new ExcelColumnConfig { Label = "DiemKK", Description = "Điểm khuyến khích" },
                new ExcelColumnConfig { Label = "DienXT", Description = "Diện xét tuyển" },
                new ExcelColumnConfig { Label = "DiemXTN", Description = "Điểm xét tốt nghiệp" },
                new ExcelColumnConfig { Label = "MaDKThi", Description = "Mã đăng ký thi" },
                new ExcelColumnConfig { Label = "TenDKThi", Description = "Tên đăng ký thi" },
                new ExcelColumnConfig { Label = "GhiChu", Description = "Ghi chú" },
            };

            return columnConfigs;
        }

        /// <summary>
        /// Tổng số học Sinh chờ duyệt
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        [HttpGet("TongHocSinhChoDuyet")]
        public IActionResult TongHocSinhChoDuyet(string idDanhMucTotNghiep, string idTruong)
        {
            List<TrangThaiHocSinhEnum> trangThais = new List<TrangThaiHocSinhEnum>() { TrangThaiHocSinhEnum.ChoDuyet };
            var hocSinh = _cacheLayer.TongSoHocSinhTheoTrangThai(idDanhMucTotNghiep, idTruong, trangThais);
            return ResponseHelper.Ok(hocSinh);
        }
    }
}
