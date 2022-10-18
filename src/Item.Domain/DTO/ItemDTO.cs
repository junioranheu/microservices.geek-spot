using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Usuarios.Domain.DTO;
using Utils.DTO;
using static Utils.Biblioteca;

namespace Itens.Domain.DTO
{
    public class ItemDTO : _RetornoApiDTO
    {
        [Key]
        public int ItemId { get; set; }

        public string? Nome { get; set; } = null;
        public string? Descricao { get; set; } = null;
        public string? Tamanho { get; set; } = null;
        public string? Marca { get; set; } = null;
        public string? Condicao { get; set; } = null;

        // Fk (De cá pra lá);
        [JsonIgnore]
        public List<ItemImagemDTO>? ItensImagens { get; set; }

        public double? Preco { get; set; } = 0;
        public double? PrecoDesconto { get; set; } = 0;

        // Fk (De lá pra cá);
        public int UsuarioId { get; set; }
        public UsuarioDTO? Usuarios { get; set; }

        // Fk (De lá pra cá);
        public int ItemTipoId { get; set; }
        public ItemTipoDTO? ItensTipos { get; set; }

        public bool IsAtivo { get; set; } = true;
        public DateTime? DataRegistro { get; set; } = HorarioBrasilia();
    }
}
