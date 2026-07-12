using ProAuto.PortalAssociado.Web.Domain;
using ProAuto.PortalAssociado.Web.Repositories;

namespace ProAuto.PortalAssociado.Web.Services;

public sealed class AssociadoService : IAssociadoService
{
    private readonly IAssociadoRepository _repository;

    public AssociadoService(IAssociadoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Associado?> AutenticarAsync(string? cpf, string? placa, CancellationToken cancellationToken = default)
    {
        if (!Cpf.TryParse(cpf, out var cpfValido) || !Placa.TryParse(placa, out var placaValida))
        {
            return null;
        }

        var associado = await _repository.GetByCpfAsync(cpfValido!, cancellationToken);

        return associado is not null && associado.Placa == placaValida
            ? associado
            : null;
    }

    public Task<Associado?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public async Task<Associado?> AtualizarEnderecoAsync(Guid associadoId, Endereco novoEndereco, CancellationToken cancellationToken = default)
    {
        var associado = await _repository.GetByIdAsync(associadoId, cancellationToken);
        if (associado is null)
        {
            return null;
        }

        associado.AtualizarEndereco(novoEndereco);
        await _repository.SaveChangesAsync(cancellationToken);

        return associado;
    }
}
