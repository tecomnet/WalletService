using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Template.RestAPI.Filters;

/// <summary>
/// Adds a custom header response when the requested method version was deprecated.
/// </summary>
public class ObsoleteMethodFilter : IActionFilter
{
	/// <summary>
	/// Method to apply the filter
	/// </summary>
	/// <param name="context">Context of the operation</param>
	public void OnActionExecuting(ActionExecutingContext context)
	{
		// not implemented
	}
	/// <summary>
	/// Method to apply the filter
	/// </summary>
	/// <param name="context">Context of the operation</param>
	public void OnActionExecuted(ActionExecutedContext context)
	{
		var requestedApiVersion = context.HttpContext.GetRequestedApiVersion();
		var obsoleteOperation =
			(context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.CustomAttributes.Any(type =>
				type.AttributeType.Name == "ObsoleteAttribute");
		if (obsoleteOperation.HasValue && obsoleteOperation.Value)
		{
			context.HttpContext.Response.Headers.Add(
				"deprecated-method-version",
				$"Requested version '{requestedApiVersion}' is deprecated for this method.");
		}
	}
}