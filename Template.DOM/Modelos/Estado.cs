using System.ComponentModel.DataAnnotations;
using Template.DOM.Comun;

namespace Template.DOM.Modelos;

public class Estado : ValidatablePersistentObjectLogicalDelete
{
    [Key]
    public int Id { get; private set; }
    [Required]
    [MaxLength(100)]
    public string Nombre { get; private set; }
}