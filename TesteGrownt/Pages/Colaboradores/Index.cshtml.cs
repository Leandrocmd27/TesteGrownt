using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Pages.Colaboradores
{
    public class IndexModel : PageModel
    {
        private readonly IColaboradorService _colaboradorService;

        public IndexModel(IColaboradorService colaboradorService)
        {
            _colaboradorService = colaboradorService;
        }

        public IEnumerable<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();

        [BindProperty(SupportsGet = true)]
        public string? Nome { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CPF { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RG { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? DepartamentoId { get; set; }

        public async Task OnGetAsync()
        {
            Colaboradores = await _colaboradorService.ListarAsync(
                Nome,
                CPF,
                DepartamentoId,
                RG
            );
        }
    }
}
