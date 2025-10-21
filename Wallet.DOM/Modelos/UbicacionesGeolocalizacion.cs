using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Enums;

namespace Wallet.DOM.Modelos;

public class UbicacionesGeolocalizacion
{
    [Key]
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Latitud { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Longitud { get; private set; }
    
    [Required]
    public Dispositivo Dispositivo { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string TipoEvento { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string TipoDispositivo { get; private set; }
    [Required]
    [MaxLength(100)]
    public string Agente { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string? DireccionIp { get; private set; }
    
}