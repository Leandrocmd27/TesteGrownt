using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Pages.Departamentos
{
    public class IndexModel : PageModel
    {
        private readonly IDepartamentoService _service = null!;


        public IndexModel(IDepartamentoService service)
        {
            _service = service;
        }

        public IEnumerable<Departamento> Departamentos { get; set; }
            = Enumerable.Empty<Departamento>();

        // Filtros
        public string? Nome { get; set; }
        public Guid? GerenteId { get; set; }
        public Guid? DepartamentoSuperiorId { get; set; }

        public async Task OnGetAsync(
            string? nome,
            Guid? gerenteId,
            Guid? departamentoSuperiorId)
        {
            Nome = nome;
            GerenteId = gerenteId;
            DepartamentoSuperiorId = departamentoSuperiorId;

            Departamentos = await _service.ListarAsync(
                nome,
                gerenteId,
                departamentoSuperiorId);
        }
    }
}
