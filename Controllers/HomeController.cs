using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FilaU.Models;

namespace FilaU.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult IniciarSesion()
    {
        return View();
    }
    public IActionResult PanelEstudiante()
    {
        return View();
    }
}
