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
        PropertyConstraint.StringPropertyConstraint(nameof(Tipo), true, 1, 50),
        PropertyConstraint.StringPropertyConstraint(nameof(Direccion), true, 1, 20),
        PropertyConstraint.StringPropertyConstraint(nameof(Estatus), true, 1, 20),
        PropertyConstraint.StringPropertyConstraint(nameof(RefExternaId), false, 1, 100)
    ];

    [Required] public int IdBilletera { get; set; }

    [Required]
    [Column(TypeName = "decimal(19,2)")]
    public decimal Monto { get; set; }

    /// <summary>
    /// Tipo de transacción: SPEI, DEPOSITO, SERVICIO, REMESA
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; }

    /// <summary>
    /// Dirección de la transacción: Abono, Cargo
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Direccion { get; set; }

    /// <summary>
    /// Estatus de la transacción: Pendiente, Completada, Fallida
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Estatus { get; set; }

    /// <summary>
    /// Identificador externo del proveedor (UUID)
    /// </summary>
    [MaxLength(100)]
    public string? RefExternaId { get; set; }

    [ForeignKey(nameof(IdBilletera))] public virtual CuentaWallet? CuentaWallet { get; set; }

    protected BitacoraTransaccion()
    {
    }

    public BitacoraTransaccion(int idBilletera, decimal monto, string tipo, string direccion, string estatus,
        Guid creationUser, string? refExternaId = null)
        : base(creationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(nameof(Tipo), tipo, ref exceptions);
        IsPropertyValid(nameof(Direccion), direccion, ref exceptions);
        IsPropertyValid(nameof(Estatus), estatus, ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions);

        IdBilletera = idBilletera;
        Monto = monto;
        Tipo = tipo;
        Direccion = direccion;
        Estatus = estatus;
        RefExternaId = refExternaId;
    }
}
