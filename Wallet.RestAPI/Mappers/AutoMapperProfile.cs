using System;
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
        //src.FechaNacimiento es DateOnly? (anulable) y dest.FechaNacimiento es DateTime? (anulable)
        CreateMap<Wallet.DOM.Modelos.Cliente, ClienteResult>()
            .ForMember(destinationMember: dest => dest.FechaNacimiento,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.FechaNacimiento.HasValue
                    ? src.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
            .ForMember(destinationMember: dest => dest.Estado,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Estado.Nombre))
            .ForMember(destinationMember: dest => dest.Empresa,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Empresa.Nombre))
            .ForMember(destinationMember: dest => dest.CodigoPais,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Usuario.CodigoPais))
            .ForMember(destinationMember: dest => dest.Telefono,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Usuario.Telefono))
            .ForMember(destinationMember: dest => dest.CorreoElectronico,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Usuario.CorreoElectronico));
        CreateMap<UbicacionesGeolocalizacion, UbicacionResult>();
        CreateMap<DispositivoMovilAutorizado, DispositivoMovilAutorizadoResult>();
        CreateMap<Direccion, DireccionResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Empresa, EmpresaResult>();
        CreateMap<Estado, EstadoResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<ServicioFavorito, ServicioFavoritoResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Proveedor, ProveedorResult>();
        CreateMap<Producto, ProductoResult>();
        CreateMap<Usuario, UsuarioResult>();
        CreateMap<Wallet.Funcionalidad.Models.AuthResultDto, AuthResult>();
        CreateMap<Wallet.DOM.Modelos.Broker, BrokerResult>();
        CreateMap<ConsentimientosUsuario, ConsentimientoUsuarioResult>();
        CreateMap<KeyValueConfig, KeyValueConfigResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
    }
}