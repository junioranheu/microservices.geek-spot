using System.ComponentModel.DataAnnotations;
using static Utils.Biblioteca;

namespace Usuarios.Domain.Entities
{
    public class UsuarioInformacao
    {
        [Key]
        public int UsuarioInformacaoId { get; set; }

        // Fk (De lá pra cá);
        public int UsuarioId { get; set; }

        public DateTime DataAniversario { get; set; }
        public string? CPF { get; set; } = null;
        public string? Telefone { get; set; } = null;
        public string? CEP { get; set; } = null;
        public string? Estado { get; set; } = null;
        public string? Cidade { get; set; } = null;
        public string? Bairro { get; set; } = null;
        public string? Rua { get; set; } = null;
        public string? NumeroResidencia { get; set; } = null;
        public string? ReferenciaLocal { get; set; } = null;

        // Propriedades referentes à "Lojinha";
        public string? LojinhaTitulo { get; set; } = null;
        public string? LojinhaDescricao { get; set; } = null;
        public string? LojinhaImagemCapa { get; set; } = null;
        public double? LojinhaQtdEstrelas { get; set; } = 0;

        public DateTime? DataUltimaAlteracao { get; set; } = HorarioBrasilia();
    }
}
