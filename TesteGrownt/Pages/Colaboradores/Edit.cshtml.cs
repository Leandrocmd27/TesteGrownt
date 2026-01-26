using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TesteGrownt.Infrastructure.Data;
using TesteGrownt.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TesteGrownt.Pages.Colaboradores
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Colaborador Colaborador { get; set; } = null!;

        public SelectList Departamentos { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Colaborador = await _context.Colaboradores
                .Include(c => c.Departamento)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Colaborador == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Colaborador).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
