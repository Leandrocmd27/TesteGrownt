using Microsoft.EntityFrameworkCore;
using TesteGrownt.Application.Interfaces;
using TesteGrownt.Domain.Entities;
using TesteGrownt.Infrastructure.Data;

namespace TesteGrownt.Application.Services
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly AppDbContext _context;

        public DepartamentoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Departamento> CriarAsync(Departamento departamento)
        {
            // Valida se o gerente existe
            var gerenteExiste = await _context.Colaboradores
                .AnyAsync(c => c.Id == departamento.GerenteId);

            if (!gerenteExiste)
                throw new InvalidOperationException("O gerente informado não existe.");

            // Valida departamento superior (se informado)
            if (departamento.DepartamentoSuperiorId.HasValue &&
                departamento.DepartamentoSuperiorId.Value != Guid.Empty)
            {
                var superiorExiste = await _context.Departamentos
                    .AnyAsync(d => d.Id == departamento.DepartamentoSuperiorId.Value);

                if (!superiorExiste)
                    throw new InvalidOperationException("O departamento superior informado não existe.");
            }
            else
            {
                // Garante que seja null se não foi informado
                departamento.DepartamentoSuperiorId = null;
            }

            // Gera novo Id
            if (departamento.Id == Guid.Empty)
                departamento.Id = Guid.NewGuid();

            _context.Departamentos.Add(departamento);
            await _context.SaveChangesAsync();
            return departamento;
        }

        public async Task<Departamento> AtualizarAsync(Departamento departamento)
        {
            // Valida se o departamento existe
            var existe = await _context.Departamentos
                .AnyAsync(d => d.Id == departamento.Id);

            if (!existe)
                throw new InvalidOperationException("Departamento não encontrado.");

            // Valida se o gerente existe
            var gerenteExiste = await _context.Colaboradores
                .AnyAsync(c => c.Id == departamento.GerenteId);

            if (!gerenteExiste)
                throw new InvalidOperationException("O gerente informado não existe.");

            // Valida auto-referência
            if (departamento.DepartamentoSuperiorId.HasValue &&
                departamento.DepartamentoSuperiorId.Value == departamento.Id)
            {
                throw new InvalidOperationException("Um departamento não pode ser superior a si mesmo.");
            }

            // Valida departamento superior (se informado)
            if (departamento.DepartamentoSuperiorId.HasValue &&
                departamento.DepartamentoSuperiorId.Value != Guid.Empty)
            {
                var superiorExiste = await _context.Departamentos
                    .AnyAsync(d => d.Id == departamento.DepartamentoSuperiorId.Value);

                if (!superiorExiste)
                    throw new InvalidOperationException("O departamento superior informado não existe.");

                // Valida ciclos na hierarquia
                if (await VerificarCicloHierarquiaAsync(departamento.Id, departamento.DepartamentoSuperiorId.Value))
                {
                    throw new InvalidOperationException("Esta operação criaria um ciclo na hierarquia de departamentos.");
                }
            }
            else
            {
                departamento.DepartamentoSuperiorId = null;
            }

            _context.Entry(departamento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return departamento;
        }

        public async Task<Departamento?> ObterPorIdAsync(Guid id)
        {
            return await _context.Departamentos
                .Include(d => d.Gerente)
                .Include(d => d.DepartamentoSuperior)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Departamento>> ListarAsync(
            string? nome,
            Guid? gerenteId,
            Guid? departamentoSuperiorId)
        {
            var query = _context.Departamentos
                .Include(d => d.Gerente)
                .Include(d => d.DepartamentoSuperior)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(x => x.Nome.Contains(nome));

            if (gerenteId.HasValue)
                query = query.Where(x => x.GerenteId == gerenteId);

            if (departamentoSuperiorId.HasValue)
                query = query.Where(x => x.DepartamentoSuperiorId == departamentoSuperiorId);

            return await query.ToListAsync();
        }

        public async Task<Departamento?> ObterArvoreAsync(Guid id)
        {
            var departamento = await _context.Departamentos
                .Include(d => d.SubDepartamentos)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (departamento == null)
                return null;

            await CarregarSubDepartamentos(departamento);
            return departamento;
        }

        private async Task CarregarSubDepartamentos(Departamento departamento)
        {
            foreach (var sub in departamento.SubDepartamentos)
            {
                await _context.Entry(sub)
                    .Collection(d => d.SubDepartamentos)
                    .LoadAsync();
                await CarregarSubDepartamentos(sub);
            }
        }

        public async Task<IEnumerable<Colaborador>> ObterColaboradoresDoGerenteAsync(Guid gerenteId)
        {
            var departamentos = await _context.Departamentos
                .Where(d => d.GerenteId == gerenteId)
                .Include(d => d.Colaboradores)
                .Include(d => d.SubDepartamentos)
                .ToListAsync();

            var resultado = new List<Colaborador>();
            foreach (var dep in departamentos)
            {
                resultado.AddRange(dep.Colaboradores);
                await ColetarColaboradoresRecursivo(dep, resultado);
            }

            return resultado.Distinct().ToList();
        }

        private async Task ColetarColaboradoresRecursivo(
            Departamento departamento,
            List<Colaborador> resultado)
        {
            foreach (var sub in departamento.SubDepartamentos)
            {
                await _context.Entry(sub)
                    .Collection(d => d.Colaboradores)
                    .LoadAsync();
                resultado.AddRange(sub.Colaboradores);

                await _context.Entry(sub)
                    .Collection(d => d.SubDepartamentos)
                    .LoadAsync();
                await ColetarColaboradoresRecursivo(sub, resultado);
            }
        }

        private async Task<bool> VerificarCicloHierarquiaAsync(Guid departamentoId, Guid novoPaiId)
        {
            var departamentoAtual = novoPaiId;
            var visitados = new HashSet<Guid> { departamentoId };

            while (departamentoAtual != Guid.Empty)
            {
                if (visitados.Contains(departamentoAtual))
                    return true;

                visitados.Add(departamentoAtual);

                var depto = await _context.Departamentos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == departamentoAtual);

                if (depto?.DepartamentoSuperiorId == null)
                    break;

                departamentoAtual = depto.DepartamentoSuperiorId.Value;
            }

            return false;
        }
    }
}