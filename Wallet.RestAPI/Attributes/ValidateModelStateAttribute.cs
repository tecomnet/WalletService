using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

#nullable enable

namespace Wallet.RestAPI.Attributes
{
    /// <summary>
    /// Model state validation attribute
    /// </summary>
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called before the action method is invoked
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Per https://blog.markvincze.com/how-to-validate-action-parameters-with-dataannotation-attributes/
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                foreach (var parameter in descriptor.MethodInfo.GetParameters())
                {
                    object? args = null;
                    if (parameter.Name != null && context.ActionArguments.ContainsKey(key: parameter.Name))
                    {
                        args = context.ActionArguments[key: parameter.Name];
                    }

                    ValidateAttributes(parameter: parameter, args: args, modelState: context.ModelState);
                }
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(modelState: context.ModelState);
            }
        }

        private static void ValidateAttributes(ParameterInfo parameter, object? args, ModelStateDictionary modelState)
        {
            foreach (var attributeData in parameter.CustomAttributes)
            {
                var attributeInstance = parameter.GetCustomAttribute(attributeType: attributeData.AttributeType);

                var validationAttribute = attributeInstance as ValidationAttribute;
                if (validationAttribute == null)
                {
                    continue;
                }

                var isValid = validationAttribute.IsValid(value: args);
                if (isValid)
                {
                    continue;
                }

                if (parameter.Name != null)
                {
                    modelState.AddModelError(key: parameter.Name,
                        errorMessage: validationAttribute.FormatErrorMessage(name: parameter.Name));
                }
            }
        }
    }
}
