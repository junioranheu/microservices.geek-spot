using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Utils.DTO;

namespace Usuarios.Domain.DTO
{
    public class UsuarioSeguirDTO: _RetornoApiDTO
    {
        [Key]
        public int Id { get; set; }

        /////////////////////////////////////////////////////////
        // Como fazer duas FKs da mesma classe se relacionarem //
        // https://stackoverflow.com/a/57470931                //
        /////////////////////////////////////////////////////////

        // Fk (De lá pra cá);
        [ForeignKey(nameof(UsuariosSeguidos))]
        public int UsuarioSeguidoId { get; set; } // Usuário que é seguido;

        [JsonIgnore]
        public UsuarioDTO? UsuariosSeguidos { get; set; }

        // Fk (De lá pra cá);
        [ForeignKey(nameof(UsuariosSeguidores))]
        public int UsuarioSeguidorId { get; set; } // Usuário que segue o "UsuarioSeguidoId"; 

        [JsonIgnore]
        public UsuarioDTO? UsuariosSeguidores { get; set; }

        public bool IsAtivo { get; set; } = true;
        public DateTime? DataRegistro { get; set; }
    }
}
