using FilaU.Models;

namespace FilaU.Services
{
    public class AuthService
    {
       
        private readonly List<Usuario> _usuarios = new()
        {
        
            new Usuario { Email = "jlozada@correo.iue.edu.co", Password = "123", Nombre = "Juan Esteban", Rol = "Estudiante" },
            new Usuario { Email = "nashly@correo.iue.edu.co", Password = "123", Nombre = "Nashly Herrán", Rol = "Estudiante" },
            new Usuario { Email = "sebastian@correo.iue.edu.co", Password = "123", Nombre = "Sebastián Mesa", Rol = "Estudiante" },
            new Usuario { Email = "daniel@correo.iue.edu.co", Password = "123", Nombre = "Daniel Rodríguez", Rol = "Estudiante" },
            new Usuario { Email = "liliana@correo.iue.edu.co", Password = "123", Nombre = "Liliana Trujillo", Rol = "Estudiante" },

        
            new Usuario { Email = "admin@correo.iue.edu.co", Password = "admin123", Nombre = "Administrador Cafetería", Rol = "Admin" },
            new Usuario { Email = "juaneslozada032107@gmail.com", Password = "admin123", Nombre = "Propietario", Rol = "Admin" }
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