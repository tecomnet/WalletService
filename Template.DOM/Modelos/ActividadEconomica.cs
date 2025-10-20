using System.ComponentModel.DataAnnotations;

namespace Template.DOM.Modelos;

public class ActividadEconomica
{
    [Key]
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; private set; }
    [Required]
    public decimal Ingreso {get; private set;}
    [Required]
    [MaxLength(100)]
    public string OrigenRecurso {get; private set;}
    [Required]
    [MaxLength(500)] 
    public string ArchivoAWS {get; private set;}
}   