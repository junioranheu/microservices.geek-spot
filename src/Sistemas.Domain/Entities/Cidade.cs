using System.ComponentModel.DataAnnotations;

namespace Sistemas.Domain.Entities
{
    public class Cidade
    {
        [Key]
        public int CidadeId { get; set; }
        public string? Nome { get; set; } = null;

        // Fk (De lá pra cá);
        public int EstadoId { get; set; }
        public Estado? Estados { get; set; }

        public bool IsAtivo { get; set; } = true;
    }
}
