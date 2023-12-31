﻿using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.CauHinh;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Processor.ExcelProcess;
using CenIT.DegreeManagement.CoreAPI.Processor.SendNotification;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using CenIT.DegreeManagement.CoreAPI.Validation;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using OfficeOpenXml;
using SharpCompress.Common;
using System.Data;
using static CenIT.DegreeManagement.CoreAPI.Core.Helpers.DataTableValidatorHelper;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DuLieuHocSinh
{
    [Route("api/[controller]")]
    [ApiController]
    public class HocSinhTruongController : BaseAppController
    {
        private HocSinhCL _cacheLayer;
        private DanTocCL _danTocCL;
        private MonThiCL _monThiCL;
        private TruongCL _truong;
        private MessageCL _messageCL;
        private SysMessageConfigCL _sysMessageConfigCL;
        private SysDeviceTokenCL _sysDeviceTokenCL;
        private SysConfigCL _sysConfigCL;

        private DanhMucTotNghiepCL _danhMucTotNghiepCL;
        private readonly FirebaseNotificationUtils _firebaseNotificationUtils;
        private readonly BackgroundJobManager _backgroundJobManager;
        private ILogger<HocSinhTruongController> _logger;
        private readonly IFileService _fileService;
        private readonly IExcelProcess _excelProcess;

        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;
        private readonly AppPaths _appPaths;

        private readonly string _prefixNameFile = "error_import.xlsx";


        public HocSinhTruongController(ICacheService cacheService, IConfiguration configuration, AppPaths appPaths, IExcelProcess excelProcess,
            IFileService fileService, BackgroundJobManager backgroundJobManager, FirebaseNotificationUtils firebaseNotificationUtils, ShareResource shareResource, ILogger<HocSinhTruongController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _danTocCL = new DanTocCL(cacheService, configuration);
            _monThiCL = new MonThiCL(cacheService, configuration);
            _sysDeviceTokenCL = new SysDeviceTokenCL(cacheService);
            _sysConfigCL = new SysConfigCL(cacheService);
            _truong = new TruongCL(cacheService, configuration);
            _sysMessageConfigCL = new SysMessageConfigCL(cacheService);
            _messageCL = new MessageCL(cacheService);
            _backgroundJobManager = backgroundJobManager;
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
            _firebaseNotificationUtils = firebaseNotificationUtils;
            _fileService = fileService;
            _appPaths = appPaths;
            _excelProcess = excelProcess;
        }

        #region Function Main

        /// <summary>
        /// Thêm từng học sinh 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] HocSinhInputModel model)
        {
            var response = await _cacheLayer.Create(model, false);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.HocSinh.ToStringValue()), model.HoTen);
            if (response == (int)HocSinhEnum.ExistCccd)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.HocSinh.ToStringValue(), HocSinhInFoEnum.CCCD.ToStringValue()), model.CCCD);
            if (response == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanhMucTotNghiep.ToStringValue()));
            if (response == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Truong.ToStringValue()));
            if (response == (int)HocSinhEnum.NotExistDanToc)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanToc.ToStringValue()));
            if (response == (int)HocSinhEnum.NotExistKhoaThi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.KhoaThi.ToStringValue()));
            if (response == (int)HocSinhEnum.NotMatchHtdt)
                return ResponseHelper.NotFound("Hình thức đào tạo của trường không khớp với danh mục tốt nghiệp");
            else
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()), model.HoTen);
        }

        /// <summary>
        /// Xóa tất cả học sinh chưa xác nhận theo maTruong
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAll(string idTruong)
        {
            var response = await _cacheLayer.XoaTatCaHocSinhChuaXacNhan(idTruong);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(NameControllerEnum.HocSinh.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()));
        }

        /// <summary>
        /// Xóa học sinh chưa xác nhận theo maTruong
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string idTruong, List<string> listCCCD)
        {
            var response = await _cacheLayer.XoaHocSinhChuaXacNhan(idTruong, listCCCD);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(NameControllerEnum.HocSinh.ToStringValue()));
            else
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()));
        }

        /// <summary>
        /// Lấy thông tin học sinh theo cccd
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetByCccd")]
        public IActionResult GetByCccd(string cccd)
        {
            if (cccd == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()));
            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            return hocSinh != null ? ResponseHelper.Ok(hocSinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()), cccd);
        }

        /// <summary>
        /// Lấy danh sách học sinh by trường (Chưa xác nhận, đang chờ duyệt, đã duyệt, đã đưa vào sổ gốc)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        [HttpGet("GetSearchByTruong/{idTruong}")]
        public IActionResult GetSearchByTruong(string idTruong, [FromQuery] HocSinhParamModel model)
        {
            var data = _cacheLayer.GetSearchHocSinhByTruong(idTruong, model);

            return Ok(ResponseHelper.ResultJson(data));
        }


        /// <summary>
        /// Tổng số học Sinh chưa xác nhận
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        [HttpGet("TongHocSinhChuaXacNhan")]
        public IActionResult TongHocSinhChuaXacNhan(string idDanhMucTotNghiep, string idTruong)
        {
            List<TrangThaiHocSinhEnum> trangThais = new List<TrangThaiHocSinhEnum>() { TrangThaiHocSinhEnum.ChuaXacNhan };
            var hocSinh = _cacheLayer.TongSoHocSinhTheoTrangThai(idDanhMucTotNghiep, idTruong, trangThais);
            return ResponseHelper.Ok(hocSinh);
        }

        /// <summary>
        /// Tổng số học Sinh đã duyệt
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        [HttpGet("TongHocSinhDaDuyet")]
        public IActionResult TongHocSinhDaDuyet(string idDanhMucTotNghiep, string idTruong)
        {
            List<TrangThaiHocSinhEnum> trangThais = new List<TrangThaiHocSinhEnum>() { TrangThaiHocSinhEnum.DaDuyet };
            var hocSinh = _cacheLayer.TongSoHocSinhTheoTrangThai(idDanhMucTotNghiep, idTruong, trangThais);
            return ResponseHelper.Ok(hocSinh);
        }


        /// <summary>
        /// Lấy tất cả danh sách học sinh đã duyệt the id truong và danh mục tốt nghiệp để in bằng tạm thời
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpGet("GetAllHocSinhDaDuyet")]
        public IActionResult GetAllHocSinhDaDuyet(string idTruong, string idDanhMucTotNghiep)
        {
            var hocSinhs = _cacheLayer.GetAllHocSinhDaDuyet(idTruong, idDanhMucTotNghiep);

            return ResponseHelper.Ok(hocSinhs);
        }

        /// <summary>
        /// Lấy thông tin học sinh theo listCCCD
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>

        [HttpPost("GetHocSinhDaDuyetByCCCD")]
        public IActionResult GetHocSinhDaDuyetByCCCD(string idTruong, [FromBody] List<string> listCCCD)
        {
            var hocSinhs = _cacheLayer.GetHocSinhDaDuyetByCCCD(idTruong, listCCCD);

            return ResponseHelper.Ok(hocSinhs);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] HocSinhInputModel model)
        {
            if (model.Id == null)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()));
            }
            var response = await _cacheLayer.Modify(model);
            //Cập nhật thất bại
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.HocSinh.ToStringValue()), model.HoTen);
            //Cập nhật thất bại
            if (response == (int)HocSinhEnum.ExistCccd)
                return ResponseHelper.BadRequest(HocSinhInFoEnum.CCCD.ToStringValue() + _localizer.GetAlreadyExistMessage(NameControllerEnum.HocSinh.ToStringValue()), model.CCCD);
            if (response == (int)HocSinhEnum.NotExist)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response == (int)HocSinhEnum.HocSinhApproved)
                return ResponseHelper.NotFound(_localizer.GetApprovedMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanhMucTotNghiep.ToStringValue()));
            if (response == (int)HocSinhEnum.NotExistDanToc)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanToc.ToStringValue()));
            if (response == (int)HocSinhEnum.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HinhThucDaoTao.ToStringValue()));
            if (response == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Truong.ToStringValue()));
            if (response == (int)HocSinhEnum.NotMatchHtdt)
                return ResponseHelper.NotFound("Hình thức đào tạo của trường không khớp với danh mục tốt nghiệp");
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()), model.HoTen);
        }

        /// <summary>
        /// Xác nhận tất cả học sinh chưa xác nhận theo idTruong
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("ConfirmALL")]
        public async Task<IActionResult> ConfirmALL(string idTruong, string idDanhMucTotNghiep)
        {
            var response = await _cacheLayer.XacNhanTatCaHocSinh(idTruong, idDanhMucTotNghiep);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetConfirmErrorMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response.MaLoi == (int)HocSinhEnum.Approved)
                return ResponseHelper.BadRequest(_localizer.GetApprovedMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response.MaLoi == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanhMucTotNghiep.ToStringValue()));
            if (response.MaLoi == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Truong.ToStringValue()));

            var phong = _truong.GetPhong(idTruong);
            var deviceTokens = _sysDeviceTokenCL.GetByIdDonVi(phong.Id);
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
                    MaHeDaoTao = response.MaHeDaoTao,
                    TrangThai = (int)TrangThaiHocSinhEnum.ChoDuyet
                };
                string jsonString = JsonConvert.SerializeObject(jsonData);

                MessageInputModel message = new MessageInputModel()
                {
                    IdMessage = Guid.NewGuid().ToString(),
                    Action = action,
                    MessageType = MessageTypeEnum.TruongGuiPhong.ToStringValue(),
                    SendingMethod = SendingMethodEnum.Notification.ToStringValue(),
                    Title = messageConfig.Title,
                    Content = string.Format(messageConfig.Body, response.TenTruong),
                    Color = messageConfig.Color,
                    Recipient = null,
                    Url = messageConfig.URL,
                    IDDonVi = phong.Id,
                    ValueRedirect = jsonString
                };

                var sendMessage = _messageCL.Save(message);

                BackgroundJob.Enqueue(() => _backgroundJobManager.SendNotificationInBackground(message.Title, message.Content, deviceTokens));
            }
            if (response.MaLoi == (int)HocSinhEnum.Success && response.DaGui == false)
            {
                await _danhMucTotNghiepCL.CapNhatSoLuongTruongDaGui(idDanhMucTotNghiep);
                return ResponseHelper.Success(_localizer.GetConfirmAllSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()));
            }
            else
                return ResponseHelper.Success(_localizer.GetConfirmAllSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()));
        }

        /// <summary>
        /// Xác nhận học sinh theo mã trường
        /// </summary>
        /// <param name="maTruong"></param>
        /// <param name="listCCCD"></param> 
        /// <returns></returns>
        [HttpPost("Confirm")]
        public async Task<IActionResult> Confirm(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            var response = await _cacheLayer.XacNhanHocSinh(idTruong, idDanhMucTotNghiep, listCCCD);
            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetConfirmErrorMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response.MaLoi == (int)HocSinhEnum.Approved)
                return ResponseHelper.BadRequest(_localizer.GetApprovedMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response.MaLoi == (int)HocSinhEnum.ListEmpty)
                return ResponseHelper.BadRequest(_localizer.GetListEmptyMessage(NameControllerEnum.HocSinh.ToStringValue()));
            if (response.MaLoi == (int)HocSinhEnum.NotExistDanhMucTotNghiep)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.DanhMucTotNghiep.ToStringValue()));
            if (response.MaLoi == (int)HocSinhEnum.NotExistTruong)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.Truong.ToStringValue()));

            var phong = _truong.GetPhong(idTruong);
            var deviceTokens = _sysDeviceTokenCL.GetByIdDonVi(phong.Id);
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
                    MaHeDaoTao = response.MaHeDaoTao,
                    TrangThai = (int)TrangThaiHocSinhEnum.ChoDuyet
                };
                string jsonString = JsonConvert.SerializeObject(jsonData);

                MessageInputModel message = new MessageInputModel()
                {
                    IdMessage = Guid.NewGuid().ToString(),
                    Action = action,
                    MessageType = MessageTypeEnum.TruongGuiPhong.ToStringValue(),
                    SendingMethod = SendingMethodEnum.Notification.ToStringValue(),
                    Title = messageConfig.Title,
                    Content = string.Format(messageConfig.Body, response.TenTruong),
                    Color = messageConfig.Color,
                    Recipient = null,
                    Url = messageConfig.URL,
                    IDDonVi = phong.Id,
                    ValueRedirect = jsonString
                };

                var sendMessage = _messageCL.Save(message);
                BackgroundJob.Enqueue(() => _backgroundJobManager.SendNotificationInBackground(message.Title, message.Content, deviceTokens));
            }

            if (response.MaLoi == (int)HocSinhEnum.Success && response.DaGui == false)
            {
                await _danhMucTotNghiepCL.CapNhatSoLuongTruongDaGui(idDanhMucTotNghiep);
                return ResponseHelper.Success(_localizer.GetConfirmSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()));
            }
            else
                return ResponseHelper.Success(_localizer.GetConfirmSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()));
        }

        #endregion

        /// <summary>
        /// Thêm danh sách học sinh từ excel
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
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
                if(config != null)
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
                if (isAutoGraded)
                {
                    dataFromFile = GraduationType.CalculateGraduationFields(dataFromFile);
                }

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
    }
}
