using Usuarios.Domain.DTO;

namespace Usuarios.Application.Common.Interfaces.Persistence
{
    public interface IRefreshTokenRepository
    {
        Task? Adicionar(RefreshTokenDTO dto);
        Task<string>? GetRefreshTokenByUsuarioId(int usuarioId);
    }
}
