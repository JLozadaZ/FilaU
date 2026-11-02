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

        // GET: /Login/UserLogin
        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        // POST: /Login/UserLogin
        [HttpPost]
        public IActionResult UserLogin(string correo, string password)
        {
            var usuario = _authService.ValidarUsuario(correo, password, "Estudiante");
            
            if (usuario != null)
            {
                // Pasa el nombre del usuario autenticado como parámetro al panel
                return RedirectToAction("PanelEstudiante", "Panel", new { nombre = usuario.Nombre });
            }

            ViewBag.Error = "Correo o contraseña incorrectos para el rol de Estudiante.";
            return View();
        }

        // GET: /Login/AdminLogin
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: /Login/AdminLogin
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