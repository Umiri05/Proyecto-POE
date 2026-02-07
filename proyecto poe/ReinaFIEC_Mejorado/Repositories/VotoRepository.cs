using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ReinaFIEC.Models;
using ReinaFIEC.Data;

namespace ReinaFIEC.Repositories
{
    public class VotoRepository
    {
        private readonly DatabaseContext _context;

        public VotoRepository(DatabaseContext context)
        {
            _context = context;
        }

        public void RegistrarVoto(Voto voto)
        {
            string sql = @"INSERT INTO Votos (UsuarioId, CandidataId, TipoVoto, FechaVoto, IpAddress, UserAgent)
                          VALUES (@UsuarioId, @CandidataId, @TipoVoto, @FechaVoto, @IpAddress, @UserAgent)";

            var parameters = new[]
            {
                new SqlParameter("@UsuarioId", voto.UsuarioId),
                new SqlParameter("@CandidataId", voto.CandidataId),
                new SqlParameter("@TipoVoto", (int)voto.TipoVoto),
                new SqlParameter("@FechaVoto", voto.FechaVoto),
                new SqlParameter("@IpAddress", (object)voto.IpAddress ?? DBNull.Value),
                new SqlParameter("@UserAgent", (object)voto.UserAgent ?? DBNull.Value)
            };

            _context.ExecuteNonQuery(sql, parameters);
        }

        public bool UsuarioYaVoto(int usuarioId, TipoVoto tipoVoto)
        {
            string sql = "SELECT COUNT(*) FROM Votos WHERE UsuarioId = @UsuarioId AND TipoVoto = @TipoVoto";
            
            var parameters = new[]
            {
                new SqlParameter("@UsuarioId", usuarioId),
                new SqlParameter("@TipoVoto", (int)tipoVoto)
            };

            return Convert.ToInt32(_context.ExecuteScalar(sql, parameters)) > 0;
        }

        public IEnumerable<ResultadoVotacion> ObtenerResultados(TipoVoto tipoVoto)
        {
            var resultados = new List<ResultadoVotacion>();
            
            string sql = @"WITH VotacionCTE AS (
                              SELECT 
                                  c.Id as CandidataId,
                                  c.NombreCompleto,
                                  c.Carrera,
                                  c.FotoPrincipalUrl,
                                  COUNT(v.Id) as TotalVotos,
                                  RANK() OVER (ORDER BY COUNT(v.Id) DESC) as Posicion
                              FROM Candidatas c
                              LEFT JOIN Votos v ON c.Id = v.CandidataId AND v.TipoVoto = @TipoVoto
                              WHERE c.Activo = 1
                              GROUP BY c.Id, c.NombreCompleto, c.Carrera, c.FotoPrincipalUrl
                          )
                          SELECT *, 
                                 CASE WHEN TotalVotos > 0 THEN 
                                      CAST(TotalVotos AS DECIMAL(10,2)) / 
                                      (SELECT SUM(TotalVotos) FROM VotacionCTE) * 100 
                                 ELSE 0 END as PorcentajeVotos,
                                 CASE WHEN Posicion = 1 THEN 1 ELSE 0 END as EsGanadora
                          FROM VotacionCTE
                          ORDER BY TotalVotos DESC";

            using (var reader = _context.ExecuteReader(sql, new SqlParameter("@TipoVoto", (int)tipoVoto)))
            {
                while (reader.Read())
                {
                    resultados.Add(new ResultadoVotacion
                    {
                        CandidataId = reader.GetInt32(0),
                        NombreCompleto = reader.GetString(1),
                        Carrera = reader.GetString(2),
                        FotoPrincipalUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                        TotalVotos = reader.GetInt32(4),
                        Posicion = reader.GetInt32(5),
                        PorcentajeVotos = reader.GetDecimal(6),
                        EsGanadora = reader.GetInt32(7) == 1
                    });
                }
            }
            return resultados;
        }

        public int ObtenerTotalVotosPorTipo(TipoVoto tipoVoto)
        {
            string sql = "SELECT COUNT(*) FROM Votos WHERE TipoVoto = @TipoVoto";
            return Convert.ToInt32(_context.ExecuteScalar(sql, new SqlParameter("@TipoVoto", (int)tipoVoto)));
        }

        public void RegistrarAuditoria(AuditoriaVoto auditoria)
        {
            string sql = @"INSERT INTO AuditoriaVotos 
                          (VotoId, Accion, UsuarioId, CandidataId, TipoVoto, FechaAccion, DetallesJson, IpAddress)
                          VALUES (@VotoId, @Accion, @UsuarioId, @CandidataId, @TipoVoto, @FechaAccion, @DetallesJson, @IpAddress)";

            var parameters = new[]
            {
                new SqlParameter("@VotoId", (object)auditoria.VotoId ?? DBNull.Value),
                new SqlParameter("@Accion", auditoria.Accion),
                new SqlParameter("@UsuarioId", auditoria.UsuarioId),
                new SqlParameter("@CandidataId", auditoria.CandidataId),
                new SqlParameter("@TipoVoto", (int)auditoria.TipoVoto),
                new SqlParameter("@FechaAccion", auditoria.FechaAccion),
                new SqlParameter("@DetallesJson", (object)auditoria.DetallesJson ?? DBNull.Value),
                new SqlParameter("@IpAddress", (object)auditoria.IpAddress ?? DBNull.Value)
            };

            _context.ExecuteNonQuery(sql, parameters);
        }
    }
}
