using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos.GestionWallet;

/// <summary>
/// Representa una tarjeta emitida por la plataforma (Producto FINTECH).
/// </summary>
public class TarjetaEmitida : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(TokenProcesador), isRequired: true,
            minimumLength: 1, maximumLength: 255),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(PanEnmascarado), isRequired: true,
            minimumLength: 4, maximumLength: 20),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(NombreImpreso), isRequired: false,
            minimumLength: 1, maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(NumeroGuia), isRequired: false,
            minimumLength: 1, maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(Paqueteria), isRequired: false,
            minimumLength: 1, maximumLength: 50),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(MotivoCancelacion), isRequired: false,
            minimumLength: 1, maximumLength: 255)
    ];

    [Required] public int IdCuentaWallet { get; private set; }

    /// <summary>
    /// Token del procesador emisor (LKL, etc.)
    /// </summary>
    [Required]
    [MaxLength(length: 255)]
    public string TokenProcesador { get; private set; }

    /// <summary>
    /// PAN enmascarado para mostrar al usuario (e.g. **** 1234).
    /// </summary>
    [Required]
    [MaxLength(length: 20)]
    public string PanEnmascarado { get; private set; }

    [Required] public TipoTarjeta Tipo { get; private set; }

    [Required] public EstadoTarjeta Estado { get; private set; }

    [Required] public DateTime FechaExpiracion { get; private set; }

    /// <summary>
    /// Si el usuario ha congelado la tarjeta temporalmente.
    /// </summary>
    [Required]
    public bool BloqueoTemporal { get; private set; }

    /// <summary>
    /// Límite de gastos diario configurado por el usuario.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(19,2)")]
    public decimal LimiteDiario { get; private set; }

    /// <summary>
    /// Si la tarjeta puede realizar compras en línea.
    /// </summary>
    [Required]
    public bool ComprasEnLineaHabilitadas { get; private set; }

    /// <summary>
    /// Si la tarjeta puede realizar retiros en cajero.
    /// </summary>
    [Required]
    public bool RetirosCajeroHabilitados { get; private set; }

    /// <summary>
    /// Razón textual de la cancelación (si aplica).
    /// </summary>
    [MaxLength(length: 255)]
    public string? MotivoCancelacion { get; private set; }

    // --- Logística (Solo para Físicas) ---

    public EstadoEntrega? EstadoEntrega { get; private set; }

    [MaxLength(length: 100)] public string? NumeroGuia { get; private set; }

    [MaxLength(length: 50)] public string? Paqueteria { get; private set; }

    [MaxLength(length: 100)] public string? NombreImpreso { get; private set; }

    /// <summary>
    /// Relación con la cuenta wallet de fondos.
    /// </summary>
    [ForeignKey(name: nameof(IdCuentaWallet))]
    public virtual CuentaWallet? CuentaWallet { get; set; }

    // Constructor requerido por EF Core
    protected TarjetaEmitida()
    {
    }

    public TarjetaEmitida(
        int idCuentaWallet,
        string tokenProcesador,
        string panEnmascarado,
        TipoTarjeta tipo,
        DateTime fechaExpiracion,
        Guid creationUser,
        string? nombreImpreso = null)
        : base(creationUser: creationUser)
    {
        List<EMGeneralException> exceptions = new();

        IsPropertyValid(propertyName: nameof(TokenProcesador), value: tokenProcesador, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(PanEnmascarado), value: panEnmascarado, exceptions: ref exceptions);
        if (nombreImpreso != null)
            IsPropertyValid(propertyName: nameof(NombreImpreso), value: nombreImpreso, exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        IdCuentaWallet = idCuentaWallet;
        TokenProcesador = tokenProcesador;
        PanEnmascarado = panEnmascarado;
        Tipo = tipo;
        FechaExpiracion = fechaExpiracion;
        NombreImpreso = nombreImpreso;

        // Default values
        Estado = EstadoTarjeta.Inactiva; // Nace inactiva hasta que el usuario la active
        BloqueoTemporal = false;
        LimiteDiario = 10000; // Default limit, could be parametrized
        ComprasEnLineaHabilitadas = true;
        RetirosCajeroHabilitados = true;

        if (tipo == TipoTarjeta.Fisica)
        {
            EstadoEntrega = Wallet.DOM.Enums.EstadoEntrega.Solicitada;
        }
    }

    public bool EstaVencida => DateTime.UtcNow > FechaExpiracion;

    public bool VerificarExpiracion(Guid modificationUser)
    {
        if (Estado != EstadoTarjeta.Expirada && EstaVencida)
        {
            Estado = EstadoTarjeta.Expirada;
            Update(modificationUser: modificationUser);
            return true;
        }

        return false;
    }

    public void ActivarTarjeta(Guid modificationUser)
    {
        if (Estado == EstadoTarjeta.Expirada || EstaVencida)
            throw new EMGeneralException(
                serviceError: ServiceErrorsBuilder.Instance().GetError(errorCode: ServiceErrorsBuilder.TarjetaExpirada),
                serviceName: "GestionWallet");

        Estado = EstadoTarjeta.Activa;
        Update(modificationUser: modificationUser);
    }

    public void BloquearTemporalmente(bool bloquear, Guid modificationUser)
    {
        if (Estado == EstadoTarjeta.Expirada || EstaVencida)
            throw new EMGeneralException(
                serviceError: ServiceErrorsBuilder.Instance().GetError(errorCode: ServiceErrorsBuilder.TarjetaExpirada),
                serviceName: "GestionWallet");

        BloqueoTemporal = bloquear;
        Estado = bloquear
            ? EstadoTarjeta.BloqueadaTemporalmente
            : EstadoTarjeta.Activa; // Simple state management logic
        Update(modificationUser: modificationUser);
    }

    public void ConfigurarReglas(decimal limiteDiario, bool comprasLinea, bool retiros, Guid modificationUser)
    {
        if (Estado == EstadoTarjeta.Expirada || EstaVencida)
            throw new EMGeneralException(
                serviceError: ServiceErrorsBuilder.Instance().GetError(errorCode: ServiceErrorsBuilder.TarjetaExpirada),
                serviceName: "GestionWallet");

        LimiteDiario = limiteDiario;
        ComprasEnLineaHabilitadas = comprasLinea;
        RetirosCajeroHabilitados = retiros;
        Update(modificationUser: modificationUser);
    }

    public void ActualizarLogistica(Wallet.DOM.Enums.EstadoEntrega estadoEntrega, string? numeroGuia,
        string? paqueteria,
        Guid modificationUser)
    {
        if (Tipo != TipoTarjeta.Fisica)
            throw new EMGeneralException(
                serviceError: ServiceErrorsBuilder.Instance()
                    .GetError(errorCode: ServiceErrorsBuilder.TarjetaVirtualNoLogistica),
                serviceName: "GestionWallet");

        EstadoEntrega = estadoEntrega;
        NumeroGuia = numeroGuia;
        Paqueteria = paqueteria;
        Update(modificationUser: modificationUser);
    }
}
