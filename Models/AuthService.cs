using FilaU.Models;

namespace FilaU.Services
{
    public class AuthService
    {
        // Lista fija con las cuentas autorizadas que tú escojas
        private readonly List<Usuario> _usuarios = new()
        {
            // Cuentas de Estudiantes
            new Usuario { Email = "juan@iue.edu.co", Password = "123", Nombre = "Juan Esteban", Rol = "Estudiante" },
            new Usuario { Email = "nashly@iue.edu.co", Password = "123", Nombre = "Nashly Herrán", Rol = "Estudiante" },
            new Usuario { Email = "sebastian@iue.edu.co", Password = "123", Nombre = "Sebastián Mesa", Rol = "Estudiante" },

            // Cuentas de Administrador / Servidor
            new Usuario { Email = "admin@iue.edu.co", Password = "admin123", Nombre = "Administrador Cafetería", Rol = "Admin" },
            new Usuario { Email = "123@123", Password = "123", Nombre = "PruebaAdmin", Rol = "Admin" }
        };

        public Usuario? ValidarUsuario(string email, string password, string rolRequerido)
        {
            return _usuarios.FirstOrDefault(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password && 
                u.Rol == rolRequerido);
        }
    }
}