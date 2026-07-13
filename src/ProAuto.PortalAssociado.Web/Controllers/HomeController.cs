using Microsoft.AspNetCore.Mvc;

namespace ProAuto.PortalAssociado.Web.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "MeusDados");
    }
}
