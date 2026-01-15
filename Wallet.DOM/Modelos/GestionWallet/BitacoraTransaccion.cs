using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos.GestionWallet;

/// <summary>
/// Registro de transacciones financieras en la billetera.
/// </summary>
public class BitacoraTransaccion : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(Tipo), isRequired: true, minimumLength: 1, maximumLength: 50),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(Direccion), isRequired: true, minimumLength: 1, maximumLength: 20),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(Estatus), isRequired: true, minimumLength: 1, maximumLength: 20),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(RefExternaId), isRequired: false, minimumLength: 1, maximumLength: 100)
    ];

    [Required] public int CuentaWalletId { get; set; }

    [Required]
    [Column(TypeName = "decimal(19,2)")]
    public decimal Monto { get; set; }

    /// <summary>
    /// Tipo de transacción: SPEI, DEPOSITO, SERVICIO, REMESA
    /// </summary>
    [Required]
    [MaxLength(length: 50)]
    public string Tipo { get; set; }

    /// <summary>
    /// Dirección de la transacción: Abono, Cargo
    /// </summary>
    [Required]
    [MaxLength(length: 20)]
    public string Direccion { get; set; }

    /// <summary>
    /// Estatus de la transacción: Pendiente, Completada, Fallida
    /// </summary>
    [Required]
    [MaxLength(length: 20)]
    public string Estatus { get; set; }

    /// <summary>
    /// Identificador externo del proveedor (UUID)
    /// </summary>
    [MaxLength(length: 100)]
    public string? RefExternaId { get; set; }

    [ForeignKey(name: nameof(CuentaWalletId))] public virtual CuentaWallet? CuentaWallet { get; set; }
    
    public DetallesPagoServicio DetallesPagoServicio { get; set; }

    protected BitacoraTransaccion()
    {
    }

    public BitacoraTransaccion(int cuentaWalletId, decimal monto, string tipo, string direccion, string estatus,
        Guid creationUser, string? refExternaId = null)
        : base(creationUser: creationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Tipo), value: tipo, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Direccion), value: direccion, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Estatus), value: estatus, exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        CuentaWalletId = cuentaWalletId;
        Monto = monto;
        Tipo = tipo;
        Direccion = direccion;
        Estatus = estatus;
        RefExternaId = refExternaId;
    }
}
