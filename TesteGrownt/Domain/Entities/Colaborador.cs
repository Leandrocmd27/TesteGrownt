using System;

namespace TesteGrownt.Domain.Entities
{
    public class Colaborador
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = null!;

        public string CPF { get; set; } = null!;

        public string? RG { get; set; }

        public Guid? DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }
    }
}
