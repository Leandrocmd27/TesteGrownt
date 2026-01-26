using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Infrastructure.Mappings
{
    public class DepartamentoMapping : IEntityTypeConfiguration<Departamento>
    {
        public void Configure(EntityTypeBuilder<Departamento> builder)
        {
            builder.ToTable("Departamentos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasOne(x => x.Gerente)
                .WithMany()
                .HasForeignKey(x => x.GerenteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DepartamentoSuperior)
                .WithMany(x => x.SubDepartamentos)
                .HasForeignKey(x => x.DepartamentoSuperiorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
