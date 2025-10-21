using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;

namespace Wallet.DOM.Modelos;

public class Documento : ValidatablePersistentObjectLogicalDelete
{
    [Key]
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; private set; }
    
    public TipoPersona TipoPersona { get; private set; }
    
    public List<DocumentacionAdjunta> DocumentacionAdjuntas { get; private set; }
}