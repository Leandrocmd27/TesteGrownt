using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

        // 🔹 PROPRIEDADES SOMENTE PARA EXIBIÇÃO
        public string CPFFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CPF))
                    return "";

                var somenteNumeros = Regex.Replace(CPF, @"\D", "");

                return somenteNumeros.Length == 11
                    ? Convert.ToUInt64(somenteNumeros).ToString(@"000\.000\.000\-00")
                    : CPF;
            }
        }

        public string RGFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RG))
                    return "";

                var somenteNumeros = Regex.Replace(RG, @"\D", "");

                return Convert.ToUInt64(somenteNumeros).ToString(@"00\.000\.000\-0");
            }
        }
    }
}
