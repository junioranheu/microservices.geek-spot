using System.ComponentModel.DataAnnotations;
using Utils.DTO;
using static Utils.Biblioteca;

namespace Usuarios.Domain.DTO
{
    public class UsuarioTipoDTO : _RetornoApiDTO
    {
        [Key]
        public int UsuarioTipoId { get; set; } 
        public string? Tipo { get; set; } = null;
        public string? Descricao { get; set; } = null;
        public bool IsAtivo { get; set; } = true;
        public DateTime DataRegistro { get; set; } = HorarioBrasilia();
    }
}
