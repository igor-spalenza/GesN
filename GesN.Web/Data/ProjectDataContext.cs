using Dapper;
using GesN.Web.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using System.Data;

namespace GesN.Web.Data
{
    public class ProjectDataContext : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public ProjectDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Cria uma nova conexão SQLite aberta
        /// </summary>
        public IDbConnection CreateConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            // ✅ TESTE: Remover PRAGMA para verificar se é a causa da lentidão
            // ConfigureSqliteConnection(connection);
            
            return connection;
        }

        /// <summary>
        /// Cria uma nova conexão SQLite aberta de forma assíncrona
        /// </summary>
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            // ✅ TESTE: Remover PRAGMA para verificar se é a causa da lentidão
            // await ConfigureSqliteConnectionAsync(connection);
            
            return connection;
        }

        /// <summary>
        /// Configura a conexão SQLite para melhor performance e concorrência
        /// ✅ OTIMIZADO: Apenas configurações essenciais para reduzir overhead
        /// </summary>
        private void ConfigureSqliteConnection(IDbConnection connection)
        {
            try
            {
                // ✅ Apenas configurações críticas em uma única execução
                connection.Execute(@"
                    PRAGMA journal_mode=WAL;
                    PRAGMA busy_timeout=30000;
                    PRAGMA synchronous=NORMAL;");
            }
            catch (Exception ex)
            {
                // Log do erro mas não falha a conexão
                Console.WriteLine($"Aviso: Não foi possível configurar PRAGMA do SQLite: {ex.Message}");
            }
        }

        /// <summary>
        /// Configura a conexão SQLite de forma assíncrona
        /// ✅ OTIMIZADO: Apenas configurações essenciais para reduzir overhead
        /// </summary>
        private async Task ConfigureSqliteConnectionAsync(IDbConnection connection)
        {
            try
            {
                // ✅ Apenas configurações críticas em uma única execução
                await connection.ExecuteAsync(@"
                    PRAGMA journal_mode=WAL;
                    PRAGMA busy_timeout=30000;
                    PRAGMA synchronous=NORMAL;");
            }
            catch (Exception ex)
            {
                // Log do erro mas não falha a conexão
                Console.WriteLine($"Aviso: Não foi possível configurar PRAGMA do SQLite: {ex.Message}");
            }
        }

        /// <summary>
        /// Propriedade de conveniência para compatibilidade com código existente
        /// Cria uma nova conexão a cada acesso
        /// </summary>
        public IDbConnection Connection => CreateConnection();
    }
}
