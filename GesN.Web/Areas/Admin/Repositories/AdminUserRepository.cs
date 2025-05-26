using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using System.Data;

namespace GesN.Web.Areas.Admin.Repositories
{
    public class AdminUserRepository
    {
        private readonly IDbConnection _connection;

        public AdminUserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            const string query = "SELECT * FROM AspNetUsers ORDER BY UserName";
            return await _connection.QueryAsync<ApplicationUser>(query);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            const string query = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { Id = userId });
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            const string query = "SELECT * FROM AspNetUsers WHERE Email = @Email";
            return await _connection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { Email = email });
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user)
        {
            const string query = @"
                INSERT INTO AspNetUsers (
                    Id, UserName, NormalizedUserName, Email, NormalizedEmail,
                    EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
                    PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                    LockoutEnd, LockoutEnabled, AccessFailedCount,
                    FirstName, LastName
                ) VALUES (
                    @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                    @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp,
                    @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled,
                    @LockoutEnd, @LockoutEnabled, @AccessFailedCount,
                    @FirstName, @LastName
                )";

            var result = await _connection.ExecuteAsync(query, user);
            return result > 0;
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            const string query = @"
                UPDATE AspNetUsers SET 
                    UserName = @UserName,
                    NormalizedUserName = @NormalizedUserName,
                    Email = @Email,
                    NormalizedEmail = @NormalizedEmail,
                    EmailConfirmed = @EmailConfirmed,
                    PasswordHash = @PasswordHash,
                    SecurityStamp = @SecurityStamp,
                    ConcurrencyStamp = @ConcurrencyStamp,
                    PhoneNumber = @PhoneNumber,
                    PhoneNumberConfirmed = @PhoneNumberConfirmed,
                    TwoFactorEnabled = @TwoFactorEnabled,
                    LockoutEnd = @LockoutEnd,
                    LockoutEnabled = @LockoutEnabled,
                    AccessFailedCount = @AccessFailedCount,
                    FirstName = @FirstName,
                    LastName = @LastName
                WHERE Id = @Id";

            var result = await _connection.ExecuteAsync(query, user);
            return result > 0;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            // Remover relacionamentos primeiro
            await _connection.ExecuteAsync("DELETE FROM AspNetUserRoles WHERE UserId = @Id", new { Id = userId });
            await _connection.ExecuteAsync("DELETE FROM AspNetUserClaims WHERE UserId = @Id", new { Id = userId });
            await _connection.ExecuteAsync("DELETE FROM AspNetUserLogins WHERE UserId = @Id", new { Id = userId });
            await _connection.ExecuteAsync("DELETE FROM AspNetUserTokens WHERE UserId = @Id", new { Id = userId });

            // Remover usuário
            const string query = "DELETE FROM AspNetUsers WHERE Id = @Id";
            var result = await _connection.ExecuteAsync(query, new { Id = userId });
            return result > 0;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            const string query = @"
                SELECT r.Name 
                FROM AspNetRoles r
                INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId";
            
            var roles = await _connection.QueryAsync<string>(query, new { UserId = userId });
            return roles;
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string roleId)
        {
            // Verificar se já existe
            const string checkQuery = "SELECT COUNT(*) FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
            var exists = await _connection.QuerySingleAsync<int>(checkQuery, new { UserId = userId, RoleId = roleId });
            
            if (exists > 0) return true;

            const string insertQuery = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            var result = await _connection.ExecuteAsync(insertQuery, new { UserId = userId, RoleId = roleId });
            return result > 0;
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleId)
        {
            const string query = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
            var result = await _connection.ExecuteAsync(query, new { UserId = userId, RoleId = roleId });
            return result > 0;
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            const string query = "SELECT COUNT(*) FROM AspNetUsers";
            return await _connection.QuerySingleAsync<int>(query);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersWithRoleAsync(string roleName)
        {
            const string query = @"
                SELECT u.* 
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE r.Name = @RoleName
                ORDER BY u.UserName";
            
            return await _connection.QueryAsync<ApplicationUser>(query, new { RoleName = roleName });
        }
    }
} 