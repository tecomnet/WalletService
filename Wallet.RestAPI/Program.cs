using System;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Scalar.AspNetCore;
using Wallet.Funcionalidad;
using Wallet.RestAPI.Filters;
using Wallet.RestAPI.Helpers;
using Wallet.RestAPI.Mappers;

// Inicializa el constructor de la aplicación web.
var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la aplicación
// Agrega variables de entorno a la configuración, permitiendo sobreescribir valores desde el entorno.
builder.Configuration.AddEnvironmentVariables();

// 2. Registro de Servicios
// Configuración de los controladores API y los serializadores JSON/XML.
builder.Services.AddControllers(options =>
	{
		// Elimina los formatters de System.Text.Json para asegurar el uso exclusivo de Newtonsoft.Json.
		options.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>();
		options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter>();

		// Registro de filtros globales para la aplicación.
		options.Filters.Add<ObsoleteMethodFilter>(); // Filtro para manejar métodos obsoletos.
		options.Filters.Add<ServiceExceptionFilter>(); // Filtro para capturar y manejar excepciones de servicio.
	})
	// Configura Newtonsoft.Json como el serializador principal.
	.AddNewtonsoftJson(opts =>
	{
		// Configura la resolución de nombres de propiedades a camelCase.
		opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		// Agrega un conversor para enumeraciones que las serializa como cadenas en camelCase.
		opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy
			{ OverrideSpecifiedNames = true }));
	})
	// Agrega soporte para serialización y deserialización XML.
	.AddXmlSerializerFormatters();

// Configura el explorador de endpoints API para generar la documentación de la API.
builder.Services.AddEndpointsApiExplorer();
// Configura el versionado de la API llamando a un método auxiliar.
AddApiVersioning(builder.Services);

// Configuración de OpenAPI / Swagger para la documentación interactiva de la API.
builder.Services.AddOpenApi();
// Agrega servicios relacionados con Swagger llamando a un método auxiliar.
AddSwagger(builder.Services);

// Configuración de AutoMapper para mapear objetos entre diferentes capas.
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<AutoMapperProfile>(); });

// Registro de servicios personalizados del dominio de la aplicación.
builder.Services.AddEmServices(builder.Configuration);

// Configuración de la autenticación y autorización.
builder.Services.AddAuthentication(options =>
	{
		// Establece el esquema de autenticación por defecto a JWT Bearer.
		options.DefaultAuthenticateScheme =
			Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme =
			Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
	})
	// Configura la autenticación JWT Bearer.
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
		{
			ValidateIssuer = true, // Valida el emisor del token.
			ValidateAudience = true, // Valida la audiencia del token.
			ValidateLifetime = true, // Valida la vida útil del token.
			ValidateIssuerSigningKey = true, // Valida la clave de firma del emisor.
			ValidIssuer = builder.Configuration["Jwt:Issuer"], // Emisor válido configurado.
			ValidAudience = builder.Configuration["Jwt:Audience"], // Audiencia válida configurada.
			IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
				System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
				                                   throw new InvalidOperationException(
					                                   "Jwt:Key is not configured"))) // Clave de firma.
		};
	});


// Construye la aplicación a partir de la configuración y los servicios registrados.
var app = builder.Build();

// 3. Pipeline de Middleware HTTP

// Manejo de excepciones.
if (app.Environment.IsDevelopment())
{
	// En entorno de desarrollo, usa la página de excepción detallada.
	app.UseDeveloperExceptionPage();
}
else
{
	// En producción, usa un manejador de excepciones genérico.
	app.UseExceptionHandler("/Error");
	// HSTS (HTTP Strict Transport Security) para forzar HTTPS.
	app.UseHsts();
}

// Redirección HTTPS (opcional, actualmente comentado).
// app.UseHttpsRedirection();

// Habilita el enrutamiento.
app.UseRouting();

// Habilita la autenticación.
app.UseAuthentication();
// Habilita la autorización.
app.UseAuthorization();

// Configuración de la UI de Swagger / OpenAPI.
// Mapea los endpoints de OpenAPI.
app.MapOpenApi();

// Configura Swagger para agregar información de servidores dinámicamente.
app.UseSwagger(options =>
{
	options.PreSerializeFilters.Add((swagger, httpReq) =>
	{
		swagger.Servers =
		[
			new() { Url = $"https://{httpReq.Host.Value}", Description = "Local Tecom Net" },
			new() { Url = $"https://{httpReq.Host.Value}", Description = "Deployed Tecom Net" }
		];
	});
});

// Configura la interfaz de usuario de Swagger (Swagger UI).
app.UseSwaggerUI(c =>
{
	var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
	// Genera un endpoint de Swagger para cada versión de API disponible.
	foreach (var description in provider.ApiVersionDescriptions)
	{
		c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", "WalletService " + description.ApiVersion);
	}
});

// Mapea la referencia de API de Scalar para una documentación alternativa y mejorada.
app.MapScalarApiReference(opt =>
{
	opt.Title = "Wallet Service"; // Título del servicio.
	opt.Theme = ScalarTheme.Saturn; // Tema visual de Scalar.
	opt.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient); // Cliente HTTP por defecto.
	opt.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json"; // Patrón para los documentos OpenAPI.
	opt.EndpointPathPrefix = "/scalar/{documentName}"; // Prefijo de ruta para los endpoints de Scalar.
});

// Middleware personalizado para manejar rutas no encontradas.
app.UseMiddleware<NotFoundMiddleware>();

// Mapea los controladores a sus rutas correspondientes.
app.MapControllers();

// Inicia la aplicación.
app.Run();

// Métodos Auxiliares (Funciones Locales)

// Configura el versionado de la API para la aplicación.
void AddApiVersioning(IServiceCollection builderServices)
{
	builderServices.AddApiVersioning(setup =>
	{
		setup.DefaultApiVersion = new ApiVersion(0, 1); // Establece la versión por defecto de la API.
		setup.AssumeDefaultVersionWhenUnspecified = true; // Asume la versión por defecto si no se especifica.
		setup.ReportApiVersions = true; // Reporta las versiones de la API en los encabezados de respuesta.
	}).AddApiExplorer(setup =>
	{
		setup.GroupNameFormat = "V.v"; // Formato del nombre del grupo de la API Explorer.
		setup.SubstituteApiVersionInUrl = true; // Sustituye la versión de la API en la URL.
	});
}


// Configura los servicios de Swagger para la generación de documentación de la API.
void AddSwagger(IServiceCollection builderServices)
{
	builderServices.AddSwaggerGen(options =>
	{
		options.OperationFilter<DeprecatedVersionFilter>(); // Aplica un filtro para operaciones de versiones obsoletas.
		options.IgnoreObsoleteProperties(); // Ignora propiedades marcadas como obsoletas en los esquemas.
		options.CustomSchemaIds(type => type.FullName?.Replace("+", ".")); // Genera IDs de esquema personalizados.
	});

	// Configura opciones adicionales de Swagger, posiblemente desde un archivo de configuración.
	builderServices.ConfigureOptions<ConfigureSwaggerOptions>();
	// Agrega soporte para Newtonsoft.Json en Swagger.
	builderServices.AddSwaggerGenNewtonsoftSupport();
}

/// <summary>
/// Clase parcial Program, utilizada para la generación de código por el compilador.
/// </summary>
public partial class Program
{
}
