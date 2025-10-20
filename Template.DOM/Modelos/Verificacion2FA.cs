using System.ComponentModel.DataAnnotations;
using Template.DOM.Comun;
using Template.DOM.Enums;

namespace Template.DOM.Modelos;

public class Verificacion2FA : ValidatablePersistentObjectLogicalDelete
{
    [Key]
    public int Id { get; private set; }
    [Required]
    [MaxLength(4)]
    public string Codigo { get; private set; }
    [Required]
    public DateTime FechaVencimiento { get; private set; }
    [Required]
    public Tipo2FA Tipo { get; private set; }
    [Required]
    public bool Verificado { get; private set; }
}