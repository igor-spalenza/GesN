using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace GesN.Web.Areas.Identity.Data.Stores
{
    public class DapperRoleStore : IRoleStore<ApplicationRole>, IRoleClaimStore<ApplicationRole>, IQueryableRoleStore<ApplicationRole>
    {
        private readonly IDbConnection _dbConnection;

        public DapperRoleStore(ProjectDataContext context)
        {
            _dbConnection = context.Connection;
        }

        public IQueryable<ApplicationRole> Roles
        {
            get
            {
                var roles = _dbConnection.Query<ApplicationRole>("SELECT * FROM AspNetRoles").AsQueryable();
                return roles;
            }
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.Id = role.Id ?? Guid.NewGuid().ToString();
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            role.NormalizedName = role.Name?.ToUpper();

            var query = @"
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

            try
            {
                await _dbConnection.ExecuteAsync(query, role);
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

            try
            {
                await _dbConnection.ExecuteAsync(
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

            return await _dbConnection.QuerySingleOrDefaultAsync<ApplicationRole>(
                "SELECT * FROM AspNetRoles WHERE Id = @Id",
                new { Id = roleId }
            );
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _dbConnection.QuerySingleOrDefaultAsync<ApplicationRole>(
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

            try
            {
                await _dbConnection.ExecuteAsync(@"
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

        #region IRoleClaimStore Implementation

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                var roleClaims = await _dbConnection.QueryAsync<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>(
                    "SELECT * FROM AspNetRoleClaims WHERE RoleId = @RoleId",
                    new { RoleId = role.Id }
                );
                
                return roleClaims.Select(rc => new Claim(rc.ClaimType, rc.ClaimValue)).ToList();
            }
            catch (Exception ex)
            {
                // Log ou trate a exceção conforme necessário
                throw new Exception($"Erro ao recuperar claims da role: {ex.Message}", ex);
            }
        }

        public async Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                var query = @"
                INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                VALUES (@RoleId, @ClaimType, @ClaimValue)";
                
                await _dbConnection.ExecuteAsync(query, new 
                { 
                    RoleId = role.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar claim à role: {ex.Message}", ex);
            }
        }

        public async Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                await _dbConnection.ExecuteAsync(@"
                DELETE FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                new 
                { 
                    RoleId = role.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao remover claim da role: {ex.Message}", ex);
            }
        }

        #endregion

        public void Dispose()
        {
            // Nada a fazer aqui
        }
    }
}
