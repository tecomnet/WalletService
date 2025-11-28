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
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Usuario.Empresa.Nombre))
            .ForMember(destinationMember: dest => dest.CodigoPais,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Usuario.CodigoPais))
            .ForMember(destinationMember: dest => dest.Telefono,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Usuario.Telefono))
            .ForMember(destinationMember: dest => dest.CorreoElectronico,
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Usuario.CorreoElectronico));
        CreateMap<UbicacionesGeolocalizacion, UbicacionResult>();
        CreateMap<DispositivoMovilAutorizado, DispositivoMovilAutorizadoResult>();
        CreateMap<Direccion, DireccionResult>();
        CreateMap<Empresa, EmpresaResult>();
        CreateMap<Estado, EstadoResult>();
        CreateMap<ProveedorServicio, ProveedorServicioResult>()
            .ForMember(destinationMember: dest => dest.Categoria, memberOptions: opt => opt.MapFrom(mapExpression: src => src.Categoria.ToString()));
        CreateMap<ServicioFavorito, ServicioFavoritoResult>();
        CreateMap<ProductoProveedor, ProductoProveedorResult>();
        CreateMap<Usuario, UsuarioResult>();
        CreateMap<Wallet.Funcionalidad.Models.AuthResultDto, AuthResult>();
    }
}