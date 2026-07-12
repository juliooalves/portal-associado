using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProAuto.PortalAssociado.Web.Models;

namespace ProAuto.PortalAssociado.Web.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "MeusDados");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
