using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Pages.Colaboradores
{
    public class CreateModel : PageModel
    {
        private readonly IColaboradorService _colaboradorService;
        private readonly IDepartamentoService _departamentoService;

        public CreateModel(
            IColaboradorService colaboradorService,
            IDepartamentoService departamentoService)
        {
            _colaboradorService = colaboradorService;
            _departamentoService = departamentoService;
        }

        [BindProperty]
        public Colaborador Colaborador { get; set; } = new();

        public SelectList Departamentos { get; set; } = null!;

        // carrega combo ao abrir a tela
        public async Task OnGetAsync()
        {
            await CarregarDepartamentos();
        }

        // carrega combo se der erro
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CarregarDepartamentos(); 
                return Page();
            }

            try
            {
                Colaborador.CPF = Regex.Replace(Colaborador.CPF, @"\D", "");

                if (!string.IsNullOrWhiteSpace(Colaborador.RG))
                    Colaborador.RG = Regex.Replace(Colaborador.RG, @"\D", "");
                else
                    Colaborador.RG = null;

                await _colaboradorService.CriarAsync(Colaborador);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await CarregarDepartamentos(); 
                return Page();
            }
        }

        // MÉTODO REUTILIZÁVEL
        private async Task CarregarDepartamentos()
        {
            var departamentos = await _departamentoService.ListarAsync(
                null, null, null);

            Departamentos = new SelectList(
                departamentos,
                "Id",
                "Nome");
        }
    }
}
