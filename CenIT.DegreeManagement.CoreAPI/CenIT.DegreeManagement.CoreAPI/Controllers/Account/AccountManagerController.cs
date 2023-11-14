using CenIT.DegreeManagement.CoreAPI.Attributes;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Account;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Account;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Models.Account;
using CenIT.DegreeManagement.CoreAPI.Models.Sys.UserDTO;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountManagerController : BaseAppController
    {
        private ILogger<AccountManagerController> _logger;
        private SysUserCL _sysUserCL;
        private TruongCL _truongCL;
        private readonly IFileService _fileService;
        private readonly ShareResource _localizer;
        public AccountManagerController(ICacheService cacheService, IConfiguration configuration, ShareResource localizer, IFileService fileService, ILogger<AccountManagerController> logger) : base(cacheService, configuration)
        {
            _logger = logger;
            _localizer = localizer;
            _sysUserCL = new SysUserCL(cacheService);
            _fileService = fileService;
            _truongCL = new TruongCL(cacheService, configuration);
        }

        /// <summary>
        /// Thay đổi mật khẩu
        /// API: /api/AccountManager/ChangePassword
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePassword")]
        [AllowAnyPermission]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var userLoginInfo = HttpContext.Items["UserLoginInfo"] as UserLoginInfoModel;
            if (userLoginInfo == null)
                return ResponseHelper.Unauthorized(_localizer.GetInvalidTokenMessage());

            AccountInputModel data = ModelProvider.MapModelFromModel<ChangePasswordDTO, AccountInputModel>(model);
            var response = _sysUserCL.ChangePassword(userLoginInfo.Username, data);
            if (response == (int)PasswordEnum.Success)
            {
                return ResponseHelper.Success(_localizer.GetChangePasswordSuccessMessage(NameControllerEnum.User.ToStringValue()), userLoginInfo.Username);
            }
            else if (response == (int)PasswordEnum.ExistUser)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()), userLoginInfo.Username);
            }
            else if (response == (int)PasswordEnum.CurrentPasswordIncorrect)
            {
                return ResponseHelper.BadRequest(_localizer.GetOldPasswordErrorrMessage());

            }
            return ResponseHelper.BadRequest(_localizer.GetChangePasswordErrorMessage(NameControllerEnum.User.ToStringValue()), userLoginInfo.Username);
        }

        /// <summary>
        /// Thông tin tài khoản
        /// API: /api/AccountManager/Profile
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Profile")]
        [AllowAnyPermission]
        public IActionResult Profile()
        {
            var userLoginInfo = HttpContext.Items["UserLoginInfo"] as UserLoginInfoModel;

            if (userLoginInfo == null)
                return ResponseHelper.Unauthorized(_localizer.GetInvalidTokenMessage());

            var data = _sysUserCL.GetByUsername(userLoginInfo.Username);

            if (data.UserId > 0)
            {
                TruongInfoModel truongModel = new TruongInfoModel();
                if (!string.IsNullOrEmpty(data.TruongID))
                {
                    var truong = _truongCL.GetById(data.TruongID);
                    if (truong != null)
                    {
                        truongModel = ModelProvider.MapModelFromModel<TruongModel, TruongInfoModel>(truong);
                    }
                }

                return ResponseHelper.Ok(new { data, donvi = truongModel });
            }

            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()));
        }

        /// <summary>
        /// Cập nhật thông tin tài khoản
        /// API: /api/AccountManager/UpdateProfile
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateProfile")]
        [AllowAnyPermission]
        public IActionResult UpdateProfile([FromForm]UserUpdateProfileDTO request)
        {
            // Lấy thông tin tài khoản đăng nhập
            var userLoginInfo = HttpContext.Items["UserLoginInfo"] as UserLoginInfoModel;

            if (userLoginInfo == null || request.UserName != userLoginInfo.Username)
                return ResponseHelper.Unauthorized(_localizer.GetInvalidTokenMessage());

            if (request.FileImage != null)
            {
                string folderName = "Users";
                var fileResult = _fileService.SaveImage(request.FileImage, folderName);
                if (fileResult.Item1 == 1)
                {
                    request.Avatar = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            UserInputModel model = ModelProvider.MapModelFromModel<UserUpdateProfileDTO, UserInputModel>(request);
            model.IsUpdateProfile = true;
            var response = _sysUserCL.Save(model);
            if (response > 0)
            {
                return ResponseHelper.Created(_localizer.GetUpdateSuccessMessage(NameControllerEnum.User.ToStringValue()), model.UserName);
            }
            else if (response == -9)
            {
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(NameControllerEnum.User.ToStringValue()), model.UserName);
            }
            return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(NameControllerEnum.User.ToStringValue()), model.UserName);
        }
    }
}
