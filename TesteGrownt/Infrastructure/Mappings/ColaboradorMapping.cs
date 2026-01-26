using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Infrastructure.Mappings
{
    public class ColaboradorMapping : IEntityTypeConfiguration<Colaborador>
    {
        public void Configure(EntityTypeBuilder<Colaborador> builder)
        {
            builder.ToTable("Colaboradores");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.CPF)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(x => x.RG)
                .HasMaxLength(20);

            builder.HasIndex(x => x.CPF)
                .IsUnique();

            builder.HasIndex(x => x.RG)
                .IsUnique();

            builder.HasOne(x => x.Departamento)
                .WithMany(d => d.Colaboradores)
                .HasForeignKey(x => x.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
