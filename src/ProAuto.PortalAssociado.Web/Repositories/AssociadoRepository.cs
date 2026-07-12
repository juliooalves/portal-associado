using Microsoft.EntityFrameworkCore;
using ProAuto.PortalAssociado.Web.Data;
using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Repositories;

public sealed class AssociadoRepository : IAssociadoRepository
{
    private readonly AppDbContext _dbContext;

    public AssociadoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Associado?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Associados.SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Associado?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default) =>
        _dbContext.Associados.SingleOrDefaultAsync(a => a.Cpf == cpf, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
