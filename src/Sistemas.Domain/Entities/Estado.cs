using System.ComponentModel.DataAnnotations;

namespace Sistemas.Domain.Entities
{
    public class Estado
    {
        [Key]
        public int EstadoId { get; set; }
        public string? Nome { get; set; } = null;
        public string? Sigla { get; set; } = null;
        public bool IsAtivo { get; set; } = true;
    }
}
