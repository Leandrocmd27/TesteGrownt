using System;
using System.ComponentModel.DataAnnotations;

namespace TesteGrownt.Domain.Entities
{
    public class Colaborador
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        public string CPF { get; set; } = null!;

        public string? RG { get; set; }

        public Guid? DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }
    }
}
