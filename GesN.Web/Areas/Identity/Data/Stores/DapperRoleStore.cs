using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;

namespace GesN.Web.Areas.Identity.Data.Stores
{
    public class DapperRoleStore : IRoleStore<ApplicationRole>
    {
        private readonly string _connectionString;

        public DapperRoleStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            role.Id = role.Id ?? Guid.NewGuid().ToString();
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            role.NormalizedName = role.Name?.ToUpper();

            var query = @"
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

            try
            {
                await connection.ExecuteAsync(query, role);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            try
            {
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetRoles WHERE Id = @Id",
                    new { role.Id }
                );
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<ApplicationRole>(
                "SELECT * FROM AspNetRoles WHERE Id = @Id",
                new { Id = roleId }
            );
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<ApplicationRole>(
                "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                new { NormalizedName = normalizedRoleName }
            );
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            try
            {
                await connection.ExecuteAsync(@"
                UPDATE AspNetRoles 
                SET Name = @Name,
                    NormalizedName = @NormalizedName,
                    ConcurrencyStamp = @ConcurrencyStamp
                WHERE Id = @Id",
                    role
                );
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public void Dispose()
        {
            // Nada a fazer aqui
        }
    }
}
