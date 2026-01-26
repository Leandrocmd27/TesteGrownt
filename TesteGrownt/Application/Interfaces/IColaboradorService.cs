using TesteGrownt.Domain.Entities;

namespace TesteGrownt.Application.Interfaces
{
    public interface IColaboradorService
    {
        Task<Colaborador> CriarAsync(Colaborador colaborador);
        Task<IEnumerable<Colaborador>> ListarAsync(
            string? nome,
            string? cpf,
            Guid? departamentoId,
            string? rg);

        Task<Colaborador?> ObterComGerenteAsync(Guid id);
    }
}
