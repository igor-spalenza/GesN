using Dapper;
using GesN.Web.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using System.Data;

namespace GesN.Web.Data
{
    public class ProjectDataContext : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private static readonly object _lockObject = new object();

        public ProjectDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Cria uma nova conexão SQLite aberta
        /// </summary>
        public IDbConnection CreateConnection()
        {
            lock (_lockObject)
            {
                var connection = new SqliteConnection(_connectionString);
                connection.Open();
                ConfigureSqliteConnection(connection);
                return connection;
            }
        }

        /// <summary>
        /// Cria uma nova conexão SQLite aberta de forma assíncrona
        /// </summary>
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            // Para SQLite, vamos usar a versão síncrona dentro de um lock
            // pois SQLite não se beneficia muito de async para conexões locais
            return await Task.Run(() => CreateConnection());
        }

        /// <summary>
        /// Configura a conexão SQLite com configuração mínima
        /// </summary>
        private void ConfigureSqliteConnection(IDbConnection connection)
        {
            try
            {
                // APENAS o timeout - sem outras configurações que deixam lento
                connection.Execute("PRAGMA busy_timeout = 5000");
            }
            catch (Exception ex)
            {
                // Log do erro mas não falha a conexão
                System.Diagnostics.Debug.WriteLine($"Aviso: Erro ao configurar SQLite PRAGMA: {ex.Message}");
            }
        }

        /// <summary>
        /// Propriedade de conveniência para compatibilidade com código existente
        /// Cria uma nova conexão a cada acesso
        /// </summary>
        public IDbConnection Connection => CreateConnection();
    }
}
