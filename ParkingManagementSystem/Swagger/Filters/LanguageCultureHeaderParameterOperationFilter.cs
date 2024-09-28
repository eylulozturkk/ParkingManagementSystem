using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ParkingManagementSystem.API.Swagger.Filters
{
    /// <summary>
    /// LanguageCultureHeaderParameterOperationFilter
    /// </summary>
    public class LanguageCultureHeaderParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply the filter
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Language-Culture",
                In = ParameterLocation.Header,
                Description = "Header parameter. Send Language Culture value on header. Examples: tr-TR, en-US. Language Entity must contains culture values. If Language-Id sent on header, Language-Culture is ignored.",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = null
                }
            });
        }
    }
}