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
            .ForMember(dest => dest.FechaNacimiento,
                opt => opt.MapFrom(src => src.FechaNacimiento.HasValue
                    ? src.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
            .ForMember(dest => dest.Estado,
                opt => opt.MapFrom(src => src.Estado.Nombre))
            .ForMember(dest => dest.Empresa,
                opt => opt.MapFrom(src => src.Usuario.Empresa.Nombre))
            .ForMember(dest => dest.CodigoPais,
                opt => opt.MapFrom(src => src.Usuario.CodigoPais))
            .ForMember(dest => dest.Telefono,
                opt => opt.MapFrom(src => src.Usuario.Telefono))
            .ForMember(dest => dest.CorreoElectronico,
                opt => opt.MapFrom(src => src.Usuario.CorreoElectronico));
        CreateMap<UbicacionesGeolocalizacion, UbicacionResult>();
        CreateMap<DispositivoMovilAutorizado, DispositivoMovilAutorizadoResult>();
        CreateMap<Direccion, DireccionResult>();
        CreateMap<Empresa, EmpresaResult>();
        CreateMap<Estado, EstadoResult>();
        CreateMap<ProveedorServicio, ProveedorServicioResult>()
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria.ToString()));
        CreateMap<ServicioFavorito, ServicioFavoritoResult>();
        CreateMap<ProductoProveedor, ProductoProveedorResult>();
    }
}