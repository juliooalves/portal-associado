using ProAuto.PortalAssociado.Web.Contracts;
using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Models;

public sealed class MeusDadosViewModel
{
    public required AssociadoResponse Dados { get; init; }
    public required AtualizarEnderecoRequest Endereco { get; init; }

    public string CpfMascarado
    {
        get
        {
            var digits = new string(Dados.Cpf.Where(char.IsDigit).ToArray());
            return $"***.{digits[3..6]}.{digits[6..9]}-**";
        }
    }

    public static MeusDadosViewModel From(Associado associado) =>
        new()
        {
            Dados = AssociadoResponse.From(associado),
            Endereco = AtualizarEnderecoRequest.From(associado.Endereco)
        };
}
