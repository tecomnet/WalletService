using System.ComponentModel.DataAnnotations;
using Template.DOM.Comun;

namespace Template.DOM.Modelos;

public class Direccion : ValidatablePersistentObjectLogicalDelete
{
    [Key]
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(5)]
    public string CodigoPostal { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Pais { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Estado { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Municipio { get; private set; }   
    
    [Required]
    [MaxLength(100)]
    public string Colonia { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Calle { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string NumeroExterior { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string NumeroInterior { get; private set; }
    
    [MaxLength(250)]
    public string? Referencia { get; private set; }
    
    public Cliente Cliente { get; private set; }
}