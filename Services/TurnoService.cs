using FilaU.Models;

namespace FilaU.Services
{
    public class TurnoService
    {
        private readonly List<Turno> _turnos = new();
        private int _contadorSecuencial = 1;

        // Devuelve únicamente los turnos que están "En Espera" o "Atendiendo"
        public List<Turno> ObtenerTurnosActivos()
        {
            lock (_turnos)
            {
                return _turnos.Where(t => t.Estado != "Finalizado").OrderBy(t => t.Id).ToList();
            }
        }

        // Busca si un estudiante ya tiene un turno activo
        public Turno? ObtenerTurnoPorEstudiante(string nombreEstudiante)
        {
            lock (_turnos)
            {
                return _turnos.FirstOrDefault(t => t.NombreEstudiante.Equals(nombreEstudiante, StringComparison.OrdinalIgnoreCase) && t.Estado != "Finalizado");
            }
        }

        // Genera un nuevo turno para el estudiante
        public Turno GenerarTurno(string nombreEstudiante)
        {
            lock (_turnos)
            {
                // Si ya tiene un turno activo, retornamos ese mismo
                var existente = ObtenerTurnoPorEstudiante(nombreEstudiante);
                if (existente != null) return existente;

                var nuevoTurno = new Turno
                {
                    Id = _contadorSecuencial,
                    Codigo = $"A{_contadorSecuencial:D02}", // Formato: A01, A02, A03...
                    NombreEstudiante = nombreEstudiante,
                    HoraSolicitud = DateTime.Now,
                    Estado = "En Espera"
                };

                _turnos.Add(nuevoTurno);
                _contadorSecuencial++;
                return nuevoTurno;
            }
        }

        // Cambia el estado de un turno (Atender / Finalizar)
        public void CambiarEstadoTurno(int idTurno, string nuevoEstado)
        {
            lock (_turnos)
            {
                var turno = _turnos.FirstOrDefault(t => t.Id == idTurno);
                if (turno != null)
                {
                    turno.Estado = nuevoEstado;
                }
            }
        }

        // Calcula cuántas personas tiene adelante un turno específico
        public int ObtenerPersonasAdelante(int idTurno)
        {
            lock (_turnos)
            {
                return _turnos.Count(t => t.Estado == "En Espera" && t.Id < idTurno);
            }
        }
    }
}