using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.ServiceClient;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Fachada que implementa la lógica de negocio relacionada con los clientes.
/// Gestiona la creación, actualización, consulta y eliminación lógica de clientes.
/// </summary>
public class ClienteFacade(
    ServiceDbContext context,
    IChecktonPldServiceFacade checktonPldService,
    IEmpresaFacade empresaFacade,
    IEstadoFacade estadoFacade) : IClienteFacade
{
    /// <inheritdoc />
    public async Task<Cliente> ObtenerClientePorIdAsync(int idCliente)
    {
        try
        {
            // Obtener cliente
            var cliente = await context.Cliente.Include(navigationPropertyPath: x => x.Direccion)
                .Include(navigationPropertyPath: x => x.Usuario)
                .ThenInclude(navigationPropertyPath: u => u.DispositivoMovilAutorizados)
                .Include(navigationPropertyPath: x => x.Usuario)
                .ThenInclude(navigationPropertyPath: u => u.Verificaciones2Fa)
                .Include(navigationPropertyPath: x => x.Estado).Include(navigationPropertyPath: x => x.Usuario)
                .ThenInclude(navigationPropertyPath: u => u.Empresa)
                .FirstOrDefaultAsync(predicate: x => x.Id == idCliente);
            // Validar cliente
            if (cliente == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ClienteNoEncontrado,
                    dynamicContent: [idCliente],
                    module: this.GetType().Name));
            }

            // Retornar cliente
            return cliente;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }


    /// <inheritdoc />
    public async Task<Cliente> ActualizarClienteDatosPersonalesAsync(
        int idCliente,
        string nombre,
        string primerApellido,
        string segundoApellido,
        string nombreEstado,
        DateOnly fechaNacimiento,
        Genero genero,
        Guid modificationUser,
        string? testCase = null)
    {
        try
        {
            // TODO: Crear cliente si no existe

            // Obtener cliente 
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            // Actualizar datos personales
            cliente.AgregarDatosPersonales(
                nombre: nombre,
                primerApellido: primerApellido,
                segundoApellido: segundoApellido,
                fechaNacimiento: fechaNacimiento,
                genero: genero,
                modificationUser: modificationUser);
            // Validar con checkton pld
            var (validacionCheckton, curpGenerada) = await ValidaDatosPersonalesChecktonPldAsync(
                nombre: nombre,
                primerApellido: primerApellido,
                segundoApellido: segundoApellido,
                fechaNacimiento: new DateTime(year: fechaNacimiento.Year, month: fechaNacimiento.Month,
                    day: fechaNacimiento.Day),
                genero: genero,
                nombreEstado: nombreEstado,
                creationUser: modificationUser,
                testCase: testCase);
            // Agrega validacion checkton
            cliente.AgregarValidacionCheckton(validacion: validacionCheckton, modificationUser: modificationUser);
            // Agrega curp
            cliente.AgregarCurp(curp: curpGenerada, modificationUser: modificationUser);
            // Cargar los codigos de verificacion
            await context.Entry(entity: cliente.Usuario)
                .Collection(propertyExpression: c => c.Verificaciones2Fa)
                .LoadAsync();
            // Validar que ya haya confirmado el código de verificación por SMS
            ValidarConfirmacionCodigoVerificacionSMS2FA(cliente: cliente);
            // TODO EMD: UBICARLO EN LA EMPRESA TECOMNET
            var empresa = await empresaFacade.ObtenerPorNombreAsync(nombre: "Tecomnet");
            cliente.Usuario.AgregarEmpresa(empresa: empresa, modificationUser: modificationUser);
            // Agregar estado
            var estado = await estadoFacade.ObtenerEstadoPorNombreAsync(nombre: nombreEstado);
            cliente.AgregarEstado(estado: estado, modificationUser: modificationUser);
            // Agregar direccion pre-registro, TODO EMD: PENDIENTE RECIBIR EL PAIS O IMPLEMENTAR EL CATALOGO PAIS
            var preDireccion = await CrearDireccionPreRegistro(pais: "México", estado: nombreEstado,
                creationUser: modificationUser, testCase: testCase);
            // Agrega pre direccion
            cliente.AgregarDireccion(direccion: preDireccion, creationUser: modificationUser);
            // Guardar cambios
            await context.SaveChangesAsync();
            // Retornar cliente
            return cliente;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }


    /// <inheritdoc />
    public async Task<Cliente> EliminarClienteAsync(int idCliente, Guid modificationUser)
    {
        try
        {
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            cliente.Deactivate(modificationUser: modificationUser);
            cliente.Usuario.Deactivate(modificationUser: modificationUser); // Also deactivate Usuario? Maybe.
            await context.SaveChangesAsync();
            return cliente;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Cliente> ActivarClienteAsync(int idCliente, Guid modificationUser)
    {
        try
        {
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            cliente.Activate(modificationUser: modificationUser);
            cliente.Usuario.Activate(modificationUser: modificationUser); // Also activate Usuario? Maybe.
            await context.SaveChangesAsync();
            return cliente;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<List<Cliente>> ObtenerClientesAsync()
    {
        try
        {
            // Obtener lista de clientes
            var clientes = await context.Cliente.Include(navigationPropertyPath: c => c.Usuario).ToListAsync();
            // Retornar lista de clientes
            return clientes;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }


    #region Metodos privados

    /// <summary>
    /// Valida los datos personales del cliente utilizando el servicio Checkton PLD.
    /// </summary>
    /// <param name="nombre">Nombre del cliente.</param>
    /// <param name="primerApellido">Primer apellido.</param>
    /// <param name="segundoApellido">Segundo apellido.</param>
    /// <param name="fechaNacimiento">Fecha de nacimiento.</param>
    /// <param name="genero">Género.</param>
    /// <param name="nombreEstado">Estado de residencia.</param>
    /// <param name="creationUser">Usuario que crea la validación.</param>
    /// <param name="testCase">Caso de prueba opcional.</param>
    /// <returns>Una tupla con el objeto de validación y la CURP generada.</returns>
    private async Task<(ValidacionCheckton, string)> ValidaDatosPersonalesChecktonPldAsync(
        string nombre, string primerApellido, string segundoApellido, DateTime fechaNacimiento, Genero genero,
        string nombreEstado, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Validar con checkton pld
            var validacionCurpResult = await checktonPldService.ValidarChecktonPld(
                nombre: nombre,
                primerApellido: primerApellido,
                segundoApellido: segundoApellido,
                fechaNacimiento: fechaNacimiento,
                genero: genero,
                estado: nombreEstado);
            // Validar que no haya error en la validacion
            if (!validacionCurpResult.Success)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ClienteChecktonPldError,
                    dynamicContent: [],
                    module: this.GetType().Name));
            }

            // Crea la validacion con el resultado
            ValidacionCheckton validacionCheckton = new ValidacionCheckton(
                tipoCheckton: TipoCheckton.Curp,
                resultado: validacionCurpResult.Success,
                creationUser: creationUser,
                testCase: testCase);
            // Retorna codigo de verificacion
            return (validacionCheckton, validacionCurpResult.CurpGenerada);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <summary>
    /// Crea una dirección de pre-registro para el cliente.
    /// </summary>
    /// <param name="pais">País de la dirección.</param>
    /// <param name="estado">Estado de la dirección.</param>
    /// <param name="creationUser">Usuario que crea el registro.</param>
    /// <param name="testCase">Caso de prueba opcional.</param>
    /// <returns>El objeto <see cref="Direccion"/> creado.</returns>
    private async Task<Direccion> CrearDireccionPreRegistro(string pais, string estado, Guid creationUser,
        string? testCase = null)
    {
        try
        {
            // Localiza estado 
            var estadoExiste = await estadoFacade.ObtenerEstadoPorNombreAsync(nombre: estado);
            // Crea la direccion
            var direccion = new Direccion(
                pais: pais,
                estado: estadoExiste.Nombre,
                creationUser: creationUser,
                testCase: testCase);
            // Return direccion
            return direccion;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <summary>
    /// Valida si el cliente se encuentra activo.
    /// </summary>
    /// <param name="cliente">El cliente a validar.</param>
    /// <exception cref="EMGeneralAggregateException">Si el cliente está inactivo.</exception>
    private void ValidarClienteActivo(Cliente cliente)
    {
        if (!cliente.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteInactivo,
                dynamicContent: [cliente.NombreCompleto!],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Valida que el cliente tenga confirmado el código de verificación por SMS.
    /// </summary>
    /// <param name="cliente">El cliente a validar.</param>
    /// <exception cref="EMGeneralAggregateException">Si no tiene confirmación SMS.</exception>
    private void ValidarConfirmacionCodigoVerificacionSMS2FA(Cliente cliente)
    {
        // Obtiene el código de verificación SMS
        var confirmacionSmsCode =
            cliente.Usuario.Verificaciones2Fa.FirstOrDefault(predicate: x =>
                x is { Tipo: Tipo2FA.Sms, Verificado: true });
        // Valida que exista el código de verificación SMS
        if (confirmacionSmsCode is null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.Verificacion2FASMSNoConfirmado,
                dynamicContent: [],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Verifica si un cliente ya ha realizado la verificación por SMS.
    /// </summary>
    /// <param name="codigoPais">Código de país del teléfono.</param>
    /// <param name="telefono">Número de teléfono.</param>
    /// <returns>True si ya está verificado, False en caso contrario.</returns>
    private async Task<bool> ClienteYaVerificoPorSmsAsync(string codigoPais, string telefono)
    {
        var estaVerificado = await context.Usuario
            .AnyAsync(predicate: c =>
                c.CodigoPais == codigoPais &&
                c.Telefono == telefono &&
                c.Verificaciones2Fa.Any(v => v.Tipo == Tipo2FA.Sms && v.Verificado));

        return estaVerificado;
    }

    #endregion
}
