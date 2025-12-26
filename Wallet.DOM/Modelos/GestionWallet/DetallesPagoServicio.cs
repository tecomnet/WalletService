using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos.GestionWallet;

/// <summary>
/// Detalles adicionales para transacciones de pago de servicios.
/// </summary>
public class DetallesPagoServicio : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(nameof(NumeroReferencia), true, 1, 100),
        PropertyConstraint.StringPropertyConstraint(nameof(CodigoAutorizacion), false, 1, 100)
    ];

    [Required]
    public int
        IdTransaccion { get; set; } // Changed to int to match BitacoraTransaccion.Id (inherited from PersistentClass)

    [Required] public int IdProveedor { get; set; }

    /// <summary>
    /// Número de referencia del servicio (Contrato, Teléfono, etc.)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string NumeroReferencia { get; set; }

    /// <summary>
    /// Código de autorización devuelto por el proveedor
    /// </summary>
    [MaxLength(100)]
    public string? CodigoAutorizacion { get; set; }

    [ForeignKey(nameof(IdTransaccion))] public virtual BitacoraTransaccion? Transaccion { get; set; }

    protected DetallesPagoServicio()
    {
    }

    public DetallesPagoServicio(int idTransaccion, int idProveedor, string numeroReferencia, Guid creationUser,
        string? codigoAutorizacion = null)
        : base(creationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(nameof(NumeroReferencia), numeroReferencia, ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions);

        IdTransaccion = idTransaccion;
        IdProveedor = idProveedor;
        NumeroReferencia = numeroReferencia;
        CodigoAutorizacion = codigoAutorizacion;
    }
}
