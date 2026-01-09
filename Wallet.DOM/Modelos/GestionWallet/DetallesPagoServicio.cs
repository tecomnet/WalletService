using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionEmpresa;

namespace Wallet.DOM.Modelos.GestionWallet;

/// <summary>
/// Detalles adicionales para transacciones de pago de servicios.
/// </summary>
public class DetallesPagoServicio : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(NumeroReferencia), isRequired: true, minimumLength: 1, maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(propertyName: nameof(CodigoAutorizacion), isRequired: false, minimumLength: 1, maximumLength: 100)
    ];

    [Required]
    public int
        IdTransaccion { get; set; } // Changed to int to match BitacoraTransaccion.Id (inherited from PersistentClass)

    [Required] public int IdProducto { get; set; }

    /// <summary>
    /// Número de referencia del servicio (Contrato, Teléfono, etc.)
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string NumeroReferencia { get; set; }

    /// <summary>
    /// Código de autorización devuelto por el proveedor
    /// </summary>
    [MaxLength(length: 100)]
    public string? CodigoAutorizacion { get; set; }

    [ForeignKey(name: nameof(IdTransaccion))] public virtual BitacoraTransaccion? Transaccion { get; set; }
    
    public virtual Producto? Producto { get; set; }

    protected DetallesPagoServicio()
    {
    }

    public DetallesPagoServicio(int idTransaccion, int idProducto, string numeroReferencia, Guid creationUser,
        string? codigoAutorizacion = null)
        : base(creationUser: creationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(NumeroReferencia), value: numeroReferencia, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        IdTransaccion = idTransaccion;
        IdProducto = idProducto;
        NumeroReferencia = numeroReferencia;
        CodigoAutorizacion = codigoAutorizacion;
    }
}
