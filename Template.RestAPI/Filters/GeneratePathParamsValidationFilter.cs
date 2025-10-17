using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Template.RestAPI.Filters
{
    /// <summary>
    /// Path Parameter Validation Rules Filter
    /// </summary>
    public class GeneratePathParamsValidationFilter : IOperationFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="context">OperationFilterContext</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var pars = context.ApiDescription.ParameterDescriptions;

            foreach (var par in pars)
            {
                var swaggerParam = operation.Parameters.SingleOrDefault(p => p.Name == par.Name);
                var attributes = ((ControllerParameterDescriptor)par.ParameterDescriptor)
                    .ParameterInfo
                    .CustomAttributes
                    .ToArray();

                if (attributes.Any() && swaggerParam != null)
                {
                    var requiredAttr = attributes.FirstOrDefault(p => p.AttributeType == typeof(RequiredAttribute));
                    if (requiredAttr != null)
                    {
                        swaggerParam.Required = true;
                    }

                    var regexAttr =
                        attributes.FirstOrDefault(p => p.AttributeType == typeof(RegularExpressionAttribute));
                    if (regexAttr != null)
                    {
                        var regex = regexAttr.ConstructorArguments[0].Value;
                        swaggerParam.Schema.Pattern = regex?.ToString();
                    }

                    int? minLength = null, maxLength = null;
                    var stringLengthAttr =
                        attributes.FirstOrDefault(p => p.AttributeType == typeof(StringLengthAttribute));
                    if (stringLengthAttr != null)
                    {
                        if (stringLengthAttr.NamedArguments.Count == 1 &&
                            stringLengthAttr.NamedArguments.Any(p => p.MemberName == "MinimumLength"))
                        {
                            minLength = (int)(stringLengthAttr.NamedArguments
                                .Single(p => p.MemberName == "MinimumLength").TypedValue.Value ?? 0);
                        }

                        maxLength = (int)(stringLengthAttr.ConstructorArguments[0].Value ?? 0);
                    }

                    var minLengthAttr =
                        attributes.FirstOrDefault(p => p.AttributeType == typeof(MinLengthAttribute));
                    if (minLengthAttr != null)
                    {
                        minLength = (int)(minLengthAttr.ConstructorArguments[0].Value ?? 0);
                    }

                    var maxLengthAttr =
                        attributes.FirstOrDefault(p => p.AttributeType == typeof(MaxLengthAttribute));
                    if (maxLengthAttr != null)
                    {
                        maxLength = (int)(maxLengthAttr.ConstructorArguments[0].Value ?? 0);
                    }

                    swaggerParam.Schema.MinLength = minLength;
                    swaggerParam.Schema.MaxLength = maxLength;

                    var rangeAttr = attributes.FirstOrDefault(p => p.AttributeType == typeof(RangeAttribute));
                    if (rangeAttr == null)
                    {
                        continue;
                    }

                    var rangeMin = (int)(rangeAttr.ConstructorArguments[0].Value ?? 0);
                    var rangeMax = (int)(rangeAttr.ConstructorArguments[1].Value ?? 0);

                    swaggerParam.Schema.Minimum = rangeMin;
                    swaggerParam.Schema.Maximum = rangeMax;
                }
            }
        }
    }
}
