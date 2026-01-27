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
            if (string.IsNullOrWhiteSpace(colaborador.Nome))
                throw new InvalidOperationException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(colaborador.CPF))
                throw new InvalidOperationException("CPF é obrigatório.");

            var cpfExiste = await _context.Colaboradores
                .AnyAsync(x => x.CPF == colaborador.CPF);

            if (cpfExiste)
                throw new InvalidOperationException("CPF já cadastrado.");

            if (!string.IsNullOrWhiteSpace(colaborador.RG))
            {
                var rgExiste = await _context.Colaboradores
                    .AnyAsync(x => x.RG == colaborador.RG);

                if (rgExiste)
                    throw new InvalidOperationException("RG já cadastrado.");
            }

            // 🔥 REGRA DO DEPARTAMENTO
            var existeDepartamento = await _context.Departamentos.AnyAsync();

            if (existeDepartamento)
            {
                if (colaborador.DepartamentoId == Guid.Empty)
                    throw new InvalidOperationException(
                        "Departamento é obrigatório pois existem departamentos cadastrados.");

                var departamentoExiste = await _context.Departamentos
                    .AnyAsync(d => d.Id == colaborador.DepartamentoId);

                if (!departamentoExiste)
                    throw new InvalidOperationException("Departamento não existe ou não informado .");
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

        // ColaboradorService.cs - ADICIONAR
        public async Task<Colaborador> AtualizarAsync(Colaborador colaborador)
        {
            if (string.IsNullOrWhiteSpace(colaborador.Nome))
                throw new InvalidOperationException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(colaborador.CPF))
                throw new InvalidOperationException("CPF é obrigatório.");

            var existe = await _context.Colaboradores
                .AnyAsync(c => c.Id == colaborador.Id);

            if (!existe)
                throw new InvalidOperationException("Colaborador não encontrado.");

            // Valida CPF duplicado (exceto o próprio)
            var cpfExiste = await _context.Colaboradores
                .AnyAsync(x => x.CPF == colaborador.CPF && x.Id != colaborador.Id);

            if (cpfExiste)
                throw new InvalidOperationException("CPF já cadastrado.");

            // Valida RG duplicado (se informado)
            if (!string.IsNullOrWhiteSpace(colaborador.RG))
            {
                var rgExiste = await _context.Colaboradores
                    .AnyAsync(x => x.RG == colaborador.RG && x.Id != colaborador.Id);

                if (rgExiste)
                    throw new InvalidOperationException("RG já cadastrado.");
            }

            // Valida departamento
            if (colaborador.DepartamentoId != Guid.Empty)
            {
                var departamentoExiste = await _context.Departamentos
                    .AnyAsync(d => d.Id == colaborador.DepartamentoId);

                if (!departamentoExiste)
                    throw new InvalidOperationException("Departamento não existe.");
            }

            _context.Entry(colaborador).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return colaborador;
        }
    }
}
