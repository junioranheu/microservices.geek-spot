using AutoMapper;
using Usuarios.Domain.DTO;
using Usuarios.Domain.Entities;

namespace Usuarios.Infrastructure.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // Outros; 
            CreateMap<RefreshToken, RefreshTokenDTO>().ReverseMap();

            // Usuário e afins;
            CreateMap<UsuarioTipo, UsuarioTipoDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioSenhaDTO>().ReverseMap();
            CreateMap<UsuarioInformacao, UsuarioInformacaoDTO>().ReverseMap();
            CreateMap<UsuarioSenhaDTO, UsuarioDTO>().ReverseMap();
            CreateMap<UsuarioSeguirDTO, UsuarioSeguir>().ReverseMap();

        }
    }
}
