using Microsoft.OpenApi.Models;
using ParkingManagementSystem.API.Swagger.CustomAttributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
namespace ParkingManagementSystem.API.Swagger.OperationFilters
{
    public class SummaryOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasFrontEndEndpointAttribute = context.ApiDescription
                .CustomAttributes().Any(x => x is FrontEndEndpointAttribute);

            if (hasFrontEndEndpointAttribute)
            {
                operation.Summary = $"[Front End] {operation.Summary}";
            }

            operation.Summary = $"[Back End] {operation.Summary}";


            var hasVersionEndpoint = context.ApiDescription.CustomAttributes().Any(x => x is VersioningEndpointAttribute);

            if (hasVersionEndpoint)
            {
                operation.Summary = $"[New Version] {operation.Summary}";
            }
        }
    }
}
