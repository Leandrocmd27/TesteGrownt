using Microsoft.EntityFrameworkCore;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Domain.Entities;
using TesteGrownt.Infrastructure.Data;

namespace TesteGrownt.Application.Services
{
    public class ColaboradorService : IColaboradorService
    {
        private readonly AppDbContext _context;

        public ColaboradorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Colaborador> CriarAsync(Colaborador colaborador)
        {
            // CPF único
            var cpfExiste = await _context.Colaboradores
                .AnyAsync(x => x.CPF == colaborador.CPF);

            if (cpfExiste)
                throw new InvalidOperationException("CPF já cadastrado.");

            // RG único (se informado)
            if (!string.IsNullOrWhiteSpace(colaborador.RG))
            {
                var rgExiste = await _context.Colaboradores
                    .AnyAsync(x => x.RG == colaborador.RG);

                if (rgExiste)
                    throw new InvalidOperationException("RG já cadastrado.");
            }

            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();

            return colaborador;
        }

        public async Task<IEnumerable<Colaborador>> ListarAsync(
            string? nome,
            string? cpf,
            Guid? departamentoId,
            string? rg)
        {
            var query = _context.Colaboradores
                .Include(x => x.Departamento)
                .ThenInclude(d => d.Gerente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(x => x.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cpf))
                query = query.Where(x => x.CPF == cpf);

            if (!string.IsNullOrWhiteSpace(rg))
                query = query.Where(x => x.RG == rg);

            if (departamentoId.HasValue)
                query = query.Where(x => x.DepartamentoId == departamentoId);

            return await query.ToListAsync();
        }

        // 🔥 DESAFIO: Buscar colaborador com nome do gerente
        public async Task<Colaborador?> ObterComGerenteAsync(Guid id)
        {
            return await _context.Colaboradores
                .Include(x => x.Departamento)
                .ThenInclude(d => d.Gerente)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
