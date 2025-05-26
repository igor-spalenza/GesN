using Dapper;
using GesN.Web.Data;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models;
using Microsoft.Data.Sqlite;
using System.Data;

namespace GesN.Web.Data.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PedidoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Pedido> GetByIdAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT p.*, c.Nome AS NomeCliente
                FROM Pedido p
                JOIN Cliente c ON p.ClienteId = c.ClienteId 
                WHERE p.PedidoId = @Id";
            return await connection.QueryFirstOrDefaultAsync<Pedido>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM Pedido";
            return await connection.QueryAsync<Pedido>(sql);
        }

        public async Task<int> AddAsync(Pedido pedido)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                INSERT INTO Pedido (ClienteId, DataPedido, DataCadastro, DataModificacao)
                VALUES (@ClienteId, @DataPedido, @DataCadastro, @DataModificacao);
                SELECT last_insert_rowid();";
            return await connection.QuerySingleAsync<int>(sql, pedido);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(Pedido pedido)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var sql = @"
                    UPDATE Pedido 
                    SET ClienteId = @ClienteId, 
                        DataPedido = @DataPedido,
                        DataModificacao = @DataModificacao
                    WHERE PedidoId = @PedidoId";
                
                int rowsAffected = await connection.ExecuteAsync(sql, pedido);
                
                if (rowsAffected > 0)
                {
                    return (true, null);
                }
                else
                {
                    return (false, "Nenhum registro foi atualizado. Verifique se o pedido existe.");
                }
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19 && ex.Message.Contains("FOREIGN KEY constraint failed"))
            {
                // Erro específico de FK constraint
                return (false, "Erro de restrição de chave estrangeira. Verifique se o Cliente existe no sistema.");
            }
            catch (SqliteException ex)
            {
                // Outros erros do SQLite
                return (false, $"Erro do banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Qualquer outro erro
                return (false, $"Erro inesperado: {ex.Message}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "DELETE FROM Pedido WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
