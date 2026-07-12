using System.ComponentModel.DataAnnotations;
using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Contracts;

public sealed class AtualizarEnderecoRequest
{
    [Required]
    [MaxLength(200)]
    public string Logradouro { get; init; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Numero { get; init; } = string.Empty;

    [MaxLength(100)]
    public string? Complemento { get; init; }

    [Required]
    [MaxLength(100)]
    public string Bairro { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Cidade { get; init; } = string.Empty;

    [Required]
    [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "UF deve conter 2 letras.")]
    public string Uf { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve conter 8 dígitos.")]
    public string Cep { get; init; } = string.Empty;

    public Endereco ToDomain() =>
        new(Logradouro, Numero, Complemento, Bairro, Cidade, Uf, Cep);
}
