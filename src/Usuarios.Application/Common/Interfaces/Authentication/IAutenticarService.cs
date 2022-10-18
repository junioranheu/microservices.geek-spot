using Usuarios.Domain.DTO;

namespace Usuarios.Application.Common.Interfaces.Authentication
{
    public interface IAutenticarService
    {
        Task<UsuarioDTO>? Registrar(UsuarioSenhaDTO dto);
        Task<UsuarioDTO>? Login(UsuarioSenhaDTO dto);
        Task<UsuarioDTO> RefreshToken(string token, string refreshToken);
    }
}
