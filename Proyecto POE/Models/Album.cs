using System;
using System.Collections.Generic;

namespace ReinaFIEC.Models
{
    /// <summary>
    /// Representa un álbum de fotos de una candidata
    /// </summary>
    public class Album
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CandidataId { get; set; }
        public DateTime FechaCreacion { get; set; }
        
        // Navegación
        public Candidata Candidata { get; set; }
        public List<Foto> Fotos { get; set; }

        public Album()
        {
            FechaCreacion = DateTime.Now;
            Fotos = new List<Foto>();
        }
    }

    /// <summary>
    /// Representa una foto dentro de un álbum
    /// </summary>
    public class Foto
    {
        public int Id { get; set; }
        public string RutaArchivo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int AlbumId { get; set; }
        public DateTime FechaSubida { get; set; }
        
        // Navegación
        public Album Album { get; set; }
        public List<Comentario> Comentarios { get; set; }

        public Foto()
        {
            FechaSubida = DateTime.Now;
            Comentarios = new List<Comentario>();
        }
    }

    /// <summary>
    /// Representa un comentario en una foto
    /// </summary>
    public class Comentario
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string Autor { get; set; }
        public int UsuarioId { get; set; }
        public int FotoId { get; set; }
        public DateTime FechaComentario { get; set; }
        public bool Aprobado { get; set; }
        
        // Navegación
        public Usuario Usuario { get; set; }
        public Foto Foto { get; set; }

        public Comentario()
        {
            FechaComentario = DateTime.Now;
            Aprobado = true;
        }

        /// <summary>
        /// Valida el comentario
        /// </summary>
        public List<string> Validar()
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(Texto))
                errores.Add("El comentario no puede estar vacío");

            if (Texto != null && Texto.Length > 500)
                errores.Add("El comentario no puede exceder 500 caracteres");

            return errores;
        }
    }
}
