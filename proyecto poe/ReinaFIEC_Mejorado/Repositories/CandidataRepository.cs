using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using ReinaFIEC.Models;
using ReinaFIEC.Data;

namespace ReinaFIEC.Repositories
{
    public class CandidataRepository : IRepository<Candidata>
    {
        private readonly DatabaseContext _context;

        public CandidataRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Candidata ObtenerPorId(int id)
        {
            string sql = @"SELECT c.*, 
                          (SELECT COUNT(*) FROM Votos WHERE CandidataId = c.Id AND TipoVoto = 1) as VotosReina,
                          (SELECT COUNT(*) FROM Votos WHERE CandidataId = c.Id AND TipoVoto = 2) as VotosFotogenia
                          FROM Candidatas c WHERE c.Id = @Id AND c.Activo = 1";

            using (var reader = _context.ExecuteReader(sql, new SqlParameter("@Id", id)))
            {
                if (reader.Read())
                {
                    return MapearCandidata(reader);
                }
            }
            return null;
        }

        public Candidata ObtenerPorCedula(string cedula)
        {
            string sql = @"SELECT c.*, 
                          (SELECT COUNT(*) FROM Votos WHERE CandidataId = c.Id AND TipoVoto = 1) as VotosReina,
                          (SELECT COUNT(*) FROM Votos WHERE CandidataId = c.Id AND TipoVoto = 2) as VotosFotogenia
                          FROM Candidatas c WHERE c.Cedula = @Cedula AND c.Activo = 1";

            using (var reader = _context.ExecuteReader(sql, new SqlParameter("@Cedula", cedula)))
            {
                if (reader.Read())
                {
                    return MapearCandidata(reader);
                }
            }
            return null;
        }

        public IEnumerable<Candidata> ObtenerTodos()
        {
            var candidatas = new List<Candidata>();
            string sql = @"SELECT c.*, 
                          (SELECT COUNT(*) FROM Votos WHERE CandidataId = c.Id AND TipoVoto = 1) as VotosReina,
                          (SELECT COUNT(*) FROM Votos WHERE CandidataId = c.Id AND TipoVoto = 2) as VotosFotogenia
                          FROM Candidatas c 
                          WHERE c.Activo = 1
                          ORDER BY c.NombreCompleto";

            using (var reader = _context.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    candidatas.Add(MapearCandidata(reader));
                }
            }
            return candidatas;
        }

        public IEnumerable<Candidata> BuscarPorCarrera(string carrera)
        {
            var candidatas = new List<Candidata>();
            string sql = @"SELECT c.*, 
                          (SELECT COUNT(*) FROM Votos WHERE CandidataId = c.Id AND TipoVoto = 1) as VotosReina,
                          (SELECT COUNT(*) FROM Votos WHERE CandidataId = c.Id AND TipoVoto = 2) as VotosFotogenia
                          FROM Candidatas c 
                          WHERE c.Carrera = @Carrera AND c.Activo = 1
                          ORDER BY c.NombreCompleto";

            using (var reader = _context.ExecuteReader(sql, new SqlParameter("@Carrera", carrera)))
            {
                while (reader.Read())
                {
                    candidatas.Add(MapearCandidata(reader));
                }
            }
            return candidatas;
        }

        public void Agregar(Candidata candidata)
        {
            string sql = @"INSERT INTO Candidatas 
                          (NombreCompleto, Cedula, FechaNacimiento, Carrera, Semestre, Email, Telefono, 
                           PromedioAcademico, Pasatiempos, Habilidades, Intereses, AspiracionesFuturo, 
                           FotoPrincipalUrl, FechaRegistro, RegistradoPor, Activo)
                          VALUES (@NombreCompleto, @Cedula, @FechaNacimiento, @Carrera, @Semestre, @Email, 
                                  @Telefono, @PromedioAcademico, @Pasatiempos, @Habilidades, @Intereses, 
                                  @AspiracionesFuturo, @FotoPrincipalUrl, @FechaRegistro, @RegistradoPor, @Activo);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

            var parameters = new[]
            {
                new SqlParameter("@NombreCompleto", candidata.NombreCompleto),
                new SqlParameter("@Cedula", candidata.Cedula),
                new SqlParameter("@FechaNacimiento", candidata.FechaNacimiento),
                new SqlParameter("@Carrera", candidata.Carrera),
                new SqlParameter("@Semestre", candidata.Semestre),
                new SqlParameter("@Email", candidata.Email),
                new SqlParameter("@Telefono", (object)candidata.Telefono ?? DBNull.Value),
                new SqlParameter("@PromedioAcademico", candidata.PromedioAcademico),
                new SqlParameter("@Pasatiempos", (object)candidata.Pasatiempos ?? DBNull.Value),
                new SqlParameter("@Habilidades", (object)candidata.Habilidades ?? DBNull.Value),
                new SqlParameter("@Intereses", (object)candidata.Intereses ?? DBNull.Value),
                new SqlParameter("@AspiracionesFuturo", (object)candidata.AspiracionesFuturo ?? DBNull.Value),
                new SqlParameter("@FotoPrincipalUrl", (object)candidata.FotoPrincipalUrl ?? DBNull.Value),
                new SqlParameter("@FechaRegistro", candidata.FechaRegistro),
                new SqlParameter("@RegistradoPor", candidata.RegistradoPor),
                new SqlParameter("@Activo", candidata.Activo)
            };

            candidata.Id = Convert.ToInt32(_context.ExecuteScalar(sql, parameters));
        }

        public void Actualizar(Candidata candidata)
        {
            string sql = @"UPDATE Candidatas SET 
                          NombreCompleto = @NombreCompleto,
                          Cedula = @Cedula,
                          FechaNacimiento = @FechaNacimiento,
                          Carrera = @Carrera,
                          Semestre = @Semestre,
                          Email = @Email,
                          Telefono = @Telefono,
                          PromedioAcademico = @PromedioAcademico,
                          Pasatiempos = @Pasatiempos,
                          Habilidades = @Habilidades,
                          Intereses = @Intereses,
                          AspiracionesFuturo = @AspiracionesFuturo,
                          FotoPrincipalUrl = @FotoPrincipalUrl
                          WHERE Id = @Id";

            var parameters = new[]
            {
                new SqlParameter("@Id", candidata.Id),
                new SqlParameter("@NombreCompleto", candidata.NombreCompleto),
                new SqlParameter("@Cedula", candidata.Cedula),
                new SqlParameter("@FechaNacimiento", candidata.FechaNacimiento),
                new SqlParameter("@Carrera", candidata.Carrera),
                new SqlParameter("@Semestre", candidata.Semestre),
                new SqlParameter("@Email", candidata.Email),
                new SqlParameter("@Telefono", (object)candidata.Telefono ?? DBNull.Value),
                new SqlParameter("@PromedioAcademico", candidata.PromedioAcademico),
                new SqlParameter("@Pasatiempos", (object)candidata.Pasatiempos ?? DBNull.Value),
                new SqlParameter("@Habilidades", (object)candidata.Habilidades ?? DBNull.Value),
                new SqlParameter("@Intereses", (object)candidata.Intereses ?? DBNull.Value),
                new SqlParameter("@AspiracionesFuturo", (object)candidata.AspiracionesFuturo ?? DBNull.Value),
                new SqlParameter("@FotoPrincipalUrl", (object)candidata.FotoPrincipalUrl ?? DBNull.Value)
            };

            _context.ExecuteNonQuery(sql, parameters);
        }

        public void Eliminar(int id)
        {
            string sql = "UPDATE Candidatas SET Activo = 0 WHERE Id = @Id";
            _context.ExecuteNonQuery(sql, new SqlParameter("@Id", id));
        }

        public void Eliminar(Candidata entidad)
        {
            Eliminar(entidad.Id);
        }

        public int Contar()
        {
            string sql = "SELECT COUNT(*) FROM Candidatas WHERE Activo = 1";
            return Convert.ToInt32(_context.ExecuteScalar(sql));
        }

        public bool ExisteCedula(string cedula, int? excluyendoId = null)
        {
            string sql = "SELECT COUNT(*) FROM Candidatas WHERE Cedula = @Cedula AND Activo = 1";
            
            if (excluyendoId.HasValue)
            {
                sql += " AND Id != @Id";
                var parameters = new[]
                {
                    new SqlParameter("@Cedula", cedula),
                    new SqlParameter("@Id", excluyendoId.Value)
                };
                return Convert.ToInt32(_context.ExecuteScalar(sql, parameters)) > 0;
            }
            else
            {
                return Convert.ToInt32(_context.ExecuteScalar(sql, new SqlParameter("@Cedula", cedula))) > 0;
            }
        }

        public IEnumerable<string> ObtenerCarreras()
        {
            var carreras = new List<string>();
            string sql = "SELECT DISTINCT Carrera FROM Candidatas WHERE Activo = 1 ORDER BY Carrera";

            using (var reader = _context.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    carreras.Add(reader.GetString(0));
                }
            }
            return carreras;
        }

        // Métodos no implementados de IRepository
        public IEnumerable<Candidata> Buscar(Expression<Func<Candidata, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Candidata ObtenerUno(Expression<Func<Candidata, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public int Contar(Expression<Func<Candidata, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public bool Existe(Expression<Func<Candidata, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        private Candidata MapearCandidata(SqlDataReader reader)
        {
            return new Candidata
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                Cedula = reader.GetString(reader.GetOrdinal("Cedula")),
                FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                Carrera = reader.GetString(reader.GetOrdinal("Carrera")),
                Semestre = reader.GetInt32(reader.GetOrdinal("Semestre")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                PromedioAcademico = reader.GetDecimal(reader.GetOrdinal("PromedioAcademico")),
                Pasatiempos = reader.IsDBNull(reader.GetOrdinal("Pasatiempos")) ? null : reader.GetString(reader.GetOrdinal("Pasatiempos")),
                Habilidades = reader.IsDBNull(reader.GetOrdinal("Habilidades")) ? null : reader.GetString(reader.GetOrdinal("Habilidades")),
                Intereses = reader.IsDBNull(reader.GetOrdinal("Intereses")) ? null : reader.GetString(reader.GetOrdinal("Intereses")),
                AspiracionesFuturo = reader.IsDBNull(reader.GetOrdinal("AspiracionesFuturo")) ? null : reader.GetString(reader.GetOrdinal("AspiracionesFuturo")),
                FotoPrincipalUrl = reader.IsDBNull(reader.GetOrdinal("FotoPrincipalUrl")) ? null : reader.GetString(reader.GetOrdinal("FotoPrincipalUrl")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                RegistradoPor = reader.GetInt32(reader.GetOrdinal("RegistradoPor")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                VotosReina = reader.GetInt32(reader.GetOrdinal("VotosReina")),
                VotosFotogenia = reader.GetInt32(reader.GetOrdinal("VotosFotogenia"))
            };
        }
    }
}
