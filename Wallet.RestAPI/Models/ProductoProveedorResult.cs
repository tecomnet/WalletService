using System.Runtime.Serialization;

namespace Wallet.RestAPI.Models;

/// <summary>
/// Resultado de una operación con ProductoProveedor.
/// </summary>
[DataContract]
public class ProductoProveedorResult
{
    /// <summary>
    /// Identificador del producto.
    /// </summary>
    [DataMember(Name = "id")]
    public int Id { get; set; }

    /// <summary>
    /// Identificador del proveedor de servicio.
    /// </summary>
    [DataMember(Name = "proveedorServicioId")]
    public int ProveedorServicioId { get; set; }

    /// <summary>
    /// SKU del producto.
    /// </summary>
    [DataMember(Name = "sku")]
    public string Sku { get; set; }

    /// <summary>
    /// Nombre del producto.
    /// </summary>
    [DataMember(Name = "nombre")]
    public string Nombre { get; set; }

    /// <summary>
    /// Monto del producto.
    /// </summary>
    [DataMember(Name = "monto")]
    public decimal Monto { get; set; }

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    [DataMember(Name = "descripcion")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Indica si el producto está activo.
    /// </summary>
    [DataMember(Name = "isActive")]
    public bool IsActive { get; set; }
}
