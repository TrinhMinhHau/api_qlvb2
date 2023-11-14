using CenIT.DegreeManagement.CoreAPI.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Account;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CenIT.DegreeManagement.CoreAPI.Attributes
{
    /// <summary>
    /// Custom authentication attribute for validating token and user permissions.
    /// </summary>
    public class CustomAuthenticateAttribute : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            ShareResource localizer = context.HttpContext.RequestServices.GetService<ShareResource>()!;

            bool isAllowAnonymous = IsAllowAnonymous(context);

            if (!isAllowAnonymous)
            {

                // Get token from header
                string headerToken = context.HttpContext.Request.Headers["Token"]!;
                if (headerToken == null)
                {
                    context.Result = ResponseHelper.Unauthorized(localizer.GetInvalidTokenMessage());
                    return;
                }

                var tokenSplit = headerToken.Split('.');

                if(tokenSplit.Length < 3)
                {
                    context.Result = ResponseHelper.Unauthorized(localizer.GetInvalidTokenMessage());
                    return;
                }

                string[] newTokenSplit = tokenSplit.Take(tokenSplit.Length - 2).ToArray();

                string userName  = string.Join(".", newTokenSplit);
                string token  = tokenSplit[tokenSplit.Length - 2];
                string signatureHeader  = tokenSplit[tokenSplit.Length - 1];



                // Create and populate UserInfoFromTokenModel
                var userLoginInfo = new UserLoginInfoModel
                {
                    Username = userName,
                    Token = token
                };
                //Save login info
                context.HttpContext.Items["UserLoginInfo"] = userLoginInfo;


                // Check token expiration
                if (!RequestProcessor.CheckTokenExpiration(context, userName, token))
                {
                    context.Result = ResponseHelper.Unauthorized(localizer.GetInvalidTokenMessage());
                    return;
                }

                // Get SECRET_KEY from AppSettings
                string key = RequestProcessor.GetSecretKey(context);

                // Get the request body value for signature generation
                string signature = "";
                if (context.HttpContext.Request != null)
                {
                    signature = await RequestProcessor.GenerateSignature(context, token, key);
                }
                else
                {
                    context.Result = ResponseHelper.Unauthorized(localizer.GetInvalidTokenMessage());
                    return;
                }

                // Compare the generated signature with the signature from the header
                if (signature != signatureHeader)
                {
                    context.Result = ResponseHelper.Unauthorized(localizer.GetInvalidTokenMessage());
                    return;
                }

                // Check if the controller action uses the SkipCheckPermissionAttributes attribute.
                if (!IsAllowAnyPermission(context))
                {
                    // Check user permissions
                    if (!RequestProcessor.CheckUserPermission(context, userName, token))
                    {
                        context.Result = ResponseHelper.Forbidden(localizer.GetNoPermissionMessage(), userName);
                        return;
                    }
                }
            }

            // Check the validity of the request
            if (context.ModelState.IsValid == false)
            {
                var fieldsError = RequestProcessor.GetErrorFields(context);

                var errorMessage = localizer.GetInvalidFieldMessage(fieldsError.Error, fieldsError.Key);
    
                context.Result = ResponseHelper.BadRequest(errorMessage, fieldsError.Key);
                return;
            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Determines if the action allows anonymous access based on the presence of the AllowAnonymousAttribute.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        /// <returns>True if the action allows anonymous access; otherwise, false.</returns>
        private bool IsAllowAnonymous(ActionExecutingContext context)
        {
            return context.ActionDescriptor.EndpointMetadata
           .Any(e => e.GetType() == typeof(AllowAnonymousAttribute));
        }

        /// <summary>
        /// Determines if the controller uses the SkipUserPermissionCheckFilter attribute.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        /// <returns>True if the controller uses the SkipUserPermissionCheckFilter attribute; otherwise, false.</returns>
        private bool IsAllowAnyPermission(ActionExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor != null)
            {
                return actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                    .Any(a => a.GetType() == typeof(AllowAnyPermission));
            }
            return false;
        }
    }
}
    