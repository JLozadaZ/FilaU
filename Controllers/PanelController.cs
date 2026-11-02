using Microsoft.AspNetCore.Mvc;
using FilaU.Services;
using FilaU.Models;

namespace FilaU.Controllers
{
    public class PanelController : Controller
    {
        private readonly ModeloColasService _modeloColasService;
        private readonly TurnoService _turnoService;

        public PanelController(ModeloColasService modeloColasService, TurnoService turnoService)
        {
            _modeloColasService = modeloColasService;
            _turnoService = turnoService;
        }

        // GET: /Panel/PanelAdmin
        [HttpGet]
        public IActionResult PanelAdmin()
        {
            // 1. Métricas Teóricas M/M/s (Fijas según el planteamiento: λ=18, μ=12, s=2)
            ViewBag.Metricas = _modeloColasService.CalcularMMs(18, 12, 2);

            // 2. Obtener la lista de turnos en tiempo real
            var turnosActivos = _turnoService.ObtenerTurnosActivos();

            // 3. Cálculos dinámicos (Observados en el momento actual)
            int clientesEnColaReal = turnosActivos.Count(t => t.Estado == "En Espera");
            int clientesAtendiendo = turnosActivos.Count(t => t.Estado == "Atendiendo");
            int totalEnSistema = clientesEnColaReal + clientesAtendiendo;

            ViewBag.LqReal = clientesEnColaReal;
            ViewBag.LReal = totalEnSistema;
            
            // Tiempo estimado real para el último de la fila (5 min por turno por servidor)
            ViewBag.WqReal = clientesEnColaReal > 0 ? (clientesEnColaReal * 5.0) / 2.0 : 0.0;
            
            // Ocupación real de los 2 servidores
            double ocupacionReal = (double)clientesAtendiendo / 2.0;
            ViewBag.UtilizacionReal = Math.Min(ocupacionReal, 1.0);
            
            // Probabilidad de sistema vacío en este instante
            ViewBag.P0Real = totalEnSistema == 0 ? 1.0 : (1.0 - (double)ViewBag.UtilizacionReal);

            return View(turnosActivos);
        }

        // POST: Atender turno desde el Panel Admin
        [HttpPost]
        public IActionResult AtenderTurno(int idTurno)
        {
            // Validamos que no supere el límite de 2 servidores
            bool exito = _turnoService.CambiarEstadoTurno(idTurno, "Atendiendo", maxServidores: 2);
            
            if (!exito)
            {
                TempData["MensajeError"] = "⚠️ Capacidad máxima alcanzada: Los 2 servidores ya están atendiendo turnos. Finaliza un turno antes de atender a otro estudiante.";
            }

            return RedirectToAction("PanelAdmin");
        }

        // POST: Finalizar turno desde el Panel Admin
        [HttpPost]
        public IActionResult FinalizarTurno(int idTurno)
        {
            _turnoService.CambiarEstadoTurno(idTurno, "Finalizado");
            return RedirectToAction("PanelAdmin");
        }

        // GET: /Panel/PanelEstudiante
        [HttpGet]
        public IActionResult PanelEstudiante(string? nombre)
        {
            string estudiante = string.IsNullOrEmpty(nombre) ? "Estudiante" : nombre;
            var turnoActual = _turnoService.ObtenerTurnoPorEstudiante(estudiante);

            ViewBag.NombreEstudiante = estudiante;

            if (turnoActual != null)
            {
                ViewBag.PersonasAdelante = _turnoService.ObtenerPersonasAdelante(turnoActual.Id);
                ViewBag.TiempoEstimadoMin = (ViewBag.PersonasAdelante + 1) * 5;
            }

            return View(turnoActual);
        }

        // POST: Pedir Turno desde el Panel Estudiante
        [HttpPost]
        public IActionResult PedirTurno(string nombreEstudiante)
        {
            _turnoService.GenerarTurno(nombreEstudiante);
            return RedirectToAction("PanelEstudiante", new { nombre = nombreEstudiante });
        }
    }
}