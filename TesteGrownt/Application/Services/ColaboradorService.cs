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

            if (!CpfValido(colaborador.CPF))
                throw new InvalidOperationException("CPF inválido.");           


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
                if (!RgValido(colaborador.RG))
                    throw new InvalidOperationException("RG inválido.");

                var rgExiste = await _context.Colaboradores
                    .AnyAsync(x => x.RG == colaborador.RG);

                if (rgExiste)
                    throw new InvalidOperationException("RG já cadastrado.");
            }

            //REGRA DO DEPARTAMENTO
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
            {
                var cpfLimpo = LimparNumero(cpf);
                query = query.Where(x => x.CPF == cpfLimpo);
            }

            if (!string.IsNullOrWhiteSpace(rg))
            {
                var rgLimpo = LimparNumero(rg);
                query = query.Where(x => x.RG == rgLimpo);
            }

            if (departamentoId.HasValue)
                query = query.Where(x => x.DepartamentoId == departamentoId);

            return await query.ToListAsync();
        }

        private static string LimparNumero(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            return new string(valor.Where(char.IsDigit).ToArray());
        }


        // Buscar colaborador com nome do gerente
        public async Task<Colaborador?> ObterComGerenteAsync(Guid id)
        {
            return await _context.Colaboradores
                .Include(x => x.Departamento)
                .ThenInclude(d => d.Gerente)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        
        public async Task<Colaborador> AtualizarAsync(Colaborador colaborador)
        {
            if (!CpfValido(colaborador.CPF))
                throw new InvalidOperationException("CPF inválido.");

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
                if (!RgValido(colaborador.RG))
                    throw new InvalidOperationException("RG inválido.");

                var rgExiste = await _context.Colaboradores
                    .AnyAsync(x => x.RG == colaborador.RG && x.Id != colaborador.Id);

                if (rgExiste)
                    throw new InvalidOperationException("RG já cadastrado.");
            }

            // Valida departamento
            if (colaborador.DepartamentoId != Guid.Empty)
            {
                
                var existeDepartamento = await _context.Departamentos.AnyAsync();

                if (existeDepartamento)
                {
                    if (colaborador.DepartamentoId == Guid.Empty)
                        throw new InvalidOperationException(
                            "Departamento é obrigatório pois existem departamentos cadastrados.");

                    var departamentoExiste = await _context.Departamentos
                        .AnyAsync(d => d.Id == colaborador.DepartamentoId);

                    if (!departamentoExiste)
                        throw new InvalidOperationException("Departamento não existe.");
                }
            }

            _context.Entry(colaborador).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return colaborador;
        }

        public static bool CpfValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            // Bloqueia CPFs com todos os números iguais
            if (cpf.Distinct().Count() == 1)
                return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        public static bool RgValido(string rg)
        {
            if (string.IsNullOrWhiteSpace(rg))
                return true; // RG é opcional

            rg = rg.Replace(".", "").Replace("-", "").ToUpper();

            if (rg.Length < 7 || rg.Length > 9)
                return false;

            // Bloqueia RG com todos os caracteres iguais
            if (rg.Distinct().Count() == 1)
                return false;

            // Aceita números e X no final
            for (int i = 0; i < rg.Length; i++)
            {
                if (!char.IsDigit(rg[i]) && !(rg[i] == 'X' && i == rg.Length - 1))
                    return false;
            }

            return true;
        }


    }
}
