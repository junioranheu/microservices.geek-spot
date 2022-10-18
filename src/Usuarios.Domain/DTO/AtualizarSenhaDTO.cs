using Utils.DTO;

namespace Usuarios.Domain.DTO
{
    public class AtualizarSenhaDTO : _RetornoApiDTO
    {
        public string? SenhaAtual { get; set; } = null;
        public string? SenhaNova { get; set; } = null;
        public string? SenhaNovaConfirmacao { get; set; } = null;
        public string? Hash { get; set; } = null;
    }
}
