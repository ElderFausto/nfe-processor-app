using Microsoft.EntityFrameworkCore;
using NfeProcessor.Models;

namespace NfeProcessor.Data
{
    public class NfeDbContext : DbContext
    {
        public NfeDbContext(DbContextOptions<NfeDbContext> options) : base(options)
        {
        }

        // Nomes das tabelas no DbSet
        public DbSet<Nfe> Nfes { get; set; }
        public DbSet<NfeProduct> NfeProducts { get; set; }

        // Configuração dos relacionamentos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Nfe>()
                .HasMany(n => n.Products)
                .WithOne(p => p.Nfe)
                .HasForeignKey(p => p.NfeAccessKey);
        }
    }
}