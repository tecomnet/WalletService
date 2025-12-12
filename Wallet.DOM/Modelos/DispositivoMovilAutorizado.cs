using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa un dispositivo móvil autorizado en el sistema.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para proporcionar validación y eliminación lógica.
/// </summary>
public class DispositivoMovilAutorizado : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de propiedad para la validación de las propiedades de la clase.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Token),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(IdDispositivo),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Caracteristicas),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
    ];

    /// <summary>
    /// Obtiene el token único del dispositivo móvil.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Token { get; private set; }

    /// <summary>
    /// Obtiene el identificador único del dispositivo móvil.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string IdDispositivo { get; private set; }

    /// <summary>
    /// Obtiene el nombre asignado al dispositivo móvil.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Nombre { get; private set; }

    /// <summary>
    /// Obtiene las características o descripción del dispositivo móvil.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Caracteristicas { get; private set; }

    /// <summary>
    /// Indica si este dispositivo es el dispositivo móvil actualmente activo o en uso.
    /// </summary>
    [Required]
    public bool Actual { get; private set; }

    /// <summary>
    /// Obtiene el identificador del usuario al que pertenece este dispositivo.
    /// </summary>
    public int UsuarioId { get; private set; }

    /// <summary>
    /// Obtiene el objeto <see cref="Usuario"/> asociado a este dispositivo.
    /// </summary>
    public Usuario Usuario { get; private set; }

    /// <summary>
    /// Constructor por defecto de la clase <see cref="DispositivoMovilAutorizado"/>.
    /// Inicializa la instancia base.
    /// </summary>
    public DispositivoMovilAutorizado() : base()
    {
    }

    /// <summary>
    /// Constructor para crear una nueva instancia de <see cref="DispositivoMovilAutorizado"/> con los datos especificados.
    /// Realiza la validación de las propiedades antes de asignarlas.
    /// </summary>
    /// <param name="token">El token único del dispositivo.</param>
    /// <param name="idDispositivo">El identificador único del dispositivo.</param>
    /// <param name="nombre">El nombre del dispositivo.</param>
    /// <param name="caracteristicas">Las características o descripción del dispositivo.</param>
    /// <param name="creationUser">El GUID del usuario que crea este registro.</param>
    /// <param name="testCase">Opcional: Un caso de prueba para propósitos de desarrollo/pruebas.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna de las propiedades no es válida.</exception>
    public DispositivoMovilAutorizado(
        string token,
        string idDispositivo,
        string nombre,
        string caracteristicas,
        Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para acumular errores de validación
        List<EMGeneralException> exceptions = new();
        // Valida cada una de las propiedades del dispositivo
        IsPropertyValid(propertyName: nameof(Token), value: token, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(IdDispositivo), value: idDispositivo, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Caracteristicas), value: caracteristicas, exceptions: ref exceptions);
        // Si hay excepciones acumuladas, se lanza una excepción agregada
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna los valores a las propiedades después de la validación exitosa
        Token = token;
        IdDispositivo = idDispositivo;
        Nombre = nombre;
        Caracteristicas = caracteristicas;
        Actual = true; // Por defecto, el dispositivo recién creado es el actual
    }

    /// <summary>
    /// Marca este dispositivo móvil como no actual.
    /// </summary>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void MarcarComoNoActual(Guid modificationUser)
    {
        if (!Actual) return;

        Actual = false;
        base.Update(modificationUser: modificationUser);
    }
}