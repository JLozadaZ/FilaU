using Microsoft.AspNetCore.Mvc;
using FilaU.Services;

namespace FilaU.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthService _authService;

        public LoginController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult UserLogin() => View();

        [HttpPost]
        public IActionResult UserLogin(string correo, string password)
        {
            var usuario = _authService.ValidarUsuario(correo, password, "Estudiante");
            
            if (usuario != null)
            {
                return RedirectToAction("PanelEstudiante", "Panel");
            }

            ViewBag.Error = "Correo o contraseña incorrectos para el rol de Estudiante.";
            return View();
        }

        [HttpGet]
        public IActionResult AdminLogin() => View();


        [HttpPost]
        public IActionResult AdminLogin(string correo, string password)
        {
            var usuario = _authService.ValidarUsuario(correo, password, "Admin");
            
            if (usuario != null)
            {
                
                return RedirectToAction("PanelAdmin", "Panel");
            }

            ViewBag.Error = "Usuario o contraseña de administrador incorrectos.";
            return View();
        }
    }
}