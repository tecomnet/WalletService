using System.Linq;
using System.Net;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Http;
using Template.Funcionalidad;
using Template.RestAPI.Errors;
using Template.RestAPI.Filters;
using Template.RestAPI.Helpers;
using Template.RestAPI.Mappers;

namespace Template.RestAPI
{
	/// <summary>
	/// Program
	/// </summary>
	public class Program
	{
		private static readonly string[] SupportedVersions = ["0.1"];

		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Configuration.AddEnvironmentVariables();
			builder.Services.AddOpenApi();
			AddSwagger(builder.Services);
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddAutoMapper(cfg =>
			{
				cfg.AddProfile<AutoMapperProfile>();
			});
			
			builder.Services.AddEmServices(builder.Configuration);
			AddApiVersioning(builder.Services);

			builder.Services.AddProblemDetails(options =>
				options.CustomizeProblemDetails = (problemDetailsContext) =>
				{
					var requestedApiVersion = problemDetailsContext.HttpContext.GetRequestedApiVersion();
					if (requestedApiVersion != null)
					{
						return;
					}

					var allowedVersions = SupportedVersions;
					if (problemDetailsContext?.HttpContext?.Request?.Path != null)
					{
						var segments = problemDetailsContext.HttpContext.Request.Path.ToString().Split('/');
						if (segments.Length > 1 && allowedVersions.Contains(segments[1]))
						{
							return;
						}
					}

					problemDetailsContext.ProblemDetails.Status = (int)HttpStatusCode.BadRequest;
					problemDetailsContext.ProblemDetails.Detail =
						"The API version provided is not supported or it wasn't specified.";
					problemDetailsContext.ProblemDetails.Type = "EM-CustomProblemDetails";
					problemDetailsContext.ProblemDetails.Extensions.Add("RestAPIErrors", new RestAPIErrors()
						.GetRestAPIError(
							"REST-API-BAD-VERSION",
							["The API version provided is not supported or it wasn't specified."]));
				}
			);

			// Add framework services.
			builder.Services
				.AddMvc(options =>
				{
					options.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters
						.SystemTextJsonInputFormatter>();
					options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters
						.SystemTextJsonOutputFormatter>();
					// Adds ObsoleteMethodFilter
					options.Filters.Add<ObsoleteMethodFilter>();
				})
				.AddNewtonsoftJson(opts =>
				{
					opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
				})
				.AddXmlSerializerFormatters();
			
            var app = builder.Build();
            
			app.UseExceptionHandler("/Error");
			app.UseRouting();
			app.MapOpenApi();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseMiddleware<NotFoundMiddleware>();

			app.UseSwagger(options =>
			{
				options.PreSerializeFilters.Add((swagger, httpReq) =>
				{
					swagger.Servers =
					[
						// You can add as many OpenApiServer instances as you want by creating them like below
						new() {
							// You can set the Url from the default http request data or by hard coding it
							// Url = $"{httpReq.Scheme}://{httpReq.Host.Value}",
							Url = $"https://{httpReq.Host.Value}",
							Description = "Local Tecom Net"
						},
						// You can add as many OpenApiServer instances as you want by creating them like below
						new() {
							// You can set the Url from the default http request data or by hard coding it
							// Url = $"{httpReq.Scheme}://{httpReq.Host.Value}",
							Url = $"https://{httpReq.Host.Value}/api/templateservice",
							Description = "Deployed Tecom Net"
						}
					];
				});
			});
			app.UseSwaggerUI(c =>
			{
				var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
				// Loop for map all available versions
				foreach (var description in provider.ApiVersionDescriptions)
				{
					// TODO Change the name parameter with information of this service
					c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json"
						, "TemplateService " + description.ApiVersion);
				}
			});

			//TODO: Use Https Redirection
			// app.UseHttpsRedirection();

#pragma warning disable ASP0014 // Suggest using top level route registrations
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
#pragma warning restore ASP0014 // Suggest using top level route registrations

			app.MapScalarApiReference(opt =>
			{
				opt.Title = "Template Service";
				opt.Theme = ScalarTheme.Saturn;
				opt.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
				opt.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
				// FIXME: User EndpointPrefix when it is available
				opt.EndpointPathPrefix = "/em-api/{documentName}";

			});

			if (builder.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				//TODO: Enable production exception handling
				//(https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
				app.UseHsts();
			}

			app.Run();
		}

		
		private static void AddApiVersioning(IServiceCollection builderServices)
		{
			builderServices.AddApiVersioning(setup =>
			{
				setup.DefaultApiVersion = new ApiVersion(0, 1);
				setup.AssumeDefaultVersionWhenUnspecified = true;
				setup.ReportApiVersions = true;
			}).AddApiExplorer(setup =>
			{
				setup.GroupNameFormat = "V.v";
				setup.SubstituteApiVersionInUrl = true;
			});
		}

		private static void AddSwagger(IServiceCollection builderServices)
		{
			builderServices.AddSwaggerGen(options =>
			{
				options.OperationFilter<DeprecatedVersionFilter>();
				options.IgnoreObsoleteProperties();
				options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
			});

			builderServices.ConfigureOptions<ConfigureSwaggerOptions>();
			builderServices.AddSwaggerGenNewtonsoftSupport();
		}
	}
}
