using System.ComponentModel.DataAnnotations;
using Utils.DTO;
using static Utils.Biblioteca;

namespace Itens.Domain.DTO
{
    public class ItemTipoDTO : _RetornoApiDTO
    {
        [Key]
        public int ItemTipoId { get; set; }
        public string? Tipo { get; set; } = null;
        public string? Descricao { get; set; } = null;
        public bool IsNovoTipo { get; set; } = false;
        public bool IsAtivo { get; set; } = true;
        public DateTime DataRegistro { get; set; } = HorarioBrasilia();
    }
}
