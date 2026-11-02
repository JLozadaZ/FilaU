using Microsoft.AspNetCore.Mvc;

namespace FilaU.Controllers
{
    public class PanelController : Controller
    {
        // GET: /Panel/PanelEstudiante
        public IActionResult PanelEstudiante()
        {
            return View();
        }

        // GET: /Panel/PanelAdmin
        public IActionResult PanelAdmin()
        {
            return View();
        }
    }
}