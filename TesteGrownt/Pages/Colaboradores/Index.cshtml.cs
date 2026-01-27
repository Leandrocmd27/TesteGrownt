using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Application.Services;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Pages.Colaboradores
{
    public class IndexModel : PageModel
    {
        private readonly IColaboradorService _colaboradorService;
        private readonly IDepartamentoService _departamentoService;

        public IndexModel(IColaboradorService colaboradorService, IDepartamentoService departamentoService)
        {
            _colaboradorService = colaboradorService;
            _departamentoService = departamentoService;
        }

        public IEnumerable<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
        public SelectList Departamentos { get; set; } = null!;

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

            var departamentos = await _departamentoService.ListarAsync(null, null, null);
            Departamentos = new SelectList(departamentos, "Id", "Nome");
        }
    }
}
