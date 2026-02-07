using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using ReinaFIEC.Models;

namespace ReinaFIEC.Repositories
{
    /// <summary>
    /// Repositorio para gestión de usuarios con consultas parametrizadas seguras
    /// </summary>
    public class UsuarioRepository : IRepository<Usuario>
    {
        private readonly DatabaseContext _context;

        public UsuarioRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Usuario ObtenerPorId(int id)
        {
            string sql = @"SELECT Id, Username, PasswordHash, Rol, Matricula, Email, NombreCompleto, 
                          FechaCreacion, UltimoAcceso, Activo, HaVotadoReina, HaVotadoFotogenia,
                          FechaVotoReina, FechaVotoFotogenia
                          FROM Usuarios WHERE Id = @Id";

            using (var reader = _context.ExecuteReader(sql, new SqlParameter("@Id", id)))
            {
                if (reader.Read())
                {
                    return MapearUsuario(reader);
                }
            }
            return null;
        }

        public IEnumerable<Usuario> ObtenerTodos()
        {
            var usuarios = new List<Usuario>();
            string sql = @"SELECT Id, Username, PasswordHash, Rol, Matricula, Email, NombreCompleto, 
                          FechaCreacion, UltimoAcceso, Activo, HaVotadoReina, HaVotadoFotogenia,
                          FechaVotoReina, FechaVotoFotogenia
                          FROM Usuarios WHERE Activo = 1
                          ORDER BY NombreCompleto";

            using (var reader = _context.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    usuarios.Add(MapearUsuario(reader));
                }
            }
            return usuarios;
        }

        public Usuario ObtenerPorUsername(string username)
        {
            string sql = @"SELECT Id, Username, PasswordHash, Rol, Matricula, Email, NombreCompleto, 
                          FechaCreacion, UltimoAcceso, Activo, HaVotadoReina, HaVotadoFotogenia,
                          FechaVotoReina, FechaVotoFotogenia
                          FROM Usuarios WHERE Username = @Username AND Activo = 1";

            using (var reader = _context.ExecuteReader(sql, new SqlParameter("@Username", username)))
            {
                if (reader.Read())
                {
                    return MapearUsuario(reader);
                }
            }
            return null;
        }

        public Usuario ObtenerPorMatricula(string matricula)
        {
            string sql = @"SELECT Id, Username, PasswordHash, Rol, Matricula, Email, NombreCompleto, 
                          FechaCreacion, UltimoAcceso, Activo, HaVotadoReina, HaVotadoFotogenia,
                          FechaVotoReina, FechaVotoFotogenia
                          FROM Usuarios WHERE Matricula = @Matricula AND Activo = 1";

            using (var reader = _context.ExecuteReader(sql, new SqlParameter("@Matricula", matricula)))
            {
                if (reader.Read())
                {
                    return MapearUsuario(reader);
                }
            }
            return null;
        }

        public void Agregar(Usuario usuario)
        {
            string sql = @"INSERT INTO Usuarios 
                          (Username, PasswordHash, Rol, Matricula, Email, NombreCompleto, FechaCreacion, Activo)
                          VALUES (@Username, @PasswordHash, @Rol, @Matricula, @Email, @NombreCompleto, @FechaCreacion, @Activo);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

            var parameters = new[]
            {
                new SqlParameter("@Username", usuario.Username),
                new SqlParameter("@PasswordHash", usuario.PasswordHash),
                new SqlParameter("@Rol", (int)usuario.Rol),
                new SqlParameter("@Matricula", (object)usuario.Matricula ?? DBNull.Value),
                new SqlParameter("@Email", usuario.Email),
                new SqlParameter("@NombreCompleto", usuario.NombreCompleto),
                new SqlParameter("@FechaCreacion", usuario.FechaCreacion),
                new SqlParameter("@Activo", usuario.Activo)
            };

            usuario.Id = Convert.ToInt32(_context.ExecuteScalar(sql, parameters));
        }

        public void Actualizar(Usuario usuario)
        {
            string sql = @"UPDATE Usuarios SET 
                          Username = @Username,
                          Email = @Email,
                          NombreCompleto = @NombreCompleto,
                          Matricula = @Matricula,
                          Rol = @Rol,
                          Activo = @Activo,
                          UltimoAcceso = @UltimoAcceso,
                          HaVotadoReina = @HaVotadoReina,
                          HaVotadoFotogenia = @HaVotadoFotogenia,
                          FechaVotoReina = @FechaVotoReina,
                          FechaVotoFotogenia = @FechaVotoFotogenia
                          WHERE Id = @Id";

            var parameters = new[]
            {
                new SqlParameter("@Id", usuario.Id),
                new SqlParameter("@Username", usuario.Username),
                new SqlParameter("@Email", usuario.Email),
                new SqlParameter("@NombreCompleto", usuario.NombreCompleto),
                new SqlParameter("@Matricula", (object)usuario.Matricula ?? DBNull.Value),
                new SqlParameter("@Rol", (int)usuario.Rol),
                new SqlParameter("@Activo", usuario.Activo),
                new SqlParameter("@UltimoAcceso", (object)usuario.UltimoAcceso ?? DBNull.Value),
                new SqlParameter("@HaVotadoReina", usuario.HaVotadoReina),
                new SqlParameter("@HaVotadoFotogenia", usuario.HaVotadoFotogenia),
                new SqlParameter("@FechaVotoReina", (object)usuario.FechaVotoReina ?? DBNull.Value),
                new SqlParameter("@FechaVotoFotogenia", (object)usuario.FechaVotoFotogenia ?? DBNull.Value)
            };

            _context.ExecuteNonQuery(sql, parameters);
        }

        public void ActualizarPassword(int usuarioId, string nuevoPasswordHash)
        {
            string sql = "UPDATE Usuarios SET PasswordHash = @PasswordHash WHERE Id = @Id";
            
            var parameters = new[]
            {
                new SqlParameter("@Id", usuarioId),
                new SqlParameter("@PasswordHash", nuevoPasswordHash)
            };

            _context.ExecuteNonQuery(sql, parameters);
        }

        public void RegistrarVoto(int usuarioId, TipoVoto tipoVoto)
        {
            string sql;
            if (tipoVoto == TipoVoto.Reina)
            {
                sql = @"UPDATE Usuarios SET 
                       HaVotadoReina = 1, 
                       FechaVotoReina = @FechaVoto 
                       WHERE Id = @Id";
            }
            else
            {
                sql = @"UPDATE Usuarios SET 
                       HaVotadoFotogenia = 1, 
                       FechaVotoFotogenia = @FechaVoto 
                       WHERE Id = @Id";
            }

            var parameters = new[]
            {
                new SqlParameter("@Id", usuarioId),
                new SqlParameter("@FechaVoto", DateTime.Now)
            };

            _context.ExecuteNonQuery(sql, parameters);
        }

        public void Eliminar(int id)
        {
            // Eliminación lógica
            string sql = "UPDATE Usuarios SET Activo = 0 WHERE Id = @Id";
            _context.ExecuteNonQuery(sql, new SqlParameter("@Id", id));
        }

        public void Eliminar(Usuario entidad)
        {
            Eliminar(entidad.Id);
        }

        public int Contar()
        {
            string sql = "SELECT COUNT(*) FROM Usuarios WHERE Activo = 1";
            return Convert.ToInt32(_context.ExecuteScalar(sql));
        }

        public bool Existe(Expression<Func<Usuario, bool>> predicado)
        {
            // Implementación básica - en producción usar Expression Trees
            throw new NotImplementedException("Use métodos específicos como ExisteUsername");
        }

        public bool ExisteUsername(string username)
        {
            string sql = "SELECT COUNT(*) FROM Usuarios WHERE Username = @Username AND Activo = 1";
            int count = Convert.ToInt32(_context.ExecuteScalar(sql, new SqlParameter("@Username", username)));
            return count > 0;
        }

        public bool ExisteMatricula(string matricula)
        {
            string sql = "SELECT COUNT(*) FROM Usuarios WHERE Matricula = @Matricula AND Activo = 1";
            int count = Convert.ToInt32(_context.ExecuteScalar(sql, new SqlParameter("@Matricula", matricula)));
            return count > 0;
        }

        // Métodos no implementados del IRepository genérico
        public IEnumerable<Usuario> Buscar(Expression<Func<Usuario, bool>> predicado)
        {
            throw new NotImplementedException("Use métodos específicos de búsqueda");
        }

        public Usuario ObtenerUno(Expression<Func<Usuario, bool>> predicado)
        {
            throw new NotImplementedException("Use métodos específicos como ObtenerPorUsername");
        }

        public int Contar(Expression<Func<Usuario, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                Rol = (TipoUsuario)reader.GetInt32(reader.GetOrdinal("Rol")),
                Matricula = reader.IsDBNull(reader.GetOrdinal("Matricula")) ? null : reader.GetString(reader.GetOrdinal("Matricula")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                UltimoAcceso = reader.IsDBNull(reader.GetOrdinal("UltimoAcceso")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UltimoAcceso")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                HaVotadoReina = reader.GetBoolean(reader.GetOrdinal("HaVotadoReina")),
                HaVotadoFotogenia = reader.GetBoolean(reader.GetOrdinal("HaVotadoFotogenia")),
                FechaVotoReina = reader.IsDBNull(reader.GetOrdinal("FechaVotoReina")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaVotoReina")),
                FechaVotoFotogenia = reader.IsDBNull(reader.GetOrdinal("FechaVotoFotogenia")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaVotoFotogenia"))
            };
        }
    }
}
