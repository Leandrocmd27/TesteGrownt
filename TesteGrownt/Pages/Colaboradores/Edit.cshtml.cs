using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Pages.Colaboradores
{
    public class EditModel : PageModel
    {
        private readonly IColaboradorService _colaboradorService;
        private readonly IDepartamentoService _departamentoService;

        public EditModel(
            IColaboradorService colaboradorService,
            IDepartamentoService departamentoService)
        {
            _colaboradorService = colaboradorService;
            _departamentoService = departamentoService;
        }

        [BindProperty]
        public Colaborador Colaborador { get; set; } = null!;

        public SelectList Departamentos { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Colaborador = await _colaboradorService.ObterComGerenteAsync(id);

            if (Colaborador == null)
                return NotFound();

            await CarregarDepartamentos(); 
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove validação de navegação
            ModelState.Remove("Colaborador.Departamento");

            if (!ModelState.IsValid)
            {            
                await CarregarDepartamentos(); 
                return Page();
            }

            try
            {
                Colaborador.CPF = Regex.Replace(Colaborador.CPF, @"\D", "");
                Colaborador.RG = Regex.Replace(Colaborador.RG, @"\D", "");

                await _colaboradorService.AtualizarAsync(Colaborador);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CarregarDepartamentos(); 
                return Page();
            }
        }

        private async Task CarregarDepartamentos()
        {
            var departamentos = await _departamentoService.ListarAsync(null, null, null);
            Departamentos = new SelectList(departamentos, "Id", "Nome");
        }
    }
}