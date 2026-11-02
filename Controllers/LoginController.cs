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

        // GET: Muestra la vista
        [HttpGet]
        public IActionResult UserLogin() => View();

        // POST: Procesa el login del Estudiante
        [HttpPost]
        public IActionResult UserLogin(string correo, string password)
        {
            var usuario = _authService.ValidarUsuario(correo, password, "Estudiante");
            
            if (usuario != null)
            {
                // Credenciales correctas -> Redirige al Panel del Estudiante
                return RedirectToAction("PanelEstudiante", "Panel");
            }

            ViewBag.Error = "Correo o contraseña incorrectos para el rol de Estudiante.";
            return View();
        }

        // GET: Muestra la vista Admin
        [HttpGet]
        public IActionResult AdminLogin() => View();

        // POST: Procesa el login del Admin
        [HttpPost]
        public IActionResult AdminLogin(string correo, string password)
        {
            var usuario = _authService.ValidarUsuario(correo, password, "Admin");
            
            if (usuario != null)
            {
                // Credenciales correctas -> Redirige al Panel Admin
                return RedirectToAction("PanelAdmin", "Panel");
            }

            ViewBag.Error = "Usuario o contraseña de administrador incorrectos.";
            return View();
        }
    }
}