using System;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Template.RestAPI.Helpers;

/// <summary>
/// This class contains configurations for enable API versioning in swagger
/// </summary>
public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        this._provider = provider;
    }
    /// <summary>
    /// Configure swagger options
    /// </summary>
    /// <param name="options"></param>
    public void Configure(SwaggerGenOptions options)
    {
        // add swagger document for every API version discovered
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }
    /// <summary>
    /// Configure swagger options
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }
    /// <summary>
    /// Create info for API version
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        OpenApiInfo info = new()
        {
            // FIXME: Title and Description, change for information about this service
            Title = "TemplateService API",
            Description = "TemplateService API to publish the functionality of TemplateService service",
            Contact = new OpenApiContact()
            {
                Name = "Swagger Codegen Contributors",
                Url = new Uri("https://github.com/swagger-api/swagger-codegen"),
                Email = "edilberto_diaz14@hotmail.com"
            },
            TermsOfService = new Uri("https://github.com/swagger-api/swagger-codegen"),
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}