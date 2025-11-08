using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Repo.Core.Infrastructure.SwaggerExtensions;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParameters = context.ApiDescription.ActionDescriptor.Parameters
            .Where(p => p.ParameterType == typeof(IFormFile));

        if (fileParameters.Any())
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "multipart/form-data", new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = fileParameters.ToDictionary(
                                    fp => fp.Name,
                                    _ => new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                )
                            }
                        }
                    }
                }
            };
        }
    }
}