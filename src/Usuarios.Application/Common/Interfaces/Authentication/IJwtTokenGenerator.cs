using System.Security.Claims;
using Usuarios.Domain.DTO;

namespace Usuarios.Application.Common.Interfaces.Authentication
{
    public interface IJwtTokenGenerator
    {
        string GerarToken(UsuarioSenhaDTO usuario, IEnumerable<Claim>? listaClaims);
        string GerarRefreshToken();
        ClaimsPrincipal? GetInfoTokenExpirado(string? token);
    }
}
