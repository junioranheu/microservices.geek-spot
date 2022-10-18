using Itens.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Usuarios.Domain.Entities;

namespace Itens.Infrastructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
       
        }

        // Usuarios;
        public DbSet<Usuario> Usuarios { get; set; }

        // Itens;
        public DbSet<ItemTipo> ItensTipos { get; set; }
        public DbSet<Item> Itens { get; set; }
        public DbSet<ItemImagem> ItensImagens { get; set; }

        // Comentários;
        public DbSet<Comentario> Comentarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
