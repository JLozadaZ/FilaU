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
            // 1. Métricas Teóricas M/M/s (λ = 96 personas/hora, μ = 60 personas/hora, s = 2 servidores)
            ViewBag.Metricas = _modeloColasService.CalcularMMs(96, 60, 2);

            // 2. Obtener lista de turnos activos en tiempo real desde el servicio
            var turnosActivos = _turnoService.ObtenerTurnosActivos();

            // 3. Cálculos Dinámicos en Tiempo Real (Ajustados a μ = 60 clientes/hora)
            int clientesEnColaReal = turnosActivos.Count(t => t.Estado == "En Espera");
            int clientesAtendiendo = turnosActivos.Count(t => t.Estado == "Atendiendo");
            int totalEnSistema = clientesEnColaReal + clientesAtendiendo;
            int s = 2; // Número de servidores (cajas)
            double mu = 60.0; // Capacidad de 60 clientes/hora/servidor (1.0 min por atención)

            // Tasa de llegada observada dinámicamente según el volumen actual en la app
            double lambdaObservada = totalEnSistema * 4.0;

            // FÓRMULA FORMAL DE IO: ρ_real = λ_real / (s * μ) -> λ_real / 120
            double capacidadTotalReal = s * mu;
            double utilizacionReal = lambdaObservada / capacidadTotalReal;

            // PROBABILIDAD DE SISTEMA VACÍO OBSERVADA (P0_real)
            double p0Real = totalEnSistema == 0 ? 1.0 : Math.Max(0.0, 1.0 - utilizacionReal);

            // Tiempo de espera en cola real (Wq en minutos con atención de 1.0 min por persona)
            double wqRealMinutes = clientesEnColaReal > 0 ? (clientesEnColaReal * 60.0) / (s * mu) : 0.0;
            
            // Tiempo total en el sistema real (W = Wq + 1.0 min de atención)
            double wRealMinutes = totalEnSistema > 0 ? wqRealMinutes + 1.0 : 0.0;

            // Envío de todos los indicadores dinámicos a la vista mediante ViewBag
            ViewBag.LambdaReal = lambdaObservada;
            ViewBag.MuReal = mu;
            ViewBag.ServidoresReal = s;
            ViewBag.UtilizacionReal = utilizacionReal;
            ViewBag.P0Real = p0Real;
            ViewBag.LqReal = clientesEnColaReal;
            ViewBag.LReal = totalEnSistema;
            ViewBag.WqReal = wqRealMinutes;
            ViewBag.WReal = wRealMinutes;

            return View(turnosActivos);
        }

        // POST: Atender turno desde el Panel Admin
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
                ViewBag.TiempoEstimadoMin = (ViewBag.PersonasAdelante + 1) * 1.0; // Calculado a 1.0 min por persona
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