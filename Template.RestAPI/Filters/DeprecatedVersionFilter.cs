using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Template.RestAPI.Filters;

/// <summary>
/// This class has an implementation that will be used to add a deprecated flag when the SwaggerUI was rendered
/// </summary>
public class DeprecatedVersionFilter : IOperationFilter
{
	/// <summary>
	/// Method to apply the filter
	/// </summary>
	/// <param name="operation">Operation to apply the filter</param>
	/// <param name="context">Context of the operation</param>
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var obsoleteOperation = context.MethodInfo.CustomAttributes.Any(type => type.AttributeType.Name == "ObsoleteAttribute");
        if (obsoleteOperation)
        {
            operation.Deprecated = true;
        }
    }
}