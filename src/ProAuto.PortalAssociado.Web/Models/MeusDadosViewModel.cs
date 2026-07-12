using ProAuto.PortalAssociado.Web.Contracts;
using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Models;

public sealed class MeusDadosViewModel
{
    public required AssociadoResponse Dados { get; init; }
    public required AtualizarEnderecoRequest Endereco { get; init; }

    public static MeusDadosViewModel From(Associado associado) =>
        new()
        {
            Dados = AssociadoResponse.From(associado),
            Endereco = AtualizarEnderecoRequest.From(associado.Endereco)
        };
}
