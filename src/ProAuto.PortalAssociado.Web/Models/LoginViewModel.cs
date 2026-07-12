using System.ComponentModel.DataAnnotations;

namespace ProAuto.PortalAssociado.Web.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Informe o CPF.")]
    [Display(Name = "CPF")]
    public string Cpf { get; init; } = string.Empty;

    [Required(ErrorMessage = "Informe a placa.")]
    [Display(Name = "Placa")]
    public string Placa { get; init; } = string.Empty;
}
