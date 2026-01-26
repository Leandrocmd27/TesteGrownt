using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Application.Interfaces
{
    public interface IDepartamentoService
    {
        Task<Departamento> CriarAsync(Departamento departamento);
        Task<Departamento> AtualizarAsync(Departamento departamento);
        Task<Departamento?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Departamento>> ListarAsync(
            string? nome,
            Guid? gerenteId,
            Guid? departamentoSuperiorId);
        Task<Departamento?> ObterArvoreAsync(Guid id);
        Task<IEnumerable<Colaborador>> ObterColaboradoresDoGerenteAsync(Guid gerenteId);
    }
}