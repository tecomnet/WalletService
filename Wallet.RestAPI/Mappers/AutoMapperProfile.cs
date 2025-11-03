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
                               : (DateTime?)null));
        CreateMap<UbicacionesGeolocalizacion, UbicacionResult>();
        CreateMap<DispositivoMovilAutorizado, DispositivoMovilAutorizadoResult>();
        CreateMap<Direccion, DireccionResult>();
        CreateMap<Empresa, EmpresaResult>();
        CreateMap<Estado, EstadoResult>();
    }
}