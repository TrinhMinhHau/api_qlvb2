using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CenIT.DegreeManagement.CoreAPI.Middleware
{
    public class AddHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Thêm header vào yêu cầu (request)
            operation.Parameters ??= new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Token",
                In = ParameterLocation.Header,
                Description = "Your header description",
                Required = false, // True nếu header là bắt buộc
                Schema = new OpenApiSchema
                {
                    Type = "String" // Kiểu dữ liệu của header
                }
            });

            // Thêm header Accept-Language vào yêu cầu (request)
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Description = "The language preference for the response",
                Required = false,
                
                Schema = new OpenApiSchema
                {
                    Type = "String"
                },
             
            });

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Name == "Accept-Language" && parameter.Schema.Default == null)
                {
                    parameter.Schema.Default = new OpenApiString("vi-VN");
                }
            }
        }
    }
}
