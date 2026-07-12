using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Repositories;

public interface IAssociadoRepository
{
    Task<Associado?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Associado?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
