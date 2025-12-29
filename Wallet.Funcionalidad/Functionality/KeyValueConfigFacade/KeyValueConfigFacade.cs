using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.KeyValueConfigFacade;

/// <summary>
/// Implementación de la fachada para gestionar configuraciones clave-valor.
/// </summary>
public class KeyValueConfigFacade(ServiceDbContext context) : IKeyValueConfigFacade
{
    /// <inheritdoc />
    public async Task<KeyValueConfig> GuardarKeyValueConfigAsync(string key, string value, Guid creationUser,
        string? testCase = null)
    {
        try
        {
            // Verifica si ya existe una configuración con la misma clave
            var existingConfig = await context.KeyValueConfig
                .FirstOrDefaultAsync(predicate: x => x.Key == key);

            if (existingConfig != null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.KeyValueConfigYaExiste,
                    dynamicContent: [key],
                    module: this.GetType().Name));
            }

            var config = new KeyValueConfig(key: key, value: value, creationUser: creationUser, testCase: testCase);

            await context.KeyValueConfig.AddAsync(entity: config);
            await context.SaveChangesAsync();

            return config;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<KeyValueConfig> ObtenerKeyValueConfigPorKeyAsync(string key)
    {
        try
        {
            var config = await context.KeyValueConfig
                .FirstOrDefaultAsync(predicate: x => x.Key == key);

            if (config == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.KeyValueConfigNoEncontrado,
                    dynamicContent: [key],
                    module: this.GetType().Name));
            }

            return config;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<List<KeyValueConfig>> ObtenerTodasLasConfiguracionesAsync()
    {
        try
        {
            return await context.KeyValueConfig.ToListAsync();
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<KeyValueConfig> ActualizarKeyValueConfigAsync(string key, string value, string? concurrencyToken,
        Guid modificationUser,
        string? testCase = null)
    {
        try
        {
            var config = await ObtenerKeyValueConfigPorKeyAsync(key: key);

            // Validar que la configuración esté activa
            if (!config.IsActive)
            {
                throw new EMGeneralAggregateException(
                    exception: DomCommon.BuildEmGeneralException(errorCode: ServiceErrorsBuilder.KeyValueConfigInactivo,
                        dynamicContent: [key], module: this.GetType().Name));
            }

            // Manejo de ConcurrencyToken
            if (!string.IsNullOrEmpty(value: concurrencyToken))
            {
                context.Entry(entity: config).Property(propertyExpression: x => x.ConcurrencyToken).OriginalValue =
                    Convert.FromBase64String(s: concurrencyToken);
            }

            config.Update(value: value, modificationUser: modificationUser);

            await context.SaveChangesAsync();

            return config;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ConcurrencyError,
                dynamicContent: []));
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<KeyValueConfig> EliminarKeyValueConfigAsync(string key, Guid modificationUser)
    {
        try
        {
            var config = await ObtenerKeyValueConfigPorKeyAsync(key: key);

            config.Deactivate(modificationUser: modificationUser);

            await context.SaveChangesAsync();

            return config;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<KeyValueConfig> ActivarKeyValueConfigAsync(string key, Guid modificationUser)
    {
        try
        {
            var config = await ObtenerKeyValueConfigPorKeyAsync(key: key);

            config.Activate(modificationUser: modificationUser);

            await context.SaveChangesAsync();

            return config;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}
