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
            ViewBag.Metricas = _modeloColasService.CalcularMMs(18, 12, 2);
            var turnosActivos = _turnoService.ObtenerTurnosActivos();
            return View(turnosActivos);
        }

        // POST: Atender turno desde el Admin
        [HttpPost]
        public IActionResult AtenderTurno(int idTurno)
        {
            _turnoService.CambiarEstadoTurno(idTurno, "Atendiendo");
            return RedirectToAction("PanelAdmin");
        }

        // POST: Finalizar turno desde el Admin
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
            string estudiante = string.IsNullOrEmpty(nombre) ? "Juan Esteban Lozada" : nombre;
            var turnoActual = _turnoService.ObtenerTurnoPorEstudiante(estudiante);

            ViewBag.NombreEstudiante = estudiante;

            if (turnoActual != null)
            {
                ViewBag.PersonasAdelante = _turnoService.ObtenerPersonasAdelante(turnoActual.Id);
                ViewBag.TiempoEstimadoMin = (ViewBag.PersonasAdelante + 1) * 5;
            }

            return View(turnoActual);
        }

        // POST: Pedir Turno desde el Estudiante
        [HttpPost]
        public IActionResult PedirTurno(string nombreEstudiante)
        {
            _turnoService.GenerarTurno(nombreEstudiante);
            return RedirectToAction("PanelEstudiante", new { nombre = nombreEstudiante });
        }
    }
}