using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using CenIT.DegreeManagement.CoreAPI.Models.Sys.UserDTO;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Processor.Mail;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CenIT.DegreeManagement.CoreAPI.Controllers.Sys
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseAppController
    {
        private SysUserCL _cacheLayer;
        private ILogger<UserController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;
        private ISendMailService _sendMailService;
        private readonly BackgroundJobManager _backgroundJobManager;
        private TruongCL _truongCl;

        private readonly string _nameController = "User";

        public UserController(ICacheService cacheService, IConfiguration configuration, BackgroundJobManager backgroundJobManager, ShareResource shareResource, ISendMailService sendMailService, ILogger<UserController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _cacheLayer = new SysUserCL(cacheService);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _sendMailService = sendMailService; 
            _backgroundJobManager = backgroundJobManager;
            _truongCl = new TruongCL(cacheService, configuration);

        }

        /// <summary>
        /// Lấy danh sách người dùng (search param)
        /// API: /api/User/GetAllByParams
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByParams")]
        public IActionResult View(string nguoiThucHien, [FromQuery]SearchParamModel model)
        {
            var user = _cacheLayer.GetByUsername(nguoiThucHien);
            string idTruongs = "";
            if (!string.IsNullOrEmpty(user.TruongID) && CheckString.CheckBsonId(user.TruongID))
            {
                var searchParach = new SearchParamModel() { PageSize = -1 };
                int total = 0;
                var donVis = _truongCl.GetSearch(out total, searchParach, user.TruongID).Select(x => x.Id).ToList();
                idTruongs = string.Join(",", donVis);
            }

            var data = _cacheLayer.GetSearch(model, idTruongs);

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy thông tin người dùng theo id
        /// API: /api/User/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Detail(int? id)
        {
            if (id == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()));

            var data = _cacheLayer.GetByID(id);
            if (data != null) return ResponseHelper.Ok(data);

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()));
        }

        /// <summary>
        /// Thêm người dùng
        /// API: /api/User/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromForm]UserCreateDTO model)
        {
            if (model.FileImage != null)
            {
                string folderName = "Users";
                var fileResult = _fileService.SaveImage(model.FileImage, folderName);
                if (fileResult.Item1 > 0)
                {
                    model.Avatar = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            UserInputModel data = ModelProvider.MapModelFromModel<UserCreateDTO, UserInputModel>(model);
            var response = _cacheLayer.Save(data);
            if (response == (int)UserEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.User.ToStringValue()), model.UserName);
            }
            else if (response == (int)UserEnum.ExistUser)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.User.ToStringValue()), model.UserName);
            }
            if (response == (int)UserEnum.ExistEmail)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.User.ToStringValue(), UserInfoEnum.Email.ToStringValue()), model.Email);

            return ResponseHelper.Created(_localizer.GetAddSuccessMessage(NameControllerEnum.User.ToStringValue()), model.UserName);

        }


        /// <summary>
        /// Chỉnh sửa người dùng
        /// API: /api/User/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update([FromForm]UserUpdateDTO model)
        {
            if (model.FileImage != null)
            {
                string folderName = "Users";
                var fileResult = _fileService.SaveImage(model.FileImage, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.Avatar = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }
            if (model.UserId == (int)UserEnum.NotUpdate)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()));
            }

            UserInputModel data = ModelProvider.MapModelFromModel<UserUpdateDTO, UserInputModel>(model);
            var response = _cacheLayer.Save(data);
            if (response == (int)UserEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(NameControllerEnum.User.ToStringValue()), model.UserName);
            }
            else if (response == (int)UserEnum.NotExíst)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()));
            }
            if (response == (int)UserEnum.ExistEmail)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.User.ToStringValue(), UserInfoEnum.Email.ToStringValue()), model.Email);
            return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(NameControllerEnum.User.ToStringValue()), model.UserName);

        }


        /// <summary>
        /// Xóa người dùng
        /// API: /api/User/Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var response = _cacheLayer.Delete(id);
            if (response == (int)UserEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.User.ToStringValue()));
            }
            return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(NameControllerEnum.User.ToStringValue()));

        }

        /// <summary>
        /// Mở hoạt động
        /// API: /api/User/Active
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Active")]
        public IActionResult Active(int id)
        {
            var response = _cacheLayer.Active(id);
            if (response == (int)UserEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetActiveErrorMessage(NameControllerEnum.User.ToStringValue()), id.ToString());
            }
            return ResponseHelper.Success(_localizer.GetActiveSuccessMessage(NameControllerEnum.User.ToStringValue()), id.ToString());
        }


        /// <summary>
        /// Ngưng hoạt động
        /// API: /api/User/DeActive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DeActive")]
        public IActionResult DeActive(int id)
        {
            var response = _cacheLayer.DeActive(id);
            if (response == (int)UserEnum.Fail)
            {
                return ResponseHelper.NotFound(_localizer.GetDeleteErrorMessage(NameControllerEnum.User.ToStringValue()), id.ToString());
            }
            return ResponseHelper.Success(_localizer.GetDeActiveSuccessMessage(NameControllerEnum.User.ToStringValue()), id.ToString());

        }


        /// <summary>
        /// Reset mật khẩu
        /// API: /api/User/ResetPassword
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([CustomRequired] int id)
        {
            var user = _cacheLayer.GetByID(id);
            if (user.UserId > 0)
            {
                var password = PasswordGenerator.GenerateRandomPassword(10);
                var result = _cacheLayer.ResetPassword(id, password);
                if (result > 0)
                {
                    MailContent content = new MailContent
                    {
                        To = user.Email,
                        Subject = "Reset Mật khẩu thành công",
                        Body = $"<p><strong>Xin chào {user.FullName}</strong></p>" +
                       $"<p>Bạn đã yêu cầu đặt lại mật khẩu tại quanlyvanbang.cenit.vn . Mật khẩu của bạn đã được đặt lại thành công.</p>" +
                       $"<p>Thông tin tài khoản của bạn:</p>" +
                       $"<p>Tên đăng nhập: {user.UserName}</p>" +
                       $"<p>Mật khẩu mới: {password}</p>" +
                       $"<p>Vui lòng lưu trữ thông tin này một cách bảo mật. Nếu bạn không thực hiện yêu cầu này, xin hãy liên hệ với chúng tôi ngay lập tức.</p>" 
                    };

                    BackgroundJob.Enqueue(() => _backgroundJobManager.SendEmailInBackground(content));
                    return ResponseHelper.Success(_localizer.GetResetPasswordSuccessMessage(NameControllerEnum.User.ToStringValue()), user.UserName);
                } else
                {
                    return ResponseHelper.Success(_localizer.GetResetPasswordErrorMessage(NameControllerEnum.User.ToStringValue()), user.UserName);
                }
            }
            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()));
        }

        /// <summary>
        /// Lấy danh sách nhóm quyền của người dùng
        /// API: /api/User/GetRolesViaUser
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRolesViaUser/{id}")]
        [AllowAnonymous]
        public IActionResult GetRolesViaUser(int id, [FromQuery] SearchParamModel model)
        {
            var data = _cacheLayer.GetUserRoles(id, model);

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lưu nhóm quyền của user
        /// API: /api/User/SaveUserRole
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveUserRole")]
        public IActionResult SaveUserRole(UserRoleInputModel model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var response = _cacheLayer.SaveUserRole(model);
            if (response > 0)
            {
                return ResponseHelper.Created(_localizer.GetPermissionGroupSuccessMessage());
            }
            else if (response == (int)UserEnum.ExistUser)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.User.ToStringValue()));
            }
            return ResponseHelper.BadRequest(_localizer.GetPermissionGroupErrorMessage());
        }

        /// <summary>
        /// Lấy danh sách báo cáo của người dùng
        /// API: /api/User/GetReportsViaUser/{id}
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReportsViaUser/{id}")]
        [AllowAnonymous]
        public IActionResult GetReportsViaUser(int id, [FromQuery] SearchParamModel model)
        {
            var data = _cacheLayer.GetUserReports(id, model);

            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lưu danh sách báo cáo của người dùng
        /// API: /api/User/SaveUserReport
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveUserReport")]
        [AllowAnonymous]
        public IActionResult SaveUserReport(UserReportInputModel model)
        {

            if (!ModelState.IsValid)
            {
                return Ok();
            }

            var response = _cacheLayer.SaveUserReport(model);
            if (response > 0)
            {
                return ResponseHelper.Created(_localizer.GetPermissionGroupSuccessMessage());
            }
            else if (response == (int)UserEnum.ExistUser)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.User.ToStringValue()));
            }
            return ResponseHelper.BadRequest(_localizer.GetPermissionGroupErrorMessage());
        }
    }
}
