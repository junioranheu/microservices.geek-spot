using System.ComponentModel.DataAnnotations;
using Utils.DTO;

namespace Sistemas.Domain.DTO
{
    public class CidadeDTO: _RetornoApiDTO
    {
        [Key]
        public int CidadeId { get; set; }
        public string? Nome { get; set; } = null;

        // Fk (De lá pra cá);
        public int EstadoId { get; set; }
        public EstadoDTO? Estados { get; set; }

        public bool IsAtivo { get; set; } = true;
    }
}
