using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionCliente;

namespace Wallet.DOM.Modelos.GestionWallet;

/// <summary>
/// Representa una tarjeta externa vinculada por el cliente (Funding Source).
/// </summary>
public class TarjetaVinculada : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(NumeroTarjeta), isRequired: true,
            minimumLength: 13, maximumLength: 20),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(PanEnmascarado), isRequired: true,
            minimumLength: 4, maximumLength: 20),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(Alias), isRequired: true, minimumLength: 1,
            maximumLength: 50),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(GatewayCustomerId), isRequired: false,
            minimumLength: 1, maximumLength: 255)
    ];

    [Required] public int IdCuentaWallet { get; private set; }

    /// <summary>
    /// Número de tarjeta completo (Internal use only).
    /// </summary>
    [Required]
    [MaxLength(length: 20)]
    public string NumeroTarjeta { get; private set; }

    /// <summary>
    /// ID del cliente en la pasarela (e.g. cus_12345).
    /// </summary>
    [MaxLength(length: 255)]
    public string? GatewayCustomerId { get; private set; }

    /// <summary>
    /// PAN enmascarado (e.g. **** 1234).
    /// </summary>
    [Required]
    [MaxLength(length: 20)]
    public string PanEnmascarado { get; private set; }

    /// <summary>
    /// Alias amigable dado por el usuario (e.g. "Nómina").
    /// </summary>
    [Required]
    [MaxLength(length: 50)]
    public string Alias { get; private set; }

    [Required] public MarcaTarjeta Marca { get; private set; }

    [Required] public DateTime FechaExpiracion { get; private set; }

    /// <summary>
    /// Si es la tarjeta favorita para fondeo.
    /// </summary>
    [Required]
    public bool EsFavorita { get; private set; }

    /// <summary>
    /// Relación con la Cuenta Wallet propietaria.
    /// </summary>
    [ForeignKey(name: nameof(IdCuentaWallet))]
    public virtual CuentaWallet? CuentaWallet { get; set; }

    protected TarjetaVinculada()
    {
    }

    public TarjetaVinculada(
        int idCuentaWallet,
        string numeroTarjeta,
        string alias,
        MarcaTarjeta marca,
        DateTime fechaExpiracion,
        Guid creationUser,
        string? gatewayCustomerId = null)
        : base(creationUser: creationUser)
    {
        List<EMGeneralException> exceptions = new();

        IsPropertyValid(propertyName: nameof(NumeroTarjeta), value: numeroTarjeta, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Alias), value: alias, exceptions: ref exceptions);
        if (gatewayCustomerId != null)
            IsPropertyValid(propertyName: nameof(GatewayCustomerId), value: gatewayCustomerId,
                exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        IdCuentaWallet = idCuentaWallet;
        NumeroTarjeta = numeroTarjeta;
        // Generate Masked PAN: **** 1234
        var last4 = numeroTarjeta.Length > 4 ? numeroTarjeta.Substring(numeroTarjeta.Length - 4) : numeroTarjeta;
        PanEnmascarado = $"**** {last4}";

        Alias = alias;
        Marca = marca;
        FechaExpiracion = fechaExpiracion;
        GatewayCustomerId = gatewayCustomerId;
        EsFavorita = false;
    }

    public void EstablecerComoFavorita(bool favorita, Guid modificationUser)
    {
        EsFavorita = favorita;
        Update(modificationUser: modificationUser);
    }

    public void Reactivar(
        string alias,
        MarcaTarjeta marca,
        DateTime fechaExpiracion,
        string? gatewayCustomerId,
        Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();

        IsPropertyValid(propertyName: nameof(Alias), value: alias, exceptions: ref exceptions);
        if (gatewayCustomerId != null)
            IsPropertyValid(propertyName: nameof(GatewayCustomerId), value: gatewayCustomerId,
                exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        Alias = alias;
        Marca = marca;
        FechaExpiracion = fechaExpiracion;
        if (gatewayCustomerId != null) GatewayCustomerId = gatewayCustomerId;

        Activate(modificationUser);
    }

    public void ActualizarAlias(string nuevoAlias, Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Alias), value: nuevoAlias, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        Alias = nuevoAlias;
        Update(modificationUser: modificationUser);
    }
}
