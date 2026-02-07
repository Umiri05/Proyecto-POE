using System;
using System.Linq;
using ReinaFIEC.Models;
using ReinaFIEC.Repositories;
using ReinaFIEC.Utils;

namespace ReinaFIEC.Services
{
    /// <summary>
    /// Servicio para manejo de autenticación y autorización
    /// </summary>
    public class AuthService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private Usuario _usuarioActual;

        public Usuario UsuarioActual => _usuarioActual;
        public bool EstaAutenticado => _usuarioActual != null;
        public bool EsAdministrador => _usuarioActual?.Rol == TipoUsuario.Administrador;

        public AuthService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        /// <summary>
        /// Intenta autenticar un usuario
        /// </summary>
        public ResultadoOperacion Login(string username, string password)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(username))
                    return ResultadoOperacion.Fallo("El nombre de usuario es requerido");

                if (string.IsNullOrWhiteSpace(password))
                    return ResultadoOperacion.Fallo("La contraseña es requerida");

                // Buscar usuario
                var usuario = _usuarioRepository.ObtenerPorUsername(username);
                
                if (usuario == null)
                    return ResultadoOperacion.Fallo("Usuario o contraseña incorrectos");

                if (!usuario.Activo)
                    return ResultadoOperacion.Fallo("Usuario inactivo. Contacte al administrador");

                // Verificar contraseña
                if (!PasswordHasher.VerifyPassword(password, usuario.PasswordHash))
                    return ResultadoOperacion.Fallo("Usuario o contraseña incorrectos");

                // Actualizar último acceso
                usuario.UltimoAcceso = DateTime.Now;
                _usuarioRepository.Actualizar(usuario);

                _usuarioActual = usuario;
                return ResultadoOperacion.Exito($"Bienvenido, {usuario.NombreCompleto}");
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Fallo($"Error al iniciar sesión: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra un nuevo estudiante
        /// </summary>
        public ResultadoOperacion RegistrarEstudiante(string username, string password, string email, 
                                                     string nombreCompleto, string matricula)
        {
            try
            {
                // Validaciones
                var errores = ValidarDatosRegistro(username, password, email, nombreCompleto, matricula);
                if (errores.Any())
                    return ResultadoOperacion.Fallo(string.Join(", ", errores));

                // Verificar si ya existe
                if (_usuarioRepository.ExisteUsername(username))
                    return ResultadoOperacion.Fallo("El nombre de usuario ya está en uso");

                if (_usuarioRepository.ExisteMatricula(matricula))
                    return ResultadoOperacion.Fallo("La matrícula ya está registrada");

                // Crear usuario
                var usuario = new Usuario
                {
                    Username = username,
                    PasswordHash = PasswordHasher.HashPassword(password),
                    Email = email,
                    NombreCompleto = nombreCompleto,
                    Matricula = matricula,
                    Rol = TipoUsuario.Estudiante,
                    Activo = true
                };

                _usuarioRepository.Agregar(usuario);
                return ResultadoOperacion.Exito("Usuario registrado exitosamente");
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Fallo($"Error al registrar usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario actual
        /// </summary>
        public ResultadoOperacion CambiarPassword(string passwordActual, string nuevoPassword)
        {
            try
            {
                if (!EstaAutenticado)
                    return ResultadoOperacion.Fallo("Debe iniciar sesión primero");

                if (!PasswordHasher.VerifyPassword(passwordActual, _usuarioActual.PasswordHash))
                    return ResultadoOperacion.Fallo("La contraseña actual es incorrecta");

                var validacion = ValidarPassword(nuevoPassword);
                if (!validacion.Exitoso)
                    return validacion;

                string nuevoHash = PasswordHasher.HashPassword(nuevoPassword);
                _usuarioRepository.ActualizarPassword(_usuarioActual.Id, nuevoHash);

                return ResultadoOperacion.Exito("Contraseña actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Fallo($"Error al cambiar contraseña: {ex.Message}");
            }
        }

        /// <summary>
        /// Cierra la sesión actual
        /// </summary>
        public void Logout()
        {
            _usuarioActual = null;
        }

        private System.Collections.Generic.List<string> ValidarDatosRegistro(string username, string password, 
                                                                              string email, string nombreCompleto, 
                                                                              string matricula)
        {
            var errores = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
                errores.Add("El usuario debe tener al menos 4 caracteres");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                errores.Add("Email inválido");

            if (string.IsNullOrWhiteSpace(nombreCompleto))
                errores.Add("El nombre completo es requerido");

            if (string.IsNullOrWhiteSpace(matricula) || matricula.Length < 6)
                errores.Add("La matrícula debe tener al menos 6 caracteres");

            var validacionPassword = ValidarPassword(password);
            if (!validacionPassword.Exitoso)
                errores.Add(validacionPassword.Mensaje);

            return errores;
        }

        private ResultadoOperacion ValidarPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return ResultadoOperacion.Fallo("La contraseña es requerida");

            if (password.Length < 6)
                return ResultadoOperacion.Fallo("La contraseña debe tener al menos 6 caracteres");

            if (!password.Any(char.IsUpper))
                return ResultadoOperacion.Fallo("La contraseña debe contener al menos una mayúscula");

            if (!password.Any(char.IsDigit))
                return ResultadoOperacion.Fallo("La contraseña debe contener al menos un número");

            return ResultadoOperacion.Exito("Contraseña válida");
        }
    }

    /// <summary>
    /// Resultado de una operación
    /// </summary>
    public class ResultadoOperacion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public object Datos { get; set; }

        public static ResultadoOperacion Exito(string mensaje, object datos = null)
        {
            return new ResultadoOperacion { Exitoso = true, Mensaje = mensaje, Datos = datos };
        }

        public static ResultadoOperacion Fallo(string mensaje)
        {
            return new ResultadoOperacion { Exitoso = false, Mensaje = mensaje };
        }
    }
}
