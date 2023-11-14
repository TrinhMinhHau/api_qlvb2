using CenIT.DegreeManagement.CoreAPI.Caching.Account;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CenIT.DegreeManagement.CoreAPI.Processor
{
    public static class RequestProcessor
    {
        public class ErrorObject
        {
            public string Key { get; set; } = string.Empty;
            public string Error { get; set; } = string.Empty;
        }

        /// <summary>
        /// Check if a user has permission to access each action of a function.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        /// <param name="username">The username.</param>
        /// <returns>True if the user has permission; false if the user does not have permission.</returns>
        public static bool CheckUserPermission(ActionExecutingContext context, string username, string token)
        {

            ICacheService cacheService = context.HttpContext.RequestServices.GetService<ICacheService>()!;
            SysPermissionCL _cacheLayer = new SysPermissionCL(cacheService);
            string controllerName = context.RouteData.Values["Controller"]!.ToString()!;
            string actionName = context.RouteData.Values["Action"]!.ToString()!;
            var isPermission = _cacheLayer.Permission_IsAllow(username, controllerName, actionName);
            var _accessHistory = new SysAccessHistoryCL(cacheService);
            if (!actionName.ToLower().Contains("get") && !actionName.ToLower().Contains("view"))
            {
                var acccessHistoryModel = new AccessHistoryInputModel()
                {
                    Action = actionName,
                    Function = controllerName,
                    UserName = username,
                    Token = token
                };

                if (isPermission < 0)
                {
                    acccessHistoryModel.IsSuccess = false;
                    _accessHistory.Save(acccessHistoryModel);
                    return false;
                }
                _accessHistory.Save(acccessHistoryModel);
                return true;
            }

            if (isPermission < 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check the expiration status of a token.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        /// <param name="userName">The username.</param>
        /// <param name="token">The token.</param>
        /// <returns>True if the token is still valid; false if it has expired.</returns>
        public static bool CheckTokenExpiration(ActionExecutingContext context, string userName, string token)
        {

            ICacheService cacheService = context.HttpContext.RequestServices.GetService<ICacheService>()!;
            AuthCL _cacheLayer = new AuthCL(cacheService);
            var getCaching = _cacheLayer.GetCacheLogin(userName, token);

             if(getCaching == null)
            {
                return false;
            }
            
            bool isExpiredTime = getCaching.ExpiredTime > DateTime.Now;

            if (!isExpiredTime)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the SECRET_KEY string.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        /// <returns>The SECRET_KEY string.</returns>
        public static string GetSecretKey(ActionExecutingContext context)
        {
            // lấy chuỗi SECRET_KEY từ AppSettings
            string key = context.HttpContext.RequestServices.GetService<IConfiguration>()!
                            .GetSection("AppSettings:SECRET_KEY").Value!;
            return key;
        }

        /// <summary>
        /// Generate a signature based on the token, request body, and secret key.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        /// <param name="token">The token.</param>
        /// <param name="key">The secret key.</param>
        /// <returns>The generated signature.</returns>
        public static async Task<string> GenerateSignature(ActionExecutingContext context, string token, string key)
        {
            var req = context.HttpContext.Request;
            req.EnableBuffering();
            string body = "";
            string signature;


            bool check = IsMultipartContentType(req.ContentType);
            if (check)
            {
                // Handle multipart/form-data request with files
                var formFeature = await req.ReadFormAsync();
                var formFields = new Dictionary<string, string>();

                // Iterate over form fields and exclude IFormFile fields
                foreach (var field in formFeature)
                {
                    if (field.Value.Count == 1 && field.Value.First() is string value)
                    {
                        // Add non-file fields to the formFields dictionary
                        formFields[field.Key] = value;
                    }
                }
                // Convert the formFields dictionary to a JSON object
                var jsonObject = JObject.FromObject(formFields);

                // Use the JSON object as the request body (without files) here
                //body = jsonObject.ToString().TrimEnd('\r', '\n');
                body = jsonObject.ToString(Formatting.None).TrimEnd(' ');

            }
            else
            {
                if (req.Body.CanSeek)
                {
                    req.Body.Seek(0, SeekOrigin.Begin);

                    using (StreamReader reader = new StreamReader(context.HttpContext.Request.Body, System.Text.Encoding.UTF8))
                    {
                        body = reader.ReadToEnd().ToString().TrimEnd('\r', '\n', ' ');
                    }

                }
            }

            var content_body = string.IsNullOrEmpty(body) ? "" : body;
            signature = (token + content_body + key).Trim();
            string signatureMD5 = EHashMd5.CalculateMD5(signature);
            return signatureMD5;
        }

        /// <summary>
        /// Retrieves the error fields from the ModelState in the provided ActionExecutingContext.
        /// </summary>
        /// <param name="context">The ActionExecutingContext containing the ModelState.</param>
        /// <returns>An array of error fields.</returns>
        public static ErrorObject GetErrorFields(ActionExecutingContext context)
        {
            var errorFieldsAndMessages = context.ModelState
                .Where(x => x.Value!.Errors.Count > 0)
                .Select(x => new ErrorObject
                {
                    Key = x.Key,
                    Error = x.Value!.Errors.First().ErrorMessage
                })
                .FirstOrDefault();

              return errorFieldsAndMessages;
        }

        private static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
