using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAuto.PortalAssociado.Web.Contracts;
using ProAuto.PortalAssociado.Web.Domain;
using ProAuto.PortalAssociado.Web.Services;

namespace ProAuto.PortalAssociado.Web.Controllers.Api;

[ApiController]
[Route("api/associados")]
[Authorize]
public sealed class AssociadosController : ControllerBase
{
    private readonly IAssociadoService _associadoService;

    public AssociadosController(IAssociadoService associadoService)
    {
        _associadoService = associadoService;
    }

    private Guid AssociadoId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var associado = await _associadoService.ObterPorIdAsync(AssociadoId, cancellationToken);

        return associado is null
            ? NotFound()
            : Ok(AssociadoResponse.From(associado));
    }

    [HttpPut("me/endereco")]
    public async Task<IActionResult> AtualizarEndereco(
        AtualizarEnderecoRequest request,
        CancellationToken cancellationToken)
    {
        Endereco novoEndereco;
        try
        {
            novoEndereco = request.ToDomain();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
            return ValidationProblem(
                statusCode: StatusCodes.Status400BadRequest,
                modelStateDictionary: ModelState);
        }

        var associado = await _associadoService.AtualizarEnderecoAsync(AssociadoId, novoEndereco, cancellationToken);

        return associado is null
            ? NotFound()
            : Ok(AssociadoResponse.From(associado));
    }
}
