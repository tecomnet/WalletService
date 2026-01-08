using System;
using AutoMapper;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionWallet;
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
        CreateMap<Cliente, ClienteResult>()
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
                memberOptions: opt => opt.MapFrom(mapExpression: src => src.Usuario.CorreoElectronico))
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<UbicacionesGeolocalizacion, UbicacionResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<DispositivoMovilAutorizado, DispositivoMovilAutorizadoResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Direccion, DireccionResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Empresa, EmpresaResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Estado, EstadoResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<ServicioFavorito, ServicioFavoritoResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Proveedor, ProveedorResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Producto, ProductoResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Usuario, UsuarioResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<Wallet.Funcionalidad.Models.AuthResultDto, AuthResult>();
        CreateMap<Broker, BrokerResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<ConsentimientosUsuario, ConsentimientoUsuarioResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<KeyValueConfig, KeyValueConfigResult>()
            .ForMember(destinationMember: dest => dest.ConcurrencyToken,
                memberOptions: opt => opt.MapFrom(mapExpression: src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<CuentaWallet, CuentaWalletResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<BitacoraTransaccion, TransaccionResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<DetallesPagoServicio, DetallesPagoServicioResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<TarjetaEmitida, TarjetaEmitidaResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
        CreateMap<TarjetaVinculada, TarjetaVinculadaResult>()
            .ForMember(dest => dest.ConcurrencyToken,
                opt => opt.MapFrom(src =>
                    src.ConcurrencyToken != null ? Convert.ToBase64String(src.ConcurrencyToken) : null));
    }
}