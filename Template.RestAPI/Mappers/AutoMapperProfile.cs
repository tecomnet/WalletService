using AutoMapper;

namespace Template.RestAPI.Mappers;

/// <summary>
/// 
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Define el mapeo de Origen (Usuario) a Destino (UsuarioDto)
        //CreateMap<Usuario, UsuarioDto>();
    }
}