using System.ComponentModel.DataAnnotations;
using Template.DOM.Comun;
using Template.DOM.Enums;

namespace Template.DOM.Modelos;

public class Cliente : ValidatablePersistentObjectLogicalDelete
{
    
    
    
    
    
    
    [Key]
    public int Id { get; private set; }
    [Required]
    [MaxLength(3)]
    public  string CodigoPais { get; private set; }

    [Required]
    [MaxLength(10)]
    public string Telefono { get; private set; }

    //[Required]
    [MaxLength(100)]
    public string? Nombre { get; private set; }

    //[Required]
    [MaxLength(100)]
    public string? PrimerApellido { get; private set; }

    //[Required]
    [MaxLength(100)]
    public string? SegundoApellido { get; private set; }
    
    public DateOnly? FechaNacimiento { get; private set; }
    
    public Genero? Genero { get; private set; }

    //[Required]
    [EmailAddress]
    public string? CorreoElectronico { get; private set; }

    //[Required]
    [MaxLength(100)]
    public string? Contrasena { get; private set; }

    //[Required]
    public TipoPersona? TipoPersona { get; private set; }
    
    //[Required]
    [MaxLength(18)]
    public string? Curp { get; private set; }
    
    //[Required]
    [MaxLength(13)]
    public string? Rfc { get; private set; }
    
    // TODO EMD: FOTO OPCIONAL, AQUI SE GUARDARA EL GUID O TOKEN QUE RETORNE AWS
    //[Required]
    [MaxLength(500)]
    public string? FotoAWS { get; private set; }
    
    public int EstadoId { get; private set; }
    
    public Estado Estado { get; private set; }
    
    public int EmpresaId { get; private set; }
    
    public Empresa Empresa { get; private set; }
    
    public int DireccionId { get; private set; }
    
    public Direccion Direccion { get; private set; }
    
    public List<Verificacion2FA> Verificaciones2FA { get; private set; }    
    public List<UbicacionesGeolocalizacion> UbicacionesGeolocalizacion { get; private set; }
    public List<DispositivoMovilAutorizado> DispositivoMovilAutorizados { get; private set; }   
    public List<DocumentacionAdjunta> DocumentacionAdjuntas { get; private set; }
    public List<ActividadEconomica> ActividadEconomicas { get; private set; }
    public List<ValidacionCheckton> ValidacionesChecktons { get; private set; } 
    

    



}