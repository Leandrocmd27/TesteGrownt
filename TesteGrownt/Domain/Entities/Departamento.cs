using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TesteGrownt.Domain.Entities
{
    public class Departamento
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O gerente é obrigatório")]
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
