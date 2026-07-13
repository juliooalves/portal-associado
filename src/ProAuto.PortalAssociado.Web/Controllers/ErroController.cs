using Microsoft.AspNetCore.Mvc;
using ProAuto.PortalAssociado.Web.Models;

namespace ProAuto.PortalAssociado.Web.Controllers;

[Route("erro")]
public sealed class ErroController : Controller
{
    [Route("{statusCode:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index(int statusCode)
    {
        if (statusCode is < 400 or > 599)
        {
            statusCode = StatusCodes.Status404NotFound;
        }

        if (statusCode == StatusCodes.Status404NotFound && User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Login", "Conta");
        }

        var (titulo, mensagem) = statusCode switch
        {
            StatusCodes.Status404NotFound =>
                ("Página não encontrada", "O endereço acessado não existe ou foi movido."),
            StatusCodes.Status403Forbidden =>
                ("Acesso negado", "Você não tem permissão para acessar esta página."),
            StatusCodes.Status405MethodNotAllowed =>
                ("Ação não permitida", "Esta operação não é suportada neste endereço."),
            StatusCodes.Status500InternalServerError =>
                ("Erro interno", "Algo deu errado ao processar sua solicitação. Tente novamente em instantes."),
            _ =>
                ("Algo deu errado", "Não foi possível completar sua solicitação."),
        };

        Response.StatusCode = statusCode;
        return View(new ErrorViewModel(statusCode, titulo, mensagem));
    }
}
