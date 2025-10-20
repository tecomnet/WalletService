using System.ComponentModel.DataAnnotations;
using Template.DOM.Enums;

namespace Template.DOM.Modelos;

public class ValidacionCheckton
{
    [Key]
    public int Id { get; private set; }
    [Required]
    public TipoCheckton TipoCheckton { get; private set; }
    [Required]
    public bool Resultado { get; private set; }
}