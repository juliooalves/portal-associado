using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Services;

public interface IAssociadoService
{
    Task<Associado?> AutenticarAsync(string? cpf, string? placa, CancellationToken cancellationToken = default);
    Task<Associado?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Associado?> AtualizarEnderecoAsync(Guid associadoId, Endereco novoEndereco, CancellationToken cancellationToken = default);
}
