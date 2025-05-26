using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using System.Data;

namespace GesN.Web.Areas.Admin.Repositories
{
    public class AdminRoleRepository
    {
        private readonly IDbConnection _connection;

        public AdminRoleRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync()
        {
            const string query = "SELECT * FROM AspNetRoles ORDER BY Name";
            return await _connection.QueryAsync<ApplicationRole>(query);
        }

        public async Task<ApplicationRole?> GetRoleByIdAsync(string roleId)
        {
            const string query = "SELECT * FROM AspNetRoles WHERE Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync<ApplicationRole>(query, new { Id = roleId });
        }

        public async Task<ApplicationRole?> GetRoleByNameAsync(string roleName)
        {
            const string query = "SELECT * FROM AspNetRoles WHERE Name = @Name";
            return await _connection.QuerySingleOrDefaultAsync<ApplicationRole>(query, new { Name = roleName });
        }

        public async Task<bool> CreateRoleAsync(ApplicationRole role)
        {
            const string query = @"
                INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

            var result = await _connection.ExecuteAsync(query, role);
            return result > 0;
        }

        public async Task<bool> UpdateRoleAsync(ApplicationRole role)
        {
            const string query = @"
                UPDATE AspNetRoles SET 
                    Name = @Name,
                    NormalizedName = @NormalizedName,
                    ConcurrencyStamp = @ConcurrencyStamp
                WHERE Id = @Id";

            var result = await _connection.ExecuteAsync(query, role);
            return result > 0;
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            // Remover relacionamentos primeiro
            await _connection.ExecuteAsync("DELETE FROM AspNetUserRoles WHERE RoleId = @Id", new { Id = roleId });
            await _connection.ExecuteAsync("DELETE FROM AspNetRoleClaims WHERE RoleId = @Id", new { Id = roleId });

            // Remover role
            const string query = "DELETE FROM AspNetRoles WHERE Id = @Id";
            var result = await _connection.ExecuteAsync(query, new { Id = roleId });
            return result > 0;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleId)
        {
            const string query = @"
                SELECT u.* 
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                WHERE ur.RoleId = @RoleId
                ORDER BY u.UserName";
            
            return await _connection.QueryAsync<ApplicationUser>(query, new { RoleId = roleId });
        }

        public async Task<int> GetUsersCountInRoleAsync(string roleId)
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM AspNetUserRoles 
                WHERE RoleId = @RoleId";
            
            return await _connection.QuerySingleAsync<int>(query, new { RoleId = roleId });
        }

        public async Task<int> GetTotalRolesCountAsync()
        {
            const string query = "SELECT COUNT(*) FROM AspNetRoles";
            return await _connection.QuerySingleAsync<int>(query);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            const string query = "SELECT COUNT(*) FROM AspNetRoles WHERE Name = @Name";
            var count = await _connection.QuerySingleAsync<int>(query, new { Name = roleName });
            return count > 0;
        }

        //public async Task<IEnumerable<ApplicationRoleClaim>> GetRoleClaimsAsync(string roleId)
        //{
        //    const string query = @"
        //        SELECT * FROM AspNetRoleClaims 
        //        WHERE RoleId = @RoleId
        //        ORDER BY ClaimType, ClaimValue";
            
        //    return await _connection.QueryAsync<ApplicationRoleClaim>(query, new { RoleId = roleId });
        //}

        public async Task<bool> AddClaimToRoleAsync(string roleId, string claimType, string claimValue)
        {
            const string checkQuery = @"
                SELECT COUNT(*) FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
            var exists = await _connection.QuerySingleAsync<int>(checkQuery, 
                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });
            
            if (exists > 0) return true;

            const string insertQuery = @"
                INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                VALUES (@RoleId, @ClaimType, @ClaimValue)";
            var result = await _connection.ExecuteAsync(insertQuery, 
                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });
            
            return result > 0;
        }

        public async Task<bool> RemoveClaimFromRoleAsync(string roleId, string claimType, string claimValue)
        {
            const string query = @"
                DELETE FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
            var result = await _connection.ExecuteAsync(query, 
                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });
            return result > 0;
        }
    }
} 