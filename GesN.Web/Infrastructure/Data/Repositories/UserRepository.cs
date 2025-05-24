using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Interfaces;

namespace GesN.Web.Infrastructure.Data.Repositories
{
    public class UserRepository : BaseRepository<ApplicationUser>
    {
        public UserRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork, "AspNetUsers")
        {
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            const string sql = "SELECT * FROM AspNetUsers WHERE Email = @Email";
            return await QuerySingleAsync<ApplicationUser>(sql, new { Email = email });
        }

        public async Task<ApplicationUser> GetByUserNameAsync(string userName)
        {
            const string sql = "SELECT * FROM AspNetUsers WHERE UserName = @UserName";
            return await QuerySingleAsync<ApplicationUser>(sql, new { UserName = userName });
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            const string sql = @"
                SELECT r.Name
                FROM AspNetRoles r
                INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId";

            return await QueryAsync<string>(sql, new { UserId = userId });
        }

        public async Task<bool> IsInRoleAsync(string userId, string roleName)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM AspNetUserRoles ur
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE ur.UserId = @UserId AND r.Name = @RoleName";

            var count = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
                sql,
                new { UserId = userId, RoleName = roleName },
                _unitOfWork.Transaction
            );

            return count > 0;
        }

        public async Task AddToRoleAsync(string userId, string roleId)
        {
            const string sql = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            await ExecuteAsync(sql, new { UserId = userId, RoleId = roleId });
        }

        public async Task RemoveFromRoleAsync(string userId, string roleId)
        {
            const string sql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
            await ExecuteAsync(sql, new { UserId = userId, RoleId = roleId });
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName)
        {
            const string sql = @"
                SELECT u.*
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE r.Name = @RoleName";

            return await QueryAsync<ApplicationUser>(sql, new { RoleName = roleName });
        }
    }
} 