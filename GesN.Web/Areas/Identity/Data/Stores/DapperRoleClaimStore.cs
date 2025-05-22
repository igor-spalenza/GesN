using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace GesN.Web.Areas.Identity.Data.Stores
{
    public class DapperRoleClaimStore : IRoleClaimStore<ApplicationRole>, IQueryableRoleStore<ApplicationRole>
    {
        private readonly IDbConnection _dbConnection;

        public DapperRoleClaimStore(ProjectDataContext context)
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

        #region IRoleStore Implementation
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

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            role.NormalizedName = role.Name?.ToUpper();

            try
            {
                await _dbConnection.ExecuteAsync(
                    @"UPDATE AspNetRoles 
                    SET Name = @Name, NormalizedName = @NormalizedName, ConcurrencyStamp = @ConcurrencyStamp
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

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // Primeiro remove as claims da role
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetRoleClaims WHERE RoleId = @Id",
                    new { role.Id }
                );

                // Depois remove a role
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

        public Task<string?> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string?> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(ApplicationRole role, string? roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }
        #endregion

        #region IRoleClaimStore Implementation
        public async Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var claims = await _dbConnection.QueryAsync<ApplicationRoleClaim<string>>(
                "SELECT * FROM AspNetRoleClaims WHERE RoleId = @RoleId",
                new { RoleId = role.Id }
            );

            return claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public async Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Verificar se a claim já existe para evitar duplicação
            var existingClaim = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRoleClaim<string>>(
                "SELECT * FROM AspNetRoleClaims WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                new { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value }
            );

            if (existingClaim != null)
            {
                return; // A claim já existe
            }

            await _dbConnection.ExecuteAsync(
                @"INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                VALUES (@RoleId, @ClaimType, @ClaimValue)",
                new 
                { 
                    RoleId = role.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                }
            );
        }

        public async Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await _dbConnection.ExecuteAsync(
                @"DELETE FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                new 
                { 
                    RoleId = role.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                }
            );
        }
        #endregion

        #region Métodos Estendidos para Facilitar o Uso
        /// <summary>
        /// Obtém todas as claims de uma role pelo nome da role
        /// </summary>
        public async Task<IEnumerable<Claim>> GetClaimsByRoleNameAsync(string roleName)
        {
            var query = @"
                SELECT rc.ClaimType, rc.ClaimValue
                FROM AspNetRoleClaims rc
                INNER JOIN AspNetRoles r ON rc.RoleId = r.Id
                WHERE r.Name = @RoleName
            ";

            var claims = await _dbConnection.QueryAsync<ApplicationRoleClaim<string>>(query, new { RoleName = roleName });
            return claims.Select(c => new Claim(c.ClaimType, c.ClaimValue));
        }

        /// <summary>
        /// Verifica se uma role possui uma determinada claim
        /// </summary>
        public async Task<bool> RoleHasClaimAsync(string roleId, string claimType, string claimValue)
        {
            var query = @"
                SELECT COUNT(1) FROM AspNetRoleClaims
                WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue
            ";

            var count = await _dbConnection.ExecuteScalarAsync<int>(query, 
                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });

            return count > 0;
        }

        /// <summary>
        /// Remove todas as claims de uma role
        /// </summary>
        public async Task<bool> RemoveAllClaimsAsync(string roleId)
        {
            var query = @"
                DELETE FROM AspNetRoleClaims
                WHERE RoleId = @RoleId
            ";

            await _dbConnection.ExecuteAsync(query, new { RoleId = roleId });
            return true;
        }

        /// <summary>
        /// Define várias claims para uma role em uma única operação (remove as existentes e adiciona as novas)
        /// </summary>
        public async Task<bool> SetRoleClaimsAsync(string roleId, Dictionary<string, List<string>> claimsByType)
        {
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Remove todas as claims existentes
                    await _dbConnection.ExecuteAsync(
                        "DELETE FROM AspNetRoleClaims WHERE RoleId = @RoleId",
                        new { RoleId = roleId },
                        transaction
                    );

                    // Adiciona as novas claims
                    foreach (var claimType in claimsByType.Keys)
                    {
                        foreach (var claimValue in claimsByType[claimType])
                        {
                            await _dbConnection.ExecuteAsync(
                                "INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, @ClaimType, @ClaimValue)",
                                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue },
                                transaction
                            );
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Obtém todas as claims agrupadas por tipo para uma role
        /// </summary>
        public async Task<Dictionary<string, List<string>>> GetClaimsByTypeForRoleAsync(string roleId)
        {
            var query = @"
                SELECT * FROM AspNetRoleClaims
                WHERE RoleId = @RoleId
            ";
            
            var claims = await _dbConnection.QueryAsync<ApplicationRoleClaim<string>>(query, new { RoleId = roleId });
            var result = new Dictionary<string, List<string>>();

            foreach (var claim in claims)
            {
                if (!result.ContainsKey(claim.ClaimType))
                {
                    result[claim.ClaimType] = new List<string>();
                }

                result[claim.ClaimType].Add(claim.ClaimValue);
            }

            return result;
        }

        /// <summary>
        /// Obtém todas as claims de uma role como objetos Claim
        /// </summary>
        /// <param name="roleId">ID da role</param>
        /// <returns>Lista de claims</returns>
        public async Task<List<Claim>> GetClaimsForRoleAsync(string roleId)
        {
            var query = @"
                SELECT * FROM AspNetRoleClaims
                WHERE RoleId = @RoleId
            ";

            var roleClaims = await _dbConnection.QueryAsync<ApplicationRoleClaim<string>>(query, new { RoleId = roleId });
            return roleClaims.Select(rc => new Claim(rc.ClaimType, rc.ClaimValue)).ToList();
        }
        #endregion

        public void Dispose()
        {
            // Nada a fazer aqui
        }
    }
} 