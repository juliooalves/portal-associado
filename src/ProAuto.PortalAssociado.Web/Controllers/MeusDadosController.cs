using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAuto.PortalAssociado.Web.Contracts;
using ProAuto.PortalAssociado.Web.Domain;
using ProAuto.PortalAssociado.Web.Models;
using ProAuto.PortalAssociado.Web.Services;

namespace ProAuto.PortalAssociado.Web.Controllers;

[Authorize]
public sealed class MeusDadosController : Controller
{
    private readonly IAssociadoService _associadoService;

    public MeusDadosController(IAssociadoService associadoService)
    {
        _associadoService = associadoService;
    }

    private Guid AssociadoId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("meus-dados")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var associado = await _associadoService.ObterPorIdAsync(AssociadoId, cancellationToken);

        if (associado is null)
        {
            return await SessaoInvalida();
        }

        return View(MeusDadosViewModel.From(associado));
    }

    [HttpPost("meus-dados/endereco")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarEndereco(
        [Bind(Prefix = "Endereco")] AtualizarEnderecoRequest endereco,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await ReexibirFormulario(endereco, cancellationToken);
        }

        Endereco novoEndereco;
        try
        {
            novoEndereco = endereco.ToDomain();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError($"Endereco.{ex.ParamName}", ex.Message);
            return await ReexibirFormulario(endereco, cancellationToken);
        }

        var associado = await _associadoService.AtualizarEnderecoAsync(AssociadoId, novoEndereco, cancellationToken);

        if (associado is null)
        {
            return await SessaoInvalida();
        }

        TempData["Sucesso"] = "Endereço atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> ReexibirFormulario(
        AtualizarEnderecoRequest endereco,
        CancellationToken cancellationToken)
    {
        var associado = await _associadoService.ObterPorIdAsync(AssociadoId, cancellationToken);

        if (associado is null)
        {
            return await SessaoInvalida();
        }

        var viewModel = new MeusDadosViewModel
        {
            Dados = AssociadoResponse.From(associado),
            Endereco = endereco
        };

        return View(nameof(Index), viewModel);
    }

    private async Task<IActionResult> SessaoInvalida()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Conta");
    }
}
