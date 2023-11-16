using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DuLieuHocSinh
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucTotNghiepController : BaseAppController
    {
        private DanhMucTotNghiepCL _cacheLayer;
        private NamThiCL _namThiCL;
        private TruongCL _truongCL;
        private SysUserCL _sysUserCL;
        private SysDeviceTokenCL _sysDeviceTokenCL;
        private SysMessageConfigCL _sysMessageConfigCL;
        private MessageCL _messageCL;


        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly IMapper _mapper;
        private ILogger<DanhMucTotNghiepController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;


        private readonly string _nameController = "Danh mục tốt nghiệp";

        public DanhMucTotNghiepController(ICacheService cacheService,IConfiguration configuration,
            ShareResource shareResource, ILogger<DanhMucTotNghiepController> logger, IFileService fileService, BackgroundJobManager backgroundJobManager, IMapper imapper) : base(cacheService, configuration)
        {
            _cacheLayer = new DanhMucTotNghiepCL(cacheService, configuration);
            _namThiCL = new NamThiCL(cacheService, configuration);
            _truongCL = new TruongCL(cacheService, configuration);
            _sysUserCL = new SysUserCL(cacheService);
            _sysMessageConfigCL = new SysMessageConfigCL(cacheService);
            _sysDeviceTokenCL = new SysDeviceTokenCL(cacheService);
            _messageCL = new MessageCL(cacheService);
            _mapper = imapper;
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _backgroundJobManager = backgroundJobManager;
        }

        #region DanhMucTotNghiep

        /// <summary>
        /// Lấy tất cả danh mục tốt nghiệp
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAll();
            var tongSoTruong = _truongCL.GetAll().Count();
            data.ForEach(danhMucTotNghep =>
            {
                danhMucTotNghep.TongSoTruong = tongSoTruong;
            });
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy tất cả danh mục tốt nghiệp theo search param
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        [AllowAnonymous]
        public IActionResult GetSearch([FromQuery] DanhMucTotNghiepSearchParam model)
        {
            int total;
            var data = _cacheLayer.GetSearch(out total, model);
            var namThi = _namThiCL.GetAll();
            var tongSoTruong = _truongCL.GetAll().Count();
            if(namThi != null)
            {
                data.ForEach(danhMucTotNghep =>
                {
                    danhMucTotNghep.TongSoTruong = tongSoTruong;
                    danhMucTotNghep.NamThi = namThi.FirstOrDefault(d => d.Id == danhMucTotNghep.IdNamThi).Ten;
                });
            }
          
            var outputData = new
            {
                DanhMucTotNghieps = data,
                totalRow = total,
                searchParam = model,
            };
            return ResponseHelper.Ok(outputData);
        }


        /// <summary>
        /// Lấy tất cả danh mục tốt nghiệp chưa khóa
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUnBlock")]
        public IActionResult GetAllUnBlock()
        {
            var data = _cacheLayer.GetAllUnBlock();
            var tongSoTruong = _truongCL.GetAll().Count();
            data.ForEach(danhMucTotNghep =>
            {
                danhMucTotNghep.TongSoTruong = tongSoTruong;
            });
            return ResponseHelper.Ok(data);
        }


        /// <summary>
        /// Thêm danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] DanhMucTotNghiepInputModel model)
        {
            var response = await _cacheLayer.Create(model);
            if (response == (int)EnumDanhMucTotNghiep.Fail) 
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(_nameController), model.TieuDe);
            if (response == (int)EnumDanhMucTotNghiep.YearNotMatchDate)
                return ResponseHelper.BadRequest("Năm thi không khớp với ngày cấp bằng");
            if (response == (int)EnumDanhMucTotNghiep.ExistName) 
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(_nameController, EnumExtensions.ToStringValue(DanhMucTotNghiepInfoEnum.TieuDe)), model.TieuDe);
            if (response == (int)EnumDanhMucTotNghiep.ExistYearAndHTDT)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(_nameController, EnumExtensions.ToStringValue(DanhMucTotNghiepInfoEnum.IdNamVaIdHTDT)), model.IdNamThi + "," + model.IdHinhThucDaoTao);
            if (response == (int)EnumDanhMucTotNghiep.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("HTDT "));
            if (response == (int)EnumDanhMucTotNghiep.NotExistNamThi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("NamThi "));
            else 
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(_nameController), model.TieuDe);
        }

        /// <summary>
        /// Cập nhật danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] DanhMucTotNghiepInputModel model)
        {
            var response = await _cacheLayer.Modify(model);
            if (response == (int)EnumDanhMucTotNghiep.Fail) 
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(_nameController), model.TieuDe);
            if (response == (int)EnumDanhMucTotNghiep.ExistName) 
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage("Tiêu đề " + _nameController), model.TieuDe);
            if (response == (int)EnumDanhMucTotNghiep.YearNotMatchDate)
                return ResponseHelper.BadRequest("Năm thi không khớp với ngày cấp bằng");
            if (response == (int)EnumDanhMucTotNghiep.NotFound) 
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            if (response == (int)EnumDanhMucTotNghiep.Locked) 
                return ResponseHelper.BadRequest("Đã khóa");
            if (response == (int)EnumDanhMucTotNghiep.Printed) 
                return ResponseHelper.BadRequest("Đã In bằng");
            if (response == (int)EnumDanhMucTotNghiep.ExistYearAndHTDT)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage("Năm học " + _nameController));
            if (response == (int)EnumDanhMucTotNghiep.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("HTDT "));
            if (response == (int)EnumDanhMucTotNghiep.NotExistNamThi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("NamThi "));
            else 
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(_nameController), model.TieuDe);
        }

        /// <summary>
        /// Xóa danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhmucTotNghiep"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string idDanhmucTotNghiep, string nguoiThucHien)
        {
            var response = await _cacheLayer.Delete(idDanhmucTotNghiep, nguoiThucHien);
            if (response == (int)EnumDanhMucTotNghiep.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(_nameController));
            if (response == (int)EnumDanhMucTotNghiep.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            if (response == (int)EnumDanhMucTotNghiep.Locked)
                return ResponseHelper.BadRequest("Đã khóa");
            if (response == (int)EnumDanhMucTotNghiep.Printed)
                return ResponseHelper.BadRequest("Đã In bằng");
            else 
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(_nameController));
        }

        /// <summary>
        /// Lấy danh mục tốt nghiệp theo idDanhMucTotNghiep
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpGet("GetById/{idDanhMucTotNghiep}")]
        [AllowAnonymous]
        public IActionResult GetById(string idDanhMucTotNghiep)
        {
            var data = _cacheLayer.GetById(idDanhMucTotNghiep);
            var tongSoTruong = _truongCL.GetAll(data.IdHinhThucDaoTao).Count();

            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));

            data.TongSoTruong = tongSoTruong;

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Khóa danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpPost("Lock")]
        public async Task<IActionResult> Lock(string idDanhMucTotNghiep, string nguoiThucHien)
        {
            var response = await _cacheLayer.KhoaDanhMucTotNghiep(idDanhMucTotNghiep, nguoiThucHien, true);
            if(response == (int)EnumDanhMucTotNghiep.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));

            return ResponseHelper.Success(string.Format("Khóa [{0}] thành công", _nameController));
        }

        /// <summary>
        /// Mở khóa danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpPost("UnLock")]
        public async Task<IActionResult> UnLock(string idDanhMucTotNghiep, string nguoiThucHien)
        {
            var response = await _cacheLayer.KhoaDanhMucTotNghiep(idDanhMucTotNghiep, nguoiThucHien, false);
            if (response == (int)EnumDanhMucTotNghiep.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));

            return ResponseHelper.Success(string.Format("Mở khóa [{0}] thành công", _nameController));
        }
        #endregion

        #region DanhMucTotNghiep via truong
        [HttpPost("CreateDanhMucTotNghiepViaTruong")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDanhMucTotNghiepViaTruong([FromBody] DanhMucTotNghiepViaTruongInputModel model)
        {
            var response = await _cacheLayer.CreateDanhMucTotNghiepViaTruong(model);
            if (response == (int)EnumDanhMucTotNghiep.Success)
                return ResponseHelper.Success("Phân quyền trường thành công");
            if (response == (int)EnumDanhMucTotNghiep.NotFound)
                return ResponseHelper.Success("Danh mục tốt nghiệp không tồn tại");
            if (response == (int)EnumDanhMucTotNghiep.NotFoundTruong)
                return ResponseHelper.Success("Trường không tồn tại");
            else
                return ResponseHelper.BadRequest("Phân quyền trường thất bại");
        }

        [HttpGet("GetAllTruong")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTruong(string idDanhMucTotNghiep, string nguoiThucHien)
        {
            var user = _sysUserCL.GetByUsername(nguoiThucHien);
            if (!string.IsNullOrEmpty(user.TruongID) && CheckString.CheckBsonId(user.TruongID))
            {
               var response =  _cacheLayer.GetTruong(user.TruongID, idDanhMucTotNghiep);
                var dataMapper = _mapper.Map<List<TruongHocViaDanhMucTotNghiepDTO>>(response);

                return ResponseHelper.Ok(dataMapper);

            }

            return ResponseHelper.BadRequest("Người thực hiện không thuộc đơn vị nào");
        }

        [HttpGet("GetTruongHasPermision")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTruongHasPermision(string idDanhMucTotNghiep)
        {
         
            var response = _cacheLayer.GetTruongHasPermision(idDanhMucTotNghiep);
            var dataMapper = _mapper.Map<List<TruongHocViaDMTNHasPermisionDTO>>(response);

            return ResponseHelper.Ok(dataMapper);
        }

        [HttpPost("GuiThongBaoTungTruong")]
        [AllowAnonymous]
        public async Task<IActionResult> GuiThongBaoTungTruong([FromBody]string idTruong, string noiDung)
        {
            var deviceTokens = _sysDeviceTokenCL.GetByIdDonVi(idTruong);
            string actionName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ActionName!;
            string controllerName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ControllerName!;
            string action = EString.GenerateActionString(controllerName, actionName);
            var messageConfig = _sysMessageConfigCL.GetByActionName(action);
            if (messageConfig != null)
            {
                MessageInputModel message = new MessageInputModel()
                {
                    IdMessage = Guid.NewGuid().ToString(),
                    Action = action,
                    MessageType = MessageTypeEnum.TruongGuiPhong.ToStringValue(),
                    SendingMethod = SendingMethodEnum.Notification.ToStringValue(),
                    Title = messageConfig.Title,
                    Content = string.IsNullOrEmpty(noiDung) ?  messageConfig.Body : noiDung,
                    Color = messageConfig.Color,
                    Recipient = null,
                    Url = messageConfig.URL,
                    IDDonVi = idTruong,
                };

                var sendMessage = _messageCL.Save(message);

                BackgroundJob.Enqueue(() => _backgroundJobManager.SendNotificationInBackground(message.Title, message.Content, deviceTokens));
                return ResponseHelper.Success("Gửi thông báo thành công");
            }

            return ResponseHelper.BadRequest("Gửi thông báo thất bạo");

        }


        [HttpPost("GuiThongBaoNhieuTruong")]
        [AllowAnonymous]
        public async Task<IActionResult> GuiThongBaoNhieuTruong([FromBody]string idTruongs, string noiDung)
        {
            var deviceTokens = _sysDeviceTokenCL.GetByIdDonVis(idTruongs);
            string actionName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ActionName!;
            string controllerName = HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ControllerName!;
            string action = EString.GenerateActionString(controllerName, actionName);
            var messageConfig = _sysMessageConfigCL.GetByActionName(action);
            if (messageConfig != null)
            {
                deviceTokens.ForEach(x=>
                {
                    MessageInputModel message = new MessageInputModel()
                    {
                        IdMessage = Guid.NewGuid().ToString(),
                        Action = action,
                        MessageType = MessageTypeEnum.TruongGuiPhong.ToStringValue(),
                        SendingMethod = SendingMethodEnum.Notification.ToStringValue(),
                        Title = messageConfig.Title,
                        Content = string.IsNullOrEmpty(noiDung) ? messageConfig.Body : noiDung,
                        Color = messageConfig.Color,
                        Recipient = null,
                        Url = messageConfig.URL,
                        IDDonVi = x.TruongId,
                    };

                    var sendMessage = _messageCL.Save(message);
                    BackgroundJob.Enqueue(() => _backgroundJobManager.SendNotificationInBackground(message.Title, message.Content, x.DeviceTokens));
                });



                return ResponseHelper.Success("Gửi thông báo thành công");
            }

            return ResponseHelper.BadRequest("Gửi thông báo thất bạo");

        }


        #endregion

    }
}