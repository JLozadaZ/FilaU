namespace FilaU.Models
{
    public class Turno
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty; // Ej: "A01", "A02"
        public string NombreEstudiante { get; set; } = string.Empty;
        public DateTime HoraSolicitud { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "En Espera"; // "En Espera", "Atendiendo", "Finalizado"
    }
}