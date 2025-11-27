using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Wallet.RestAPI.Models;

/// <summary>
/// Request para crear o actualizar un producto de proveedor.
/// </summary>
[DataContract]
public class ProductoProveedorRequest
{
    /// <summary>
    /// SKU del producto.
    /// </summary>
    [Required]
    [DataMember(Name = "sku")]
    public string Sku { get; set; }

    /// <summary>
    /// Nombre del producto.
    /// </summary>
    [Required]
    [DataMember(Name = "nombre")]
    public string Nombre { get; set; }

    /// <summary>
    /// Monto del producto.
    /// </summary>
    [Required]
    [DataMember(Name = "monto")]
    public decimal Monto { get; set; }

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    [DataMember(Name = "descripcion")]
    public string Descripcion { get; set; }
}
