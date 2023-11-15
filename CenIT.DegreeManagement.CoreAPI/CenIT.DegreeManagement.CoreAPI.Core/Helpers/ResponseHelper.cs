using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CenIT.DegreeManagement.CoreAPI.Core.Helpers
{
    public static class ResponseHelper
    {
        /// <summary>
        /// Creates an IActionResult with a successful response containing the specified data and optional message.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">An optional message to include in the response.</param>
        /// <param name="name">An optional name to use for replacing placeholders in the message.</param>
        /// <returns>An IActionResult representing a successful response.</returns>
        public static IActionResult Ok(object data, string? message = null, string? name = null)
        {
            if (message != null && name != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + name + "]");
            }

            var result = new
            {
                isSuccess = true,
                data = data,
                message = message
            };

            return new JsonResult(result)
            {
                StatusCode = 200
            };
        }

        public static string ResultJson(string data, string? message = null, string? name = null)
        {
            if (message != null && name != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + name + "]");
            }
          
            // Tạo một JObject mới chứa message và data
            JObject newData = new JObject
                {
                    { "isSuccess", true },
                    { "data", string.IsNullOrEmpty(data) ? null : JObject.Parse(data) },
                    { "message", null }

                };

            // Convert JObject mới thành chuỗi JSON
            string resultJson = newData.ToString();

            //return Ok()

            return resultJson;
        }

        /// <summary>
        /// Creates an IActionResult with a response indicating a successful resource creation, containing the specified data and optional message.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">An optional message to include in the response.</param>
        /// <param name="name">An optional name to use for replacing placeholders in the message.</param>
        public static IActionResult Created(string? message = null, string? name = null)
        {
            if (message != null && name != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + name + "]");
            }
          
            var result = new
            {
                isSuccess = true,
                message = message
            };

            return new JsonResult(result)
            {
                StatusCode = 201
            };
        }

        /// <summary>
        /// Creates an IActionResult with a successful response containing a message.
        /// </summary>
        /// <param name="message">The message to include in the response.</param>
        /// <param name="name">An optional name to use for replacing placeholders in the message.</param>
        /// <returns>An IActionResult representing a successful response with a message.</returns>
        public static IActionResult Success(string message, string? name = null)
        {
            if (message != null && name != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + name + "]");
            }
            var result = new
            {
                isSuccess = true,
                message = message
            };

            return new JsonResult(result)
            {
                StatusCode = 200
            };
        }

        public static IActionResult SuccessHaveData(string message,object data ,string? name = null)
        {
            if (message != null && name != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + name + "]");
            }
            var result = new
            {
                isSuccess = true,
                data = data,
                message = message
            };

            return new JsonResult(result)
            {
                StatusCode = 200
            };
        }

        /// <summary>
        /// Creates an IActionResult with a response indicating a resource not found, containing the specified message.
        /// </summary>
        /// <param name="message">The message to include in the response.</param>
        /// <param name="name">An optional name to use for replacing placeholders in the message.</param>
        /// <returns>An IActionResult representing a resource not found response.</returns>
        public static IActionResult NotFound(string message, string? name = null)
        {
            if (message != null && name != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + name + "]");
            }

            var result = new
            {
                isSuccess = false,
                message = message
            };

            return new JsonResult(result)
            {
                StatusCode = 404
            };
        }

        /// <summary>
        /// Creates an IActionResult with a response indicating unauthorized access, containing the specified message.
        /// </summary>
        /// <param name="message">The message to include in the response.</param>
        /// <returns>An IActionResult representing an unauthorized access response.</returns>
        public static IActionResult Unauthorized(string message)
        {
            var result = new
            {
                isSuccess = false,
                message = message
            };

            return new JsonResult(result)
            {
                StatusCode = 401
            };
        }

        // <summary>
        /// Creates an IActionResult with a response indicating forbidden access, containing the specified message.
        /// </summary>
        /// <param name="message">The message to include in the response.</param>
        /// <param name="username">An optional username to use for replacing placeholders in the message.</param>
        /// <returns>An IActionResult representing a forbidden access response.</returns>
        public static IActionResult Forbidden(string message, string? username =null)
        {
            if (message != null && username != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + username + "]");
            }

            var result = new
            {
                isSuccess = false,
                message = message
            };

            return new JsonResult(result)
            {
                StatusCode = 403
            };
        }

        /// <summary>
        /// Creates an IActionResult with a response indicating a bad request, containing the specified message, optional field name, and optional list of errors.
        /// </summary>
        /// <param name="message">The message to include in the response.</param>
        /// <param name="name">An optional name to use for replacing placeholders in the message.</param>
        /// <param name="errors">An optional array of errors.</param>
        /// <returns>An IActionResult representing a bad request response.</returns>
        public static IActionResult BadRequest(string message, string? name = null)
        {

            if (message != null && name != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + name + "]");
            }

            var result = new
            {
                isSuccess = false,
                message = message
            };
            return new JsonResult(result)
            {
                StatusCode = 400
            };
        }

        public static IActionResult BadRequestHaveData(string message, object data, string? name = null)
        {

            if (message != null && name != null && message.Contains("-"))
            {
                message = message.Replace("-", "[" + name + "]");
            }

            var result = new
            {
                isSuccess = false,
                data = data,
                message = message
            };
            return new JsonResult(result)
            {
                StatusCode = 400
            };
        }

        public static IActionResult Errors(List<Dictionary<string, string>> Errors)
        {

            var result = new
            {
                isSuccess = false,
                errors = Errors
            };
            return new JsonResult(result)
            {
                StatusCode = 400
            };
        }

        public static IActionResult Error500(string error)
        {

            var result = new
            {
                isSuccess = false,
                error = error
            };
            return new JsonResult(result)
            {
                StatusCode = 500
            };
        }


    }

}
