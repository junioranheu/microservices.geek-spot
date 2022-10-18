using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Utils.DTO;
using static Utils.Biblioteca;

namespace Usuarios.Domain.DTO
{
    public class UsuarioSenhaDTO : _RetornoApiDTO
    {
        [Key]
        public int UsuarioId { get; set; }
        public string? NomeCompleto { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? NomeUsuarioSistema { get; set; } = null;
        public string? Senha { get; set; } = null;
        public string? Token { get; set; } = null;
        public string? RefreshToken { get; set; } = null;
        public int UsuarioTipoId { get; set; }
        public string? Foto { get; set; } = null;
        public DateTime DataRegistro { get; set; } = HorarioBrasilia();
        public DateTime DataOnline { get; set; }
        public bool IsAtivo { get; set; } = true;
        public bool IsPremium { get; set; } = false;
        public bool IsVerificado { get; set; } = false;
        public string? CodigoVerificacao { get; set; } = null;
        public DateTime ValidadeCodigoVerificacao { get; set; }
        public string? HashUrlTrocarSenha { get; set; } = null;
        public DateTime ValidadeHashUrlTrocarSenha { get; set; }

        // Fk (De cá pra lá);
        [JsonIgnore]
        public UsuarioInformacaoDTO? UsuariosInformacoes { get; set; }
    }
}
