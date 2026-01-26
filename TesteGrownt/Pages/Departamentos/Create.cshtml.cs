using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Pages.Departamentos
{
    public class CreateModel : PageModel
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IColaboradorService _colaboradorService;

        public CreateModel(
            IDepartamentoService departamentoService,
            IColaboradorService colaboradorService)
        {
            _departamentoService = departamentoService;
            _colaboradorService = colaboradorService;
        }

        [BindProperty]
        public Departamento Departamento { get; set; } = new();

        public List<SelectListItem> Gerentes { get; set; } = new();
        public List<SelectListItem> DepartamentosSuperiores { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CarregarCombosAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove validação de propriedades de navegação
            ModelState.Remove("Departamento.Gerente");
            ModelState.Remove("Departamento.DepartamentoSuperior");
            ModelState.Remove("Departamento.SubDepartamentos");
            ModelState.Remove("Departamento.Colaboradores");

            // Trata Guid.Empty como null para campo opcional
            if (Departamento.DepartamentoSuperiorId == Guid.Empty)
            {
                Departamento.DepartamentoSuperiorId = null;
            }

            // Valida se o gerente foi selecionado
            if (Departamento.GerenteId == Guid.Empty)
            {
                ModelState.AddModelError("Departamento.GerenteId", "O gerente é obrigatório");
            }

            if (!ModelState.IsValid)
            {
                await CarregarCombosAsync();
                return Page();
            }

            try
            {
                await _departamentoService.CriarAsync(Departamento);
                TempData["Sucesso"] = "Departamento criado com sucesso!";
                return RedirectToPage("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CarregarCombosAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao salvar departamento. Tente novamente.");
                await CarregarCombosAsync();
                return Page();
            }
        }

        private async Task CarregarCombosAsync()
        {
            var colaboradores = await _colaboradorService.ListarAsync(null, null, null, null);
            Gerentes = colaboradores
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome
                }).ToList();

            var departamentos = await _departamentoService.ListarAsync(null, null, null);
            DepartamentosSuperiores = departamentos
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nome
                }).ToList();
        }
    }
}