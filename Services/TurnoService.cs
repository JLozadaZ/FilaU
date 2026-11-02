using FilaU.Models;

namespace FilaU.Services
{
    public class TurnoService
    {
        private readonly List<Turno> _turnos = new();
        private int _contadorSecuencial = 1;

        public List<Turno> ObtenerTurnosActivos()
        {
            lock (_turnos)
            {
                return _turnos.Where(t => t.Estado != "Finalizado").OrderBy(t => t.Id).ToList();
            }
        }

        public Turno? ObtenerTurnoPorEstudiante(string nombreEstudiante)
        {
            lock (_turnos)
            {
                return _turnos.FirstOrDefault(t => t.NombreEstudiante.Equals(nombreEstudiante, StringComparison.OrdinalIgnoreCase) && t.Estado != "Finalizado");
            }
        }

        public Turno GenerarTurno(string nombreEstudiante)
        {
            lock (_turnos)
            {
                var existente = ObtenerTurnoPorEstudiante(nombreEstudiante);
                if (existente != null) return existente;

                var nuevoTurno = new Turno
                {
                    Id = _contadorSecuencial,
                    Codigo = $"A{_contadorSecuencial:D02}",
                    NombreEstudiante = nombreEstudiante,
                    HoraSolicitud = DateTime.Now,
                    Estado = "En Espera"
                };

                _turnos.Add(nuevoTurno);
                _contadorSecuencial++;
                return nuevoTurno;
            }
        }

        // Intenta cambiar el estado. Devuelve false si los 2 servidores ya están llenos al intentar atender.
        public bool CambiarEstadoTurno(int idTurno, string nuevoEstado, int maxServidores = 2)
        {
            lock (_turnos)
            {
                if (nuevoEstado == "Atendiendo")
                {
                    int actualmenteAtendiendo = _turnos.Count(t => t.Estado == "Atendiendo");
                    if (actualmenteAtendiendo >= maxServidores)
                    {
                        return false; // Capacidad máxima alcanzada
                    }
                }

                var turno = _turnos.FirstOrDefault(t => t.Id == idTurno);
                if (turno != null)
                {
                    turno.Estado = nuevoEstado;
                    return true;
                }
                return false;
            }
        }

        public int ObtenerPersonasAdelante(int idTurno)
        {
            lock (_turnos)
            {
                return _turnos.Count(t => t.Estado == "En Espera" && t.Id < idTurno);
            }
        }
    }
}