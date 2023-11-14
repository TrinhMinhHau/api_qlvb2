using CenIT.DegreeManagement.CoreAPI.Attributes;
using CenIT.DegreeManagement.CoreAPI.Caching.Account;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Account;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Account;
using CenIT.DegreeManagement.CoreAPI.Processor.Mail;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CenIT.DegreeManagement.CoreAPI.Models.Account;
using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using Token = CenIT.DegreeManagement.CoreAPI.Core.Utils.Token;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Processor;
using Hangfire;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseAppController
    {
        private AuthCL _cacheLayer;
        private SysMenuCL _sysMenuCL;
        private SysUserCL _sysUserCL;
        private TruongCL _truongCL;
        private SysAccessHistoryCL _sysAccessHistory;
        private SysDeviceTokenCL _sysDeviceTokenCL;
        private readonly BackgroundJobManager _backgroundJobManager;
        private ILogger<AuthController> _logger;
        private ISendMailService _sendMailService;
        private readonly ShareResource _localizer;
        
        public AuthController(ICacheService cacheService, BackgroundJobManager backgroundJobManager, IConfiguration configuration, ShareResource localizer, ILogger<AuthController> logger, ISendMailService sendMailService) : base(cacheService, configuration)
        {
            _cacheLayer = new AuthCL(cacheService);
            _sysAccessHistory = new SysAccessHistoryCL(cacheService);
            _localizer = localizer;
            _logger = logger;
            _sysMenuCL = new SysMenuCL(cacheService);
            _sysUserCL = new SysUserCL(cacheService);
            _sendMailService = sendMailService;
            _truongCL = new TruongCL(cacheService, configuration);
            _sysDeviceTokenCL = new SysDeviceTokenCL(cacheService);
            _backgroundJobManager = backgroundJobManager;

        }

        /// <summary>
        /// Đăng nhập
        /// API: /api/Auth/login
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO request)
        {
            AccountInputModel data = ModelProvider.MapModelFromModel<LoginDTO, AccountInputModel>(request);
            var userLogin = _cacheLayer.Login(data);
            // Lấy tên Controller và Action từ RouteData
            var controllerName = this.ControllerContext.RouteData.Values["controller"]?.ToString();
            var actionName = this.ControllerContext.RouteData.Values["action"]?.ToString();

            var acccessHistoryModel = new AccessHistoryInputModel()
            {
                Action = actionName,
                Function = controllerName,
                UserName = userLogin.Username,
                Token = userLogin.Token,
            };

            if (userLogin.UserId > 0)
            {
                // Lấy menu theo username và token đã đăng nhập
                var menu = _sysMenuCL.GetMenuByUsername(request.Username, userLogin.Token);

                // Lấy đơn vị trường
                TruongModel truong = new TruongModel();
                if (!string.IsNullOrEmpty(userLogin.TruongID) && CheckString.CheckBsonId(userLogin.TruongID))
                {
                    truong = _truongCL.GetById(userLogin.TruongID);
                }
                var reporst = _sysUserCL.GetReportByUserId(userLogin.UserId);
                var response = new { userLogin, donvi = truong, reporst = reporst, menu };

                // Lưu lịch sử truy cập (truy cập thành công)
                _sysAccessHistory.Save(acccessHistoryModel);
                return ResponseHelper.Ok(response, _localizer.GetLoginSuccessMessage());
            }

            // Lưu lịch sử truy cập  (truy cập thất bại)
            acccessHistoryModel.IsSuccess = false;
            _sysAccessHistory.Save(acccessHistoryModel);
            if (userLogin.UserId == -9)
            {
                return ResponseHelper.BadRequest(_localizer.GetUserHasDeActiveMessage(NameControllerEnum.User.ToStringValue()), request.Username);
            }

            return ResponseHelper.BadRequest(_localizer.GetLoginErrorMessage());
        }


        /// <summary>
        /// Lưu DeviceToken
        /// API: /api/Auth/SaveDeviceToken
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveDeviceToken")]
        [AllowAnonymous]
        public IActionResult SaveDeviceToken([FromBody] DeviceTokenInputModel model)
        {

            var response = _sysDeviceTokenCL.Save(model);
            if (response == (int)RoleEnum.Fail)
            {
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage("DeviceToken"), model.DeviceToken);
            }
            return ResponseHelper.Created(_localizer.GetAddSuccessMessage("DeviceToken"), model.DeviceToken);

        }

        /// <summary>
        /// Đăng xuất
        /// API: /api/Auth/Logout
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        [AllowAnyPermission]
        public IActionResult Logout([FromForm] string token)
        {
            // Lấy thông tin tài khoản đăng nhập
            var userLoginInfo = HttpContext.Items["UserLoginInfo"] as UserLoginInfoModel;
            if (userLoginInfo == null)
                return ResponseHelper.Unauthorized(_localizer.GetInvalidTokenMessage());

            var response = _cacheLayer.Logout(token);
            var controllerName = this.ControllerContext.RouteData.Values["controller"]?.ToString();
            var actionName = this.ControllerContext.RouteData.Values["action"]?.ToString();

            var acccessHistoryModel = new AccessHistoryInputModel()
            {
                Action = actionName,
                Function = controllerName,
                UserName = userLoginInfo.Username,
                Token = token,
            };

            if (response > 0)
            {
                //Lưu lịch sử truy cập (Thành công)
                _sysAccessHistory.Save(acccessHistoryModel);
                return ResponseHelper.Success(_localizer.GetLogoutSuccessMessage());
            }

            //Lưu lịch sử truy cập (thất bại)
            acccessHistoryModel.IsSuccess = false;
            _sysAccessHistory.Save(acccessHistoryModel);
            return ResponseHelper.BadRequest(_localizer.GetLogoutErrorMessage());
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([CustomRequired][Email]string email)
        {
            var user = _sysUserCL.GetByEmail(email);
            if(user.UserId > 0)
            {
                var token = Token.ResetPasswordTokenGenerator(64, email);
                var tokenExpires = DateTime.Now.AddDays(1);
                var result = _cacheLayer.SavePasswordResetToken(email, token, tokenExpires);
                if(result > 0)
                {
                    var link = $"http://localhost:3000/reset-password?token={token}&email={email}";
                    MailContent content = new MailContent
                    {
                        To = user.Email,
                        Subject = "Quên mật khẩu",
                        Body = $"<p><strong>Xin chào {user.Email}</strong></p>" +
                       $"<p>Vui lòng nhấp vào liên kết sau để đặt lại mật khẩu:</p>" +
                       $"<a href=\"{link}\">Bấm vào đây</a>"
                    };


                  

                    BackgroundJob.Enqueue(() => _backgroundJobManager.SendEmailInBackground(content));
                    return ResponseHelper.Success(_localizer.GetRequestChangePasswordMessage(), email);
                }
            }
            return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()));
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO model)
        {
            AccountInputModel data = ModelProvider.MapModelFromModel<ResetPasswordDTO, AccountInputModel>(model);

            var response = _cacheLayer.ResetPassword(data);
            if(response > 0)
            {
                return ResponseHelper.Success(_localizer.GetChangePasswordSuccessMessage(NameControllerEnum.User.ToStringValue()), model.Email);
            } 
            else if(response == -9)
            {
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.User.ToStringValue()));

            } 
            else if(response == -1)
            {
                return ResponseHelper.BadRequest(_localizer.GetInvalidTokenMessage());
            }
            return ResponseHelper.BadRequest(_localizer.GetChangePasswordErrorMessage(NameControllerEnum.User.ToStringValue()));
        }
    }
}