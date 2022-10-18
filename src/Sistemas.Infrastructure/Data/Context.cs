using Microsoft.EntityFrameworkCore;
using Sistemas.Domain.Entities;

namespace Sistemas.Infrastructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
       
        }

        // Outros;
        public DbSet<AjudaTopico> AjudasTopicos { get; set; }
        public DbSet<AjudaItem> AjudasItens { get; set; }

        // Cidades e estados;
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
