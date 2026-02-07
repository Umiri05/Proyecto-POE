using System;

namespace ReinaFIEC.Models
{
    /// <summary>
    /// Representa un usuario del sistema (Administrador o Estudiante)
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public TipoUsuario Rol { get; set; }
        public string Matricula { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public bool Activo { get; set; }
        
        // Propiedades de navegación
        public bool HaVotadoReina { get; set; }
        public bool HaVotadoFotogenia { get; set; }
        public DateTime? FechaVotoReina { get; set; }
        public DateTime? FechaVotoFotogenia { get; set; }

        public Usuario()
        {
            FechaCreacion = DateTime.Now;
            Activo = true;
            HaVotadoReina = false;
            HaVotadoFotogenia = false;
        }
    }

    /// <summary>
    /// Tipos de usuario en el sistema
    /// </summary>
    public enum TipoUsuario
    {
        Administrador = 1,
        Estudiante = 2
    }
}
