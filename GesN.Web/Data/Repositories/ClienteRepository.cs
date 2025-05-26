using Dapper;
using GesN.Web.Data;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models;
using System.Data;

namespace GesN.Web.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ClienteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Cliente> GetByIdAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM Cliente WHERE ClienteId = @Id";
            return await connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM Cliente";
            return await connection.QueryAsync<Cliente>(sql);
        }

        public async Task<IEnumerable<Cliente>> BuscarPorNomeOuTelefoneAsync(string termo)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM Cliente 
                WHERE Nome LIKE @Termo 
                    OR TelefonePrincipal LIKE @Termo";

            return await connection.QueryAsync<Cliente>(sql, new { Termo = $"%{termo}%" });
        }

        public async Task AddAsync(Cliente cliente)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                INSERT INTO Cliente (Nome, Sobrenome, Cpf, TelefonePrincipal, DataCadastro, DataModificacao)
                VALUES (@Nome, @Sobrenome, @Cpf, @TelefonePrincipal, @DataCadastro, @DataModificacao)";
            await connection.ExecuteAsync(sql, cliente);
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                UPDATE Cliente 
                SET Nome = @Nome, 
                    Sobrenome = @Sobrenome,
                    Cpf = @Cpf,
                    TelefonePrincipal = @TelefonePrincipal
                WHERE ClienteId = @ClienteId";
            await connection.ExecuteAsync(sql, cliente);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "DELETE FROM Cliente WHERE ClienteId = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
