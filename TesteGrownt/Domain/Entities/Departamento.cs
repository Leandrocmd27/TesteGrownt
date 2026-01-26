using System;
using System.Collections.Generic;

namespace TesteGrownt.Domain.Entities
{
    public class Departamento
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = null!;

        public Guid GerenteId { get; set; }
        public Colaborador Gerente { get; set; } = null!;

        public Guid? DepartamentoSuperiorId { get; set; }
        public Departamento? DepartamentoSuperior { get; set; }

        public ICollection<Departamento> SubDepartamentos { get; set; }
            = new List<Departamento>();

        public ICollection<Colaborador> Colaboradores { get; set; }
            = new List<Colaborador>();
    }
}
