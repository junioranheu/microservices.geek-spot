using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Utils.DTO;
using static Utils.Biblioteca;

namespace Itens.Domain.DTO
{
    public class ComentarioDTO : _RetornoApiDTO
    {
        [Key]
        public int ComentarioId { get; set; }

        // Fk (De lá pra cá);
        public int ItemId { get; set; }

        [JsonIgnore]
        public ItemDTO? Itens { get; set; }

        // Fk (De lá pra cá);
        public int? UsuarioId { get; set; } // Usuário que fez a pergunta;

        [JsonIgnore]
        public UsuarioDTO? Usuarios { get; set; }

        public string? Mensagem { get; set; } = null;
        public DateTime? DataMensagem { get; set; } = HorarioBrasilia();

        public string? Resposta { get; set; } = null;
        public DateTime? DataResposta { get; set; } = null;

        public bool IsAtivo { get; set; } = true;
    }
}
