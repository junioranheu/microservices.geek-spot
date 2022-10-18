using System.ComponentModel.DataAnnotations;
using Usuarios.Domain.Entities;
using static Utils.Biblioteca;

namespace Itens.Domain.Entities
{
    public class Comentario
    {
        [Key]
        public int ComentarioId { get; set; }

        // Fk (De lá pra cá);
        public int ItemId { get; set; }
        public Item? Itens { get; set; }

        // Fk (De lá pra cá);
        public int UsuarioId { get; set; } // Usuário que fez a pergunta;
        public Usuario? Usuarios { get; set; }

        public string? Mensagem { get; set; } = null;
        public DateTime? DataMensagem { get; set; } = HorarioBrasilia();

        public string? Resposta { get; set; } = null;
        public DateTime? DataResposta { get; set; } = null;

        public bool IsAtivo { get; set; } = true;
    }
}
