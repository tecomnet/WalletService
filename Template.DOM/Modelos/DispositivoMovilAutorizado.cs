using System.ComponentModel.DataAnnotations;
using Template.DOM.Comun;

namespace Template.DOM.Modelos;

public class DispositivoMovilAutorizado :ValidatablePersistentObjectLogicalDelete
{
    [Key]
    public int Id { get; private set; }
    [Required]
    [MaxLength(100)]
    public string Token { get; private set; }
    [Required]
    [MaxLength(100)]
    public string IdDispositivo { get; private set; }
    [Required]
    [MaxLength(100)]
    public string Nombre { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Caracteristicas { get; private set; }
    
    [Required]
    public bool Actual { get; private set; }
    
}