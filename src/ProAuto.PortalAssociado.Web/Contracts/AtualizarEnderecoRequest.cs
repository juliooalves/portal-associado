using System.ComponentModel.DataAnnotations;
using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Contracts;

public sealed class AtualizarEnderecoRequest
{
    [Required(ErrorMessage = "Informe o logradouro.")]
    [MaxLength(200)]
    [Display(Name = "Logradouro")]
    public string Logradouro { get; init; } = string.Empty;

    [Required(ErrorMessage = "Informe o número.")]
    [MaxLength(20)]
    [Display(Name = "Número")]
    public string Numero { get; init; } = string.Empty;

    [MaxLength(100)]
    [Display(Name = "Complemento")]
    public string? Complemento { get; init; }

    [Required(ErrorMessage = "Informe o bairro.")]
    [MaxLength(100)]
    [Display(Name = "Bairro")]
    public string Bairro { get; init; } = string.Empty;

    [Required(ErrorMessage = "Informe a cidade.")]
    [MaxLength(100)]
    [Display(Name = "Cidade")]
    public string Cidade { get; init; } = string.Empty;

    [Required(ErrorMessage = "Informe a UF.")]
    [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "UF deve conter 2 letras.")]
    [Display(Name = "UF")]
    public string Uf { get; init; } = string.Empty;

    [Required(ErrorMessage = "Informe o CEP.")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve conter 8 dígitos.")]
    [Display(Name = "CEP")]
    public string Cep { get; init; } = string.Empty;

    public Endereco ToDomain() =>
        new(Logradouro, Numero, Complemento, Bairro, Cidade, Uf, Cep);

    public static AtualizarEnderecoRequest From(Endereco endereco) =>
        new()
        {
            Logradouro = endereco.Logradouro,
            Numero = endereco.Numero,
            Complemento = endereco.Complemento,
            Bairro = endereco.Bairro,
            Cidade = endereco.Cidade,
            Uf = endereco.Uf,
            Cep = endereco.Cep
        };
}
