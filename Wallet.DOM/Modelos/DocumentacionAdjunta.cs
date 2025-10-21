using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;

namespace Wallet.DOM.Modelos;

public class DocumentacionAdjunta :  ValidatablePersistentObjectLogicalDelete
{
    [Key]
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(500)]
    public string ArchivoAWS { get; private set; }
    
    public Documento Documento { get; private set; }
}