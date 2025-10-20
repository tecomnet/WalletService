using System.ComponentModel.DataAnnotations;

namespace Template.DOM.Modelos;

public class TipoDocumento
{
    [Key]
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; private set; }
    
    public List<Documento> Documentos { get; private set; }
}