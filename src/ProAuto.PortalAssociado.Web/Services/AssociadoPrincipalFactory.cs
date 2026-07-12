using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Services;

public static class AssociadoPrincipalFactory
{
    public static ClaimsPrincipal Create(Associado associado) =>
        new(new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, associado.Id.ToString()),
                new Claim(ClaimTypes.Name, associado.Nome)
            },
            CookieAuthenticationDefaults.AuthenticationScheme));
}
