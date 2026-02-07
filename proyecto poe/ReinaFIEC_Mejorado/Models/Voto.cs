using System;

namespace ReinaFIEC.Models
{
    /// <summary>
    /// Representa un voto emitido por un estudiante
    /// </summary>
    public class Voto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int CandidataId { get; set; }
        public TipoVoto TipoVoto { get; set; }
        public DateTime FechaVoto { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        
        // Navegación
        public Usuario Usuario { get; set; }
        public Candidata Candidata { get; set; }

        public Voto()
        {
            FechaVoto = DateTime.Now;
        }
    }

    /// <summary>
    /// Tipos de votación disponibles
    /// </summary>
    public enum TipoVoto
    {
        Reina = 1,
        MissFotogenia = 2
    }

    /// <summary>
    /// Registro de auditoría para votos
    /// </summary>
    public class AuditoriaVoto
    {
        public int Id { get; set; }
        public int? VotoId { get; set; }
        public string Accion { get; set; } // INSERT, UPDATE, DELETE
        public int UsuarioId { get; set; }
        public int CandidataId { get; set; }
        public TipoVoto TipoVoto { get; set; }
        public DateTime FechaAccion { get; set; }
        public string DetallesJson { get; set; }
        public string IpAddress { get; set; }

        public AuditoriaVoto()
        {
            FechaAccion = DateTime.Now;
        }
    }

    /// <summary>
    /// Resultado de votación para reportes
    /// </summary>
    public class ResultadoVotacion
    {
        public int CandidataId { get; set; }
        public string NombreCompleto { get; set; }
        public string Carrera { get; set; }
        public string FotoPrincipalUrl { get; set; }
        public int TotalVotos { get; set; }
        public int Posicion { get; set; }
        public decimal PorcentajeVotos { get; set; }
        public bool EsGanadora { get; set; }
    }
}
