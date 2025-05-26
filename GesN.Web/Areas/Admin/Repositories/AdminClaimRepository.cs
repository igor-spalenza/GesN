using Dapper;
using System.Data;

namespace GesN.Web.Areas.Admin.Repositories
{
    public class AdminClaimRepository
    {
        private readonly IDbConnection _connection;

        public AdminClaimRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // ===== USER CLAIMS =====

        public async Task<IEnumerable<dynamic>> GetAllUserClaimsAsync()
        {
            const string query = @"
                SELECT uc.*, u.UserName, u.Email
                FROM AspNetUserClaims uc
                INNER JOIN AspNetUsers u ON uc.UserId = u.Id
                ORDER BY u.UserName, uc.ClaimType";
            
            return await _connection.QueryAsync(query);
        }

        public async Task<IEnumerable<dynamic>> GetUserClaimsAsync(string userId)
        {
            const string query = @"
                SELECT * FROM AspNetUserClaims 
                WHERE UserId = @UserId
                ORDER BY ClaimType, ClaimValue";
            
            return await _connection.QueryAsync(query, new { UserId = userId });
        }

        public async Task<dynamic?> GetUserClaimByIdAsync(int claimId)
        {
            const string query = "SELECT * FROM AspNetUserClaims WHERE Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync(query, new { Id = claimId });
        }

        public async Task<bool> AddUserClaimAsync(string userId, string claimType, string claimValue)
        {
            // Verificar se já existe
            const string checkQuery = @"
                SELECT COUNT(*) FROM AspNetUserClaims 
                WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
            var exists = await _connection.QuerySingleAsync<int>(checkQuery, 
                new { UserId = userId, ClaimType = claimType, ClaimValue = claimValue });
            
            if (exists > 0) return true;

            const string insertQuery = @"
                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                VALUES (@UserId, @ClaimType, @ClaimValue)";
            var result = await _connection.ExecuteAsync(insertQuery, 
                new { UserId = userId, ClaimType = claimType, ClaimValue = claimValue });
            
            return result > 0;
        }

        public async Task<bool> UpdateUserClaimAsync(int claimId, string claimType, string claimValue)
        {
            const string query = @"
                UPDATE AspNetUserClaims SET 
                    ClaimType = @ClaimType,
                    ClaimValue = @ClaimValue
                WHERE Id = @Id";

            var result = await _connection.ExecuteAsync(query, 
                new { Id = claimId, ClaimType = claimType, ClaimValue = claimValue });
            
            return result > 0;
        }

        public async Task<bool> RemoveUserClaimAsync(int claimId)
        {
            const string query = "DELETE FROM AspNetUserClaims WHERE Id = @Id";
            var result = await _connection.ExecuteAsync(query, new { Id = claimId });
            return result > 0;
        }

        public async Task<bool> RemoveUserClaimAsync(string userId, string claimType, string claimValue)
        {
            const string query = @"
                DELETE FROM AspNetUserClaims 
                WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
            var result = await _connection.ExecuteAsync(query, 
                new { UserId = userId, ClaimType = claimType, ClaimValue = claimValue });
            return result > 0;
        }

        // ===== ROLE CLAIMS =====

        public async Task<IEnumerable<dynamic>> GetAllRoleClaimsAsync()
        {
            const string query = @"
                SELECT rc.*, r.Name as RoleName
                FROM AspNetRoleClaims rc
                INNER JOIN AspNetRoles r ON rc.RoleId = r.Id
                ORDER BY r.Name, rc.ClaimType";
            
            return await _connection.QueryAsync(query);
        }

        public async Task<IEnumerable<dynamic>> GetRoleClaimsAsync(string roleId)
        {
            const string query = @"
                SELECT * FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId
                ORDER BY ClaimType, ClaimValue";
            
            return await _connection.QueryAsync(query, new { RoleId = roleId });
        }

        public async Task<dynamic?> GetRoleClaimByIdAsync(int claimId)
        {
            const string query = "SELECT * FROM AspNetRoleClaims WHERE Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync(query, new { Id = claimId });
        }

        public async Task<bool> AddRoleClaimAsync(string roleId, string claimType, string claimValue)
        {
            // Verificar se já existe
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

        public async Task<bool> UpdateRoleClaimAsync(int claimId, string claimType, string claimValue)
        {
            const string query = @"
                UPDATE AspNetRoleClaims SET 
                    ClaimType = @ClaimType,
                    ClaimValue = @ClaimValue
                WHERE Id = @Id";

            var result = await _connection.ExecuteAsync(query, 
                new { Id = claimId, ClaimType = claimType, ClaimValue = claimValue });
            
            return result > 0;
        }

        public async Task<bool> RemoveRoleClaimAsync(int claimId)
        {
            const string query = "DELETE FROM AspNetRoleClaims WHERE Id = @Id";
            var result = await _connection.ExecuteAsync(query, new { Id = claimId });
            return result > 0;
        }

        public async Task<bool> RemoveRoleClaimAsync(string roleId, string claimType, string claimValue)
        {
            const string query = @"
                DELETE FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
            var result = await _connection.ExecuteAsync(query, 
                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });
            return result > 0;
        }

        // ===== STATISTICS & UTILITIES =====

        public async Task<int> GetTotalUserClaimsCountAsync()
        {
            const string query = "SELECT COUNT(*) FROM AspNetUserClaims";
            return await _connection.QuerySingleAsync<int>(query);
        }

        public async Task<int> GetTotalRoleClaimsCountAsync()
        {
            const string query = "SELECT COUNT(*) FROM AspNetRoleClaims";
            return await _connection.QuerySingleAsync<int>(query);
        }

        public async Task<int> GetTotalClaimsCountAsync()
        {
            const string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM AspNetUserClaims) + 
                    (SELECT COUNT(*) FROM AspNetRoleClaims) AS TotalClaims";
            return await _connection.QuerySingleAsync<int>(query);
        }

        public async Task<IEnumerable<string>> GetDistinctClaimTypesAsync()
        {
            const string query = @"
                SELECT DISTINCT ClaimType FROM (
                    SELECT ClaimType FROM AspNetUserClaims
                    UNION
                    SELECT ClaimType FROM AspNetRoleClaims
                ) AS AllClaims
                ORDER BY ClaimType";
            
            return await _connection.QueryAsync<string>(query);
        }

        public async Task<IEnumerable<string>> GetDistinctClaimValuesAsync(string claimType)
        {
            const string query = @"
                SELECT DISTINCT ClaimValue FROM (
                    SELECT ClaimValue FROM AspNetUserClaims WHERE ClaimType = @ClaimType
                    UNION
                    SELECT ClaimValue FROM AspNetRoleClaims WHERE ClaimType = @ClaimType
                ) AS AllValues
                ORDER BY ClaimValue";
            
            return await _connection.QueryAsync<string>(query, new { ClaimType = claimType });
        }

        public async Task<bool> ClaimExistsAsync(string claimType, string claimValue)
        {
            const string query = @"
                SELECT COUNT(*) FROM (
                    SELECT 1 FROM AspNetUserClaims WHERE ClaimType = @ClaimType AND ClaimValue = @ClaimValue
                    UNION
                    SELECT 1 FROM AspNetRoleClaims WHERE ClaimType = @ClaimType AND ClaimValue = @ClaimValue
                ) AS ExistingClaims";
            
            var count = await _connection.QuerySingleAsync<int>(query, 
                new { ClaimType = claimType, ClaimValue = claimValue });
            return count > 0;
        }
    }
} 