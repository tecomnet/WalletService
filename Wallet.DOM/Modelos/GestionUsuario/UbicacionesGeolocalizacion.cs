using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos.GestionUsuario;

/// <summary>
/// Representa una ubicación geolocalizada de un evento o dispositivo.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para la validación y borrado lógico.
/// </summary>
public class UbicacionesGeolocalizacion : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de propiedad para la validación de los campos de la ubicación geolocalizada.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.DecimalPropertyConstraint(
            propertyName: nameof(Latitud),
            isRequired: true,
            allowNegative: true,
            allowZero: true,
            allowPositive: true,
            allowedDecimals: 8),
        PropertyConstraint.DecimalPropertyConstraint(
            propertyName: nameof(Longitud),
            isRequired: true,
            allowNegative: true,
            allowZero: true,
            allowPositive: true,
            allowedDecimals: 8),
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Dispositivo),
            isRequired: true),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(TipoEvento),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(TipoDispositivo),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Agente),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(DireccionIp),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 45),
    ];

    /// <summary>
    /// Obtiene o establece la latitud de la ubicación.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(11, 8)")]
    public decimal Latitud { get; private set; }

    /// <summary>
    /// Obtiene o establece la longitud de la ubicación.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(11, 8)")]
    public decimal Longitud { get; private set; }

    /// <summary>
    /// Obtiene o establece el tipo de dispositivo asociado a la ubicación.
    /// </summary>
    [Required] public Dispositivo Dispositivo { get; private set; }

    /// <summary>
    /// Obtiene o establece el tipo de evento registrado en esta ubicación.
    /// </summary>
    [Required] [MaxLength(length: 100)] public string TipoEvento { get; private set; }

    /// <summary>
    /// Obtiene o establece el tipo de dispositivo que generó la ubicación.
    /// </summary>
    [Required] [MaxLength(length: 100)] public string TipoDispositivo { get; private set; }

    /// <summary>
    /// Obtiene o establece el agente (navegador, aplicación, etc.) que generó la ubicación.
    /// </summary>
    [Required] [MaxLength(length: 100)] public string Agente { get; private set; }

    /// <summary>
    /// Obtiene o establece la dirección IP desde donde se registró la ubicación.
    /// </summary>
    [Required] [MaxLength(length: 45)] public string DireccionIp { get; private set; }

    /// <summary>
    /// Obtiene o establece el ID del usuario asociado a esta ubicación.
    /// </summary>
    public int UsuarioId { get; private set; }

    /// <summary>
    /// Obtiene o establece la entidad de usuario asociada a esta ubicación.
    /// </summary>
    public virtual Usuario Usuario { get; protected internal set; }

    /// <summary>
    /// Constructor por defecto de <see cref="UbicacionesGeolocalizacion"/>.
    /// Inicializa una nueva instancia de la clase.
    /// </summary>
    public UbicacionesGeolocalizacion() : base()
    {
    }

    /// <summary>
    /// Constructor de <see cref="UbicacionesGeolocalizacion"/> para crear una nueva instancia con datos específicos.
    /// Realiza validaciones de las propiedades antes de asignarlas.
    /// </summary>
    /// <param name="latitud">La latitud de la ubicación.</param>
    /// <param name="longitud">La longitud de la ubicación.</param>
    /// <param name="dispositivo">El tipo de dispositivo asociado.</param>
    /// <param name="tipoEvento">El tipo de evento (ej. "Login", "Compra").</param>
    /// <param name="tipoDispositivo">El tipo de dispositivo (ej. "Mobile", "Desktop").</param>
    /// <param name="agente">El agente de usuario (ej. "Chrome", "Firefox").</param>
    /// <param name="direccionIp">La dirección IP del cliente.</param>
    /// <param name="creationUser">El GUID del usuario que crea este registro.</param>
    /// <param name="testCase">Opcional: un identificador para casos de prueba.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna de las propiedades no es válida.</exception>
    public UbicacionesGeolocalizacion(
        decimal latitud,
        decimal longitud,
        Dispositivo dispositivo,
        string tipoEvento,
        string tipoDispositivo,
        string agente,
        string direccionIp,
        Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para acumular errores de validación.
        List<EMGeneralException> exceptions = new();
        
        // Valida cada una de las propiedades de la ubicación geolocalizada.
        IsPropertyValid(propertyName: nameof(Latitud), value: latitud, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Longitud), value: longitud, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(TipoEvento), value: tipoEvento, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(TipoDispositivo), value: tipoDispositivo, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Agente), value: agente, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(DireccionIp), value: direccionIp, exceptions: ref exceptions);
        
        // Si hay excepciones acumuladas, se lanzan como una excepción agregada.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        
        // Asigna los valores validados a las propiedades de la instancia.
        Latitud = latitud;
        Longitud = longitud;
        Dispositivo = dispositivo;
        TipoEvento = tipoEvento;
        TipoDispositivo = tipoDispositivo;
        Agente = agente;
        DireccionIp = direccionIp;
    }
}