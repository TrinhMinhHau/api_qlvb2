using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using Microsoft.Extensions.Localization;
using System.Xml.Linq;

namespace CenIT.DegreeManagement.CoreAPI.Resources
{
    public class ShareResource : Namelocalizer
    {
        private readonly IStringLocalizer<ShareResource> _localizer;

        public ShareResource(IStringLocalizer<ShareResource> localizer)
        {
            _localizer = localizer;
        }

        #region MESSAGE CRUD
        /// <summary>
        /// Get the localized "add success" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "add success" message.</returns>
        public string GetAddSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_addSuccess], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "add error" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "add error" message.</returns>
        public string GetAddErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_addError], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "update success" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "update success" message.</returns>
        public string GetUpdateSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_updateSuccess], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "update error" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "update error" message.</returns>
        public string GetUpdateErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_updateError], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "delete success" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "delete success" message.</returns>
        public string GetDeleteSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_deleteSuccess], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "delete error" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "delete error" message.</returns>
        public string GetDeleteErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_deleteError], _localizer[controllerName].ToString().ToLower());
        }

        public string GetSettingSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_settingSucces], _localizer[controllerName].ToString().ToLower());
        }

        public string GetSettingErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_settingError], _localizer[controllerName].ToString().ToLower());
        }

        public string GetCancelSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_cancelSuccess], _localizer[controllerName].ToString().ToLower());
        }

        public string GetCancelErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_cancelError], _localizer[controllerName].ToString().ToLower());
        }
        #endregion

        #region ERROR MESSAGE

        /// <summary>
        /// Get the localized "already exist" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "already exist" message.</returns>
        public string GetAlreadyExistMessage(string controllerName, string? value = null)
        {
            string message = _localizer[controllerName];
            if(value != null)
            {
                message = _localizer[value] + " " + _localizer[controllerName].ToString().ToLower();
            }
            return string.Format(_localizer[_alreadyExist], message);
        }

        /// <summary>
        /// Get the localized "not exist" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "not exist" message.</returns>
        public string GetNotExistMessage(string controllerName)
        {
            return string.Format(_localizer[_notExist], _localizer[controllerName]);
        }

        public string GetExistInUseMessage(string controllerName)
        {
            return string.Format(_localizer[_existInUse], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "File Format Error" message.
        /// </summary>
        /// <returns>The localized "File Format Errorr" message.</returns>
        public string GetFileFormatErrorMessage(string[] allowedExtensions)
        {
            return string.Format(_localizer[_fileFormatError], string.Join(",", allowedExtensions));
        }

        /// <summary>
        /// Get the localized "UploadError" message.
        /// </summary>
        /// <returns>The localized "UploadError" message.</returns>
        public string GetUploadErrorMessage()
        {
            return string.Format(_localizer[_uploadError]);
        }


        #endregion

        #region ACCOUNT MESSAGE
        /// <summary>
        /// Get the localized "invalid token" message.
        /// </summary>
        /// <returns>The localized "invalid token" message.</returns>
        public string GetInvalidTokenMessage()
        {
            return string.Format(_localizer[_invalidToken]);
        }

        /// <summary>
        /// Get the localized "login success" message.
        /// </summary>
        /// <returns>The localized "invalid token" message.</returns>
        public string GetLoginSuccessMessage()
        {
            return string.Format(_localizer[_loginSuccess]);
        }

        /// <summary>
        /// Get the localized "login error" message.
        /// </summary>
        /// <returns>The localized "login error" message.</returns>
        public string GetLoginErrorMessage()
        {
            return string.Format(_localizer[_loginError]);
        }

        /// <summary>
        /// Get the localized "login error" message.
        /// </summary>
        /// <returns>The localized "login error" message.</returns>
        public string GetNoPermissionMessage()
        {
            return string.Format(_localizer[_noPermission]);
        }

        /// <summary>
        /// Get the localized "UploadError" message.
        /// </summary>
        /// <returns>The localized "UploadError" message.</returns>
        public string GetLogoutSuccessMessage()
        {
            return string.Format(_localizer[_logoutSuccess]);
        }

        /// <summary>
        /// Get the localized "UploadError" message.
        /// </summary>
        /// <returns>The localized "UploadError" message.</returns>
        public string GetLogoutErrorMessage()
        {
            return string.Format(_localizer[_logoutError]);
        }

        public string GetChangePasswordErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_changePasswordError], _localizer[controllerName].ToString());
        }

        public string GetChangePasswordSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_changePasswordSuccess], _localizer[controllerName].ToString());
        }

        public string GetRequestChangePasswordMessage()
        {
            return string.Format(_localizer[_requestChangePassword]);
        }

        #endregion

        #region ATTRIBUTE MESSAGE
        public string GetOldPasswordErrorrMessage()
        {
            return string.Format(_localizer[_oldPasswordError]);
        }

        public string GetConfirmPasswordErrorMessage()
        {
            return string.Format(_localizer[_confirmPasswordError]);
        }

        /// <summary>
        /// Get the localized "invalid field" message.
        /// </summary>
        /// <returns>The localized "invalid field" message.</returns>
        public string GetInvalidFieldMessage(string error, string key)
        {
            string localizerName = "";

            if (error.Contains("#"))
            {
                 localizerName = error.Replace("#", "");
            } else
            {
                if (error.ToLower().Contains("required"))
                {
                    localizerName = "Required";
                }
                else
                {
                    localizerName = _invalidField;
                }
                
            }

            string message = _localizer[localizerName];

            return message;
        }

        #endregion

        #region MESSAGE

        public string MessageName(string key)
        {
            return string.Format(_localizer[key]);
        }

        /// <summary>
        /// Get the localized "PermissionGroupError" message.
        /// </summary>
        /// <returns>The localized "PermissionGroupError" message.</returns>
        public string GetNoLoginMessage()
        {
            return string.Format(_localizer[_noLogin]);
        }
        /// <summary>
        /// Get the localized "PermissionGroupError" message.
        /// </summary>
        /// <returns>The localized "PermissionGroupError" message.</returns>
        public string GetPermissionGroupErrorMessage()
        {
            return string.Format(_localizer[_permissionGroupError]);
        }

        /// <summary>
        /// Get the localized "PermissionGroupSuccess" message.
        /// </summary>
        /// <returns>The localized "PermissionGroupSuccess" message.</returns>
        public string GetPermissionGroupSuccessMessage()
        {
            return string.Format(_localizer[_permissionGroupSuccess]);
        }

        /// <summary>
        /// Get the localized "active success" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "active success" message.</returns>
        public string GetActiveSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_activeSuccess], _localizer[controllerName].ToString().ToLower());
        }


        public string GetApprovedMessage(string controllerName)
        {
            return string.Format(_localizer[_approved], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "active error" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "active error" message.</returns>
        public string GetActiveErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_activeError], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "deActive success" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "deActive success" message.</returns>
        public string GetDeActiveSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_deActiveSuccess], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "deActive error" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "deActive error" message.</returns>
        public string GetDeActiveErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_deActiveError], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "UserHasDeActive" message.
        /// </summary>
        /// <returns>The localized "UserHasDeActive" message.</returns>
        public string GetUserHasDeActiveMessage(string controllerName)
        {
            return string.Format(_localizer[_userHasDeActive], _localizer[controllerName].ToString());
        }


        public string GetResetPasswordSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_resetPasswordSuccess], _localizer[controllerName].ToString().ToLower());
        }

        public string GetResetPasswordErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_resetPasswordError], _localizer[controllerName].ToString());
        }

        public string GetConfirmSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_confirmSuccess], _localizer[controllerName].ToString());
        }

        public string GetConfirmErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_confirmError], _localizer[controllerName].ToString());
        }

        public string GetConfirmAllSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_confirmAllSuccess], _localizer[controllerName].ToString());
        }

        public string GetConfirmAllErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_confirmAllError], _localizer[controllerName].ToString());
        }


        public string GetApproveAllSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_approveAllSuccess], _localizer[controllerName].ToString());
        }

        public string GetApproveAllErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_approveAllSuccess], _localizer[controllerName].ToString());
        }

        public string GetApproveSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_approveSuccess], _localizer[controllerName].ToString());
        }

        public string GetApproveErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_approveSuccess], _localizer[controllerName].ToString());
        }

        public string GetGiveBackSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_giveBackSuccess], _localizer[controllerName].ToString());
        }

        public string GetGiveBackErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_giveBackError], _localizer[controllerName].ToString());
        }

        /// <summary>
        /// Get the localized "update success" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "update success" message.</returns>
        public string GetImportSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_importSuccess], _localizer[controllerName].ToString().ToLower());
        }

        /// <summary>
        /// Get the localized "update success" message.
        /// </summary>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The localized "update success" message.</returns>
        public string GetImportErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_importError], _localizer[controllerName].ToString().ToLower());
        }

        public string GetListEmptyMessage(string controllerName)
        {
            return string.Format(_localizer[_listEmpty], _localizer[controllerName].ToString());
        }

        public string GetYearNotMatchDateMessage(string year, string date)
        {
            return string.Format(_localizer[_yearNotMatchDate], year, date.ToLower());
        }

        public string GetExceedsPhoiGocLimitMessage()
        {
            return string.Format(_localizer[_exceedsPhoiGocLimit]);
        }

    
        public string GetPutInToSuccessMessage(string controllerName)
        {
            return string.Format(_localizer[_putInToSuccess], _localizer[controllerName].ToString());
        }

        public string GetPutInToErrorMessage(string controllerName)
        {
            return string.Format(_localizer[_putInToError], _localizer[controllerName].ToString());
        }

        #endregion
    }
}
