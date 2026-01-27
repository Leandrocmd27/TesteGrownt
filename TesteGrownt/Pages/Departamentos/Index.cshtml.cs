using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Pages.Departamentos
{
    public class IndexModel : PageModel
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IColaboradorService _colaboradorService;

        public IndexModel(
            IDepartamentoService departamentoService,
            IColaboradorService colaboradorService)
        {
            _departamentoService = departamentoService;
            _colaboradorService = colaboradorService;
        }

        public IEnumerable<Departamento> Departamentos { get; set; }
            = Enumerable.Empty<Departamento>();

        // ?? LISTAS PARA OS FILTROS
        public SelectList Gerentes { get; set; } = null!;
        public SelectList DepartamentosSuperiores { get; set; } = null!;

        // ?? PROPRIEDADES DOS FILTROS (com SupportsGet)
        [BindProperty(SupportsGet = true)]
        public string? Nome { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? GerenteId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? DepartamentoSuperiorId { get; set; }

        public async Task OnGetAsync()
        {
            // 1?? Busca departamentos com os filtros aplicados
            Departamentos = await _departamentoService.ListarAsync(
                Nome,
                GerenteId,
                DepartamentoSuperiorId);

            // 2?? Carrega listas para os dropdowns de filtro
            await CarregarFiltrosAsync();
        }

        private async Task CarregarFiltrosAsync()
        {
            // Lista de colaboradores (gerentes)
            var colaboradores = await _colaboradorService.ListarAsync(
                null, null, null, null);
            Gerentes = new SelectList(colaboradores, "Id", "Nome");

            // Lista de departamentos (para filtro de departamento superior)
            var todosDepartamentos = await _departamentoService.ListarAsync(
                null, null, null);
            DepartamentosSuperiores = new SelectList(
                todosDepartamentos, "Id", "Nome");
        }
    }
}