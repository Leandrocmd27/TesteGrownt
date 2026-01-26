using Microsoft.EntityFrameworkCore;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Colaborador> Colaboradores => Set<Colaborador>();
        public DbSet<Departamento> Departamentos => Set<Departamento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
