using System.ComponentModel.DataAnnotations;

namespace ProAuto.PortalAssociado.Web.Contracts;

public sealed class LoginRequest
{
    [Required]
    public string Cpf { get; init; } = string.Empty;

    [Required]
    public string Placa { get; init; } = string.Empty;
}
