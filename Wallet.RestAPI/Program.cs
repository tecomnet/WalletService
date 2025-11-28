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
using Wallet.Funcionalidad;
using Wallet.RestAPI.Errors;
using Wallet.RestAPI.Filters;
using Wallet.RestAPI.Helpers;
using Wallet.RestAPI.Mappers;

namespace Wallet.RestAPI
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
			var builder = WebApplication.CreateBuilder(args: args);
			builder.Configuration.AddEnvironmentVariables();
			builder.Services.AddOpenApi();
			AddSwagger(builderServices: builder.Services);
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddAutoMapper(configAction: cfg => { cfg.AddProfile<AutoMapperProfile>(); });

			builder.Services.AddEmServices(configuration: builder.Configuration);
			AddApiVersioning(builderServices: builder.Services);

			builder.Services.AddAuthentication(configureOptions: options =>
				{
					options.DefaultAuthenticateScheme =
						Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme =
						Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(configureOptions: options =>
				{
					options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = builder.Configuration[key: "Jwt:Issuer"],
						ValidAudience = builder.Configuration[key: "Jwt:Audience"],
						IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
							key: System.Text.Encoding.UTF8.GetBytes(s: builder.Configuration[key: "Jwt:Key"]))
					};
				});

			builder.Services.AddProblemDetails(configure: options =>
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
						var segments = problemDetailsContext.HttpContext.Request.Path.ToString().Split(separator: '/');
						if (segments.Length > 1 && allowedVersions.Contains(value: segments[1]))
						{
							return;
						}
					}

					problemDetailsContext.ProblemDetails.Status = (int)HttpStatusCode.BadRequest;
					problemDetailsContext.ProblemDetails.Detail =
						"The API version provided is not supported or it wasn't specified.";
					problemDetailsContext.ProblemDetails.Type = "EM-CustomProblemDetails";
					problemDetailsContext.ProblemDetails.Extensions.Add(key: "RestAPIErrors", value: new RestAPIErrors()
						.GetRestAPIError(
							errorCode: "REST-API-BAD-VERSION",
							dynamicContent: ["The API version provided is not supported or it wasn't specified."]));
				}
			);

			// Add framework services.
			builder.Services
				.AddMvc(setupAction: options =>
				{
					options.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters
						.SystemTextJsonInputFormatter>();
					options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters
						.SystemTextJsonOutputFormatter>();
					// Adds ObsoleteMethodFilter
					options.Filters.Add<ObsoleteMethodFilter>();
					// AÑADIDO: Agrega el filtro global para el manejo de excepciones de negocio/sistema.
					options.Filters.Add<ServiceExceptionFilter>();
				})
				.AddNewtonsoftJson(setupAction: opts =>
				{
					opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					opts.SerializerSettings.Converters.Add(item: new StringEnumConverter(namingStrategy: new CamelCaseNamingStrategy()
						{ OverrideSpecifiedNames = true }));
				})
				.AddXmlSerializerFormatters();

			var app = builder.Build();

			app.UseExceptionHandler(errorHandlingPath: "/Error");
			app.UseRouting();
			app.MapOpenApi();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseMiddleware<NotFoundMiddleware>();

			app.UseSwagger(setupAction: options =>
			{
				options.PreSerializeFilters.Add(item: (swagger, httpReq) =>
				{
					swagger.Servers =
					[
						// You can add as many OpenApiServer instances as you want by creating them like below
						new()
						{
							// You can set the Url from the default http request data or by hard coding it
							// Url = $"{httpReq.Scheme}://{httpReq.Host.Value}",
							Url = $"https://{httpReq.Host.Value}",
							Description = "Local Tecom Net"
						},
						// You can add as many OpenApiServer instances as you want by creating them like below
						new()
						{
							// You can set the Url from the default http request data or by hard coding it
							// Url = $"{httpReq.Scheme}://{httpReq.Host.Value}",
							Url = $"https://{httpReq.Host.Value}",
							Description = "Deployed Tecom Net"
						}
					];
				});
			});
			app.UseSwaggerUI(setupAction: c =>
			{
				var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
				// Loop for map all available versions
				foreach (var description in provider.ApiVersionDescriptions)
				{
					// TODO Change the name parameter with information of this service
					c.SwaggerEndpoint(url: $"/swagger/{description.GroupName}/swagger.json"
						, name: "WalletService " + description.ApiVersion);
				}
			});

			//TODO: Use Https Redirection
			// app.UseHttpsRedirection();

#pragma warning disable ASP0014 // Suggest using top level route registrations
			app.UseEndpoints(configure: endpoints => { endpoints.MapControllers(); });
#pragma warning restore ASP0014 // Suggest using top level route registrations

			app.MapScalarApiReference(configureOptions: opt =>
			{
				opt.Title = "Wallet Service";
				opt.Theme = ScalarTheme.Saturn;
				opt.DefaultHttpClient = new(key: ScalarTarget.CSharp, value: ScalarClient.HttpClient);
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
			builderServices.AddApiVersioning(setupAction: setup =>
			{
				setup.DefaultApiVersion = new ApiVersion(majorVersion: 0, minorVersion: 1);
				setup.AssumeDefaultVersionWhenUnspecified = true;
				setup.ReportApiVersions = true;
			}).AddApiExplorer(setupAction: setup =>
			{
				setup.GroupNameFormat = "V.v";
				setup.SubstituteApiVersionInUrl = true;
			});
		}

		private static void AddSwagger(IServiceCollection builderServices)
		{
			builderServices.AddSwaggerGen(setupAction: options =>
			{
				options.OperationFilter<DeprecatedVersionFilter>();
				options.IgnoreObsoleteProperties();
				options.CustomSchemaIds(schemaIdSelector: type => type.FullName?.Replace(oldValue: "+", newValue: "."));
			});

			builderServices.ConfigureOptions<ConfigureSwaggerOptions>();
			builderServices.AddSwaggerGenNewtonsoftSupport();
		}
	}
}
