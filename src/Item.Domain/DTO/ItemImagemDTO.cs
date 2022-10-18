using System.ComponentModel.DataAnnotations;
using static Utils.Biblioteca;

namespace Itens.Domain.DTO
{
    public class ItemImagemDTO
    {
        [Key]
        public int ItemImagemId { get; set; }

        public string? CaminhoImagem { get; set; } = null;

        // Fk (De lá pra cá);
        public int ItemId { get; set; }

        public bool IsFotoPrincipal { get; set; } = false;
        public bool IsAtivo { get; set; } = true;
        public DateTime? DataRegistro { get; set; } = HorarioBrasilia();
    }
}
