using System;
using System.Collections.Generic;

namespace ReinaFIEC.Models
{
    /// <summary>
    /// Representa una candidata a Reina de la Facultad
    /// </summary>
    public class Candidata
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Edad 
        { 
            get 
            { 
                var today = DateTime.Today;
                var age = today.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > today.AddYears(-age)) age--;
                return age;
            } 
        }
        public string Carrera { get; set; }
        public int Semestre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public decimal PromedioAcademico { get; set; }
        public string Pasatiempos { get; set; }
        public string Habilidades { get; set; }
        public string Intereses { get; set; }
        public string AspiracionesFuturo { get; set; }
        public string FotoPrincipalUrl { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int RegistradoPor { get; set; }
        public bool Activo { get; set; }
        
        // Propiedades calculadas
        public int VotosReina { get; set; }
        public int VotosFotogenia { get; set; }
        
        // Colecciones de navegación
        public List<Album> Albumes { get; set; }
        public List<Voto> Votos { get; set; }

        public Candidata()
        {
            FechaRegistro = DateTime.Now;
            Activo = true;
            Albumes = new List<Album>();
            Votos = new List<Voto>();
            VotosReina = 0;
            VotosFotogenia = 0;
        }

        /// <summary>
        /// Valida que todos los datos de la candidata sean correctos
        /// </summary>
        public List<string> Validar()
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(NombreCompleto))
                errores.Add("El nombre completo es requerido");

            if (string.IsNullOrWhiteSpace(Cedula) || Cedula.Length != 10)
                errores.Add("La cédula debe tener 10 dígitos");

            if (Edad < 17 || Edad > 25)
                errores.Add("La edad debe estar entre 17 y 25 años");

            if (Semestre < 1 || Semestre > 12)
                errores.Add("El semestre debe estar entre 1 y 12");

            if (PromedioAcademico < 0 || PromedioAcademico > 10)
                errores.Add("El promedio académico debe estar entre 0 y 10");

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
                errores.Add("El email no es válido");

            return errores;
        }
    }
}
