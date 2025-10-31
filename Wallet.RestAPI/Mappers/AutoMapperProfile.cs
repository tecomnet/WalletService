using AutoMapper;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Mappers;

/// <summary>
/// 
/// </summary>
public class AutoMapperProfile : Profile
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AutoMapperProfile()
    {
        // Define el mapeo de Origen (Usuario) a Destino (UsuarioDto)
        CreateMap<Cliente, ClienteResult>();
        CreateMap<UbicacionesGeolocalizacion, UbicacionResult>();
        CreateMap<DispositivoMovilAutorizado, DispositivoMovilAutorizadoResult>();
        CreateMap<Direccion, DireccionResult>();
    }
}