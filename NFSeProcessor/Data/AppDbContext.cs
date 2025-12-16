using Microsoft.EntityFrameworkCore;
using NFSeProcessor.Models;

namespace NFSeProcessor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<NotaFiscal> NotaFiscal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotaFiscal>(entity =>
            {
                entity.HasIndex(e => e.Numero).IsUnique();
                entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            });
        }
    }
}