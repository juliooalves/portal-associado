using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Contracts;

public sealed record AssociadoResponse(
    string Nome,
    string Cpf,
    string Placa,
    string Telefone,
    EnderecoResponse Endereco)
{
    public static AssociadoResponse From(Associado associado) =>
        new(
            associado.Nome,
            associado.Cpf.Formatted,
            associado.Placa.Value,
            associado.Telefone,
            EnderecoResponse.From(associado.Endereco));
}

public sealed record EnderecoResponse(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Uf,
    string Cep)
{
    public static EnderecoResponse From(Endereco endereco) =>
        new(
            endereco.Logradouro,
            endereco.Numero,
            endereco.Complemento,
            endereco.Bairro,
            endereco.Cidade,
            endereco.Uf,
            endereco.Cep);
}
