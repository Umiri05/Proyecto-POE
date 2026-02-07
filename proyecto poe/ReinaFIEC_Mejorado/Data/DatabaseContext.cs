using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ReinaFIEC.Data
{
    /// <summary>
    /// Contexto de base de datos para gestión de conexiones
    /// </summary>
    public class DatabaseContext : IDisposable
    {
        private SqlConnection _connection;
        private readonly string _connectionString;
        private bool _disposed = false;

        public DatabaseContext()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ReinaFIECDB"]?.ConnectionString;
            
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'ReinaFIECDB' en App.config");
            }
        }

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Obtiene la conexión a la base de datos
        /// </summary>
        public SqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }

            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }

            return _connection;
        }

        /// <summary>
        /// Cierra la conexión si está abierta
        /// </summary>
        public void CloseConnection()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Verifica la conexión a la base de datos
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ejecuta un comando SQL sin retorno de datos
        /// </summary>
        public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Ejecuta un comando SQL y retorna un valor escalar
        /// </summary>
        public object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL y retorna un SqlDataReader
        /// </summary>
        public SqlDataReader ExecuteReader(string sql, params SqlParameter[] parameters)
        {
            var conn = GetConnection();
            var cmd = new SqlCommand(sql, conn);
            
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                    {
                        if (_connection.State == System.Data.ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DatabaseContext()
        {
            Dispose(false);
        }
    }
}
