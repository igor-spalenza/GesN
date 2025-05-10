using Dapper;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models;
using System.Data;

namespace GesN.Web.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbConnection _dbConnection;

        public ClienteRepository(ProjectDataContext context)
        {
            _dbConnection = context.Connection;
        }

        public async Task<Cliente> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Cliente WHERE ClienteId = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            var sql = "SELECT * FROM Cliente";
            return await _dbConnection.QueryAsync<Cliente>(sql);
        }

        public async Task AddAsync(Cliente cliente)
        {
            var sql = @"
                INSERT INTO Cliente (Nome, Sobrenome, Cpf, TelefonePrincipal, DataCadastro, DataModificacao)
                VALUES (@Nome, @Sobrenome, @Cpf, @TelefonePrincipal, @DataCadastro, @DataModificacao)";
            await _dbConnection.ExecuteAsync(sql, cliente);
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            var sql = @"
                UPDATE Cliente 
                SET Nome = @Nome, 
                    Sobrenome = @Sobrenome,
                    Cpf = @Cpf,
                    TelefonePrincipal = @TelefonePrincipal
                WHERE ClienteId = @ClienteId";
            await _dbConnection.ExecuteAsync(sql, cliente);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Cliente WHERE ClienteId = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
