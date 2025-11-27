using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class UbicacionesGeolocalizacion : ValidatablePersistentObjectLogicalDelete
{
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

    [Required]
    [Column(TypeName = "decimal(11, 8)")]
    public decimal Latitud { get; private set; }

    [Required]
    [Column(TypeName = "decimal(11, 8)")]
    public decimal Longitud { get; private set; }

    [Required] public Dispositivo Dispositivo { get; private set; }

    [Required] [MaxLength(100)] public string TipoEvento { get; private set; }

    [Required] [MaxLength(100)] public string TipoDispositivo { get; private set; }
    [Required] [MaxLength(100)] public string Agente { get; private set; }

    [Required] [MaxLength(45)] public string DireccionIp { get; private set; }

    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; }

    public UbicacionesGeolocalizacion() : base()
    {
    }

    public UbicacionesGeolocalizacion(
        decimal latitud,
        decimal longitud,
        Dispositivo dispositivo,
        string tipoEvento,
        string tipoDispositivo,
        string agente,
        string direccionIp,
        Guid creationUser,
        string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Latitud), value: latitud, ref exceptions);
        IsPropertyValid(propertyName: nameof(Longitud), value: longitud, ref exceptions);
        IsPropertyValid(propertyName: nameof(TipoEvento), value: tipoEvento, ref exceptions);
        IsPropertyValid(propertyName: nameof(TipoDispositivo), value: tipoDispositivo, ref exceptions);
        IsPropertyValid(propertyName: nameof(Agente), value: agente, ref exceptions);
        IsPropertyValid(propertyName: nameof(DireccionIp), value: direccionIp, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Seteo de propiedades
        Latitud = latitud;
        Longitud = longitud;
        Dispositivo = dispositivo;
        TipoEvento = tipoEvento;
        TipoDispositivo = tipoDispositivo;
        Agente = agente;
        DireccionIp = direccionIp;
    }
}