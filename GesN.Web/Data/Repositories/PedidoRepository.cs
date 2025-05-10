using Dapper;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models;
using System.Data;

namespace GesN.Web.Data.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly IDbConnection _dbConnection;

        public PedidoRepository(ProjectDataContext context)
        {
            _dbConnection = context.Connection;
        }

        public async Task<Pedido> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Pedido WHERE PedidoId = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Pedido>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            var sql = "SELECT * FROM Pedido";
            return await _dbConnection.QueryAsync<Pedido>(sql);
        }

        public async Task<int> AddAsync(Pedido pedido)
        {
            var sql = @"
                INSERT INTO Pedido (ClienteId, ColaboradorId, DataPedido, DataCadastro, DataModificacao)
                VALUES (@ClienteId, @ColaboradorId, @DataPedido, @DataCadastro, @DataModificacao);
                SELECT last_insert_rowid();";
            return await _dbConnection.QuerySingleAsync<int>(sql, pedido);
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            var sql = @"
                UPDATE Pedido 
                SET PedidoId = @PedidoId, 
                    Status = @Status 
                WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, pedido);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Pedido WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
