using System.ComponentModel.DataAnnotations;
using static Utils.Biblioteca;

namespace Sistemas.Domain.DTO
{
    public class AjudaItemDTO
    {
        [Key]
        public int AjudaItemId { get; set; }

        // Fk (De lá pra cá);
        public int AjudaTopicoId { get; set; }
        public AjudaTopicoDTO? AjudasTopicos { get; set; }

        public string? Titulo { get; set; } = null;
        public string? ConteudoHtml { get; set; } = null;
        public bool IsAtivo { get; set; } = true;
        public DateTime? DataRegistro { get; set; } = HorarioBrasilia();
    }
}
