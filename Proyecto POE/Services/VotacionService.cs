using System;
using System.Collections.Generic;
using ReinaFIEC.Models;
using ReinaFIEC.Repositories;
using ReinaFIEC.Data;

namespace ReinaFIEC.Services
{
    /// <summary>
    /// Servicio para gestión de votaciones
    /// </summary>
    public class VotacionService
    {
        private readonly VotoRepository _votoRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly CandidataRepository _candidataRepository;
        private readonly AuthService _authService;

        public VotacionService(VotoRepository votoRepository, UsuarioRepository usuarioRepository,
                              CandidataRepository candidataRepository, AuthService authService)
        {
            _votoRepository = votoRepository;
            _usuarioRepository = usuarioRepository;
            _candidataRepository = candidataRepository;
            _authService = authService;
        }

        /// <summary>
        /// Registra un voto
        /// </summary>
        public ResultadoOperacion Votar(int candidataId, TipoVoto tipoVoto, string ipAddress = null)
        {
            try
            {
                // Verificar autenticación
                if (!_authService.EstaAutenticado)
                    return ResultadoOperacion.Fallo("Debe iniciar sesión para votar");

                var usuario = _authService.UsuarioActual;

                // Verificar que sea estudiante
                if (usuario.Rol != TipoUsuario.Estudiante)
                    return ResultadoOperacion.Fallo("Solo los estudiantes pueden votar");

                // Verificar si ya votó
                if (_votoRepository.UsuarioYaVoto(usuario.Id, tipoVoto))
                {
                    string categoria = tipoVoto == TipoVoto.Reina ? "Reina" : "Miss Fotogenia";
                    return ResultadoOperacion.Fallo($"Ya has votado en la categoría {categoria}");
                }

                // Verificar que la candidata existe y está activa
                var candidata = _candidataRepository.ObtenerPorId(candidataId);
                if (candidata == null || !candidata.Activo)
                    return ResultadoOperacion.Fallo("La candidata seleccionada no es válida");

                // Registrar el voto
                var voto = new Voto
                {
                    UsuarioId = usuario.Id,
                    CandidataId = candidataId,
                    TipoVoto = tipoVoto,
                    IpAddress = ipAddress,
                    FechaVoto = DateTime.Now
                };

                _votoRepository.RegistrarVoto(voto);

                // Actualizar estado del usuario
                _usuarioRepository.RegistrarVoto(usuario.Id, tipoVoto);

                // Registrar auditoría
                var auditoria = new AuditoriaVoto
                {
                    Accion = "INSERT",
                    UsuarioId = usuario.Id,
                    CandidataId = candidataId,
                    TipoVoto = tipoVoto,
                    IpAddress = ipAddress,
                    DetallesJson = $"{{\"usuario\":\"{usuario.NombreCompleto}\",\"candidata\":\"{candidata.NombreCompleto}\"}}"
                };
                _votoRepository.RegistrarAuditoria(auditoria);

                // Actualizar usuario actual
                if (tipoVoto == TipoVoto.Reina)
                {
                    usuario.HaVotadoReina = true;
                    usuario.FechaVotoReina = DateTime.Now;
                }
                else
                {
                    usuario.HaVotadoFotogenia = true;
                    usuario.FechaVotoFotogenia = DateTime.Now;
                }

                return ResultadoOperacion.Exito("¡Voto registrado exitosamente!");
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Fallo($"Error al registrar voto: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si el usuario puede votar en una categoría
        /// </summary>
        public ResultadoOperacion PuedeVotar(TipoVoto tipoVoto)
        {
            if (!_authService.EstaAutenticado)
                return ResultadoOperacion.Fallo("Debe iniciar sesión");

            var usuario = _authService.UsuarioActual;

            if (usuario.Rol != TipoUsuario.Estudiante)
                return ResultadoOperacion.Fallo("Solo los estudiantes pueden votar");

            bool yaVoto = tipoVoto == TipoVoto.Reina ? usuario.HaVotadoReina : usuario.HaVotadoFotogenia;

            if (yaVoto)
            {
                string categoria = tipoVoto == TipoVoto.Reina ? "Reina" : "Miss Fotogenia";
                DateTime? fechaVoto = tipoVoto == TipoVoto.Reina ? usuario.FechaVotoReina : usuario.FechaVotoFotogenia;
                return ResultadoOperacion.Fallo($"Ya votó en la categoría {categoria} el {fechaVoto:dd/MM/yyyy HH:mm}");
            }

            return ResultadoOperacion.Exito("Puede votar");
        }

        /// <summary>
        /// Obtiene los resultados de la votación
        /// </summary>
        public IEnumerable<ResultadoVotacion> ObtenerResultados(TipoVoto tipoVoto)
        {
            try
            {
                return _votoRepository.ObtenerResultados(tipoVoto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener resultados: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene la ganadora de una categoría
        /// </summary>
        public ResultadoVotacion ObtenerGanadora(TipoVoto tipoVoto)
        {
            var resultados = ObtenerResultados(tipoVoto);
            foreach (var resultado in resultados)
            {
                if (resultado.Posicion == 1)
                    return resultado;
            }
            return null;
        }

        /// <summary>
        /// Obtiene estadísticas generales de votación
        /// </summary>
        public EstadisticasVotacion ObtenerEstadisticas()
        {
            try
            {
                int totalCandidatas = _candidataRepository.Contar();
                int totalVotosReina = _votoRepository.ObtenerTotalVotosPorTipo(TipoVoto.Reina);
                int totalVotosFotogenia = _votoRepository.ObtenerTotalVotosPorTipo(TipoVoto.MissFotogenia);

                var resultadosReina = ObtenerResultados(TipoVoto.Reina);
                var resultadosFotogenia = ObtenerResultados(TipoVoto.MissFotogenia);

                return new EstadisticasVotacion
                {
                    TotalCandidatas = totalCandidatas,
                    TotalVotosReina = totalVotosReina,
                    TotalVotosFotogenia = totalVotosFotogenia,
                    ParticipacionReina = totalCandidatas > 0 ? (totalVotosReina * 100.0 / totalCandidatas) : 0,
                    ParticipacionFotogenia = totalCandidatas > 0 ? (totalVotosFotogenia * 100.0 / totalCandidatas) : 0,
                    GanadoraReina = ObtenerGanadora(TipoVoto.Reina),
                    GanadoraFotogenia = ObtenerGanadora(TipoVoto.MissFotogenia)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Estadísticas generales de votación
    /// </summary>
    public class EstadisticasVotacion
    {
        public int TotalCandidatas { get; set; }
        public int TotalVotosReina { get; set; }
        public int TotalVotosFotogenia { get; set; }
        public double ParticipacionReina { get; set; }
        public double ParticipacionFotogenia { get; set; }
        public ResultadoVotacion GanadoraReina { get; set; }
        public ResultadoVotacion GanadoraFotogenia { get; set; }
    }
}
