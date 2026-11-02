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

  
        [HttpGet]
        public IActionResult PanelAdmin()
        {

            ViewBag.Metricas = _modeloColasService.CalcularMMs(18, 12, 2);


            var turnosActivos = _turnoService.ObtenerTurnosActivos();


            int clientesEnColaReal = turnosActivos.Count(t => t.Estado == "En Espera");
            int clientesAtendiendo = turnosActivos.Count(t => t.Estado == "Atendiendo");
            int totalEnSistema = clientesEnColaReal + clientesAtendiendo;

            ViewBag.LqReal = clientesEnColaReal;
            ViewBag.LReal = totalEnSistema;
            

            ViewBag.WqReal = clientesEnColaReal > 0 ? (clientesEnColaReal * 5.0) / 2.0 : 0.0;
            
 
            double ocupacionReal = (double)clientesAtendiendo / 2.0;
            ViewBag.UtilizacionReal = Math.Min(ocupacionReal, 1.0);
            

            ViewBag.P0Real = totalEnSistema == 0 ? 1.0 : (1.0 - (double)ViewBag.UtilizacionReal);

            return View(turnosActivos);
        }

 
        [HttpPost]
        public IActionResult AtenderTurno(int idTurno)
        {

            bool exito = _turnoService.CambiarEstadoTurno(idTurno, "Atendiendo", maxServidores: 2);
            
            if (!exito)
            {
                TempData["MensajeError"] = "⚠️ Capacidad máxima alcanzada: Los 2 servidores ya están atendiendo turnos. Finaliza un turno antes de atender a otro estudiante.";
            }

            return RedirectToAction("PanelAdmin");
        }

  
        [HttpPost]
        public IActionResult FinalizarTurno(int idTurno)
        {
            _turnoService.CambiarEstadoTurno(idTurno, "Finalizado");
            return RedirectToAction("PanelAdmin");
        }


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


        [HttpPost]
        public IActionResult PedirTurno(string nombreEstudiante)
        {
            _turnoService.GenerarTurno(nombreEstudiante);
            return RedirectToAction("PanelEstudiante", new { nombre = nombreEstudiante });
        }
    }
}