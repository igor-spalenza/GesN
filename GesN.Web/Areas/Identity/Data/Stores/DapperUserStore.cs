using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GesN.Web.Areas.Identity.Data.Stores
{
    public class DapperUserStore : IUserStore<ApplicationUser>,
                                   IUserEmailStore<ApplicationUser>,
                                   IUserPasswordStore<ApplicationUser>,
                                   IUserSecurityStampStore<ApplicationUser>,
                                   IUserRoleStore<ApplicationUser>
                                   /*IUserTwoFactorStore<ApplicationUser>,
                                   IUserLockoutStore<ApplicationUser>,
                                   IUserPhoneNumberStore<ApplicationUser>
        UserStoreBase<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>,*/
    {
        private readonly string _connectionString;

        public DapperUserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region IUserStore Implementation
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            user.Id = user.Id ?? Guid.NewGuid().ToString();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            user.SecurityStamp = Guid.NewGuid().ToString();

            var query = @"
            INSERT INTO AspNetUsers (
                Id, UserName, NormalizedUserName, Email, NormalizedEmail,
                EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
                PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                LockoutEnd, LockoutEnabled, AccessFailedCount
            ) VALUES (
                @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp,
                @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled,
                @LockoutEnd, @LockoutEnabled, @AccessFailedCount
            )";

            try
            {
                await connection.ExecuteAsync(query, user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            try
            {
                await connection.ExecuteAsync("DELETE FROM AspNetUsers WHERE Id = @Id", new { user.Id });
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id",
                new { Id = userId }
            );
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName",
                new { NormalizedUserName = normalizedUserName }
            );
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var query = @"
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
                AccessFailedCount = @AccessFailedCount
            WHERE Id = @Id";

            try
            {
                await connection.ExecuteAsync(query, user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        #endregion

        #region IUserEmailStore Implementation
        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail",
                new { NormalizedEmail = normalizedEmail }
            );
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserPasswordStore Implementation
        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserSecurityStampStore Implementation
        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserRoleStore Implementation
        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var normalizedRoleName = roleName.ToUpper();
            var roleId = await connection.QuerySingleOrDefaultAsync<string>(
                "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                new { NormalizedName = normalizedRoleName }
            );

            if (roleId == null)
                throw new InvalidOperationException($"Role '{roleName}' not found.");

            await connection.ExecuteAsync(
                "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                new { UserId = user.Id, RoleId = roleId }
            );
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var normalizedRoleName = roleName.ToUpper();
            await connection.ExecuteAsync(@"
            DELETE FROM AspNetUserRoles 
            WHERE UserId = @UserId AND RoleId IN 
                (SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName)",
                new { UserId = user.Id, NormalizedName = normalizedRoleName }
            );
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var roles = await connection.QueryAsync<string>(@"
            SELECT r.Name 
            FROM AspNetRoles r 
            INNER JOIN AspNetUserRoles ur ON ur.RoleId = r.Id 
            WHERE ur.UserId = @UserId",
                new { UserId = user.Id }
            );

            return roles.ToList();
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var normalizedRoleName = roleName.ToUpper();
            var roleExists = await connection.QuerySingleOrDefaultAsync<bool>(@"
            SELECT COUNT(1) 
            FROM AspNetUserRoles ur 
            INNER JOIN AspNetRoles r ON r.Id = ur.RoleId 
            WHERE ur.UserId = @UserId AND r.NormalizedName = @NormalizedName",
                new { UserId = user.Id, NormalizedName = normalizedRoleName }
            );

            return roleExists;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var normalizedRoleName = roleName.ToUpper();
            var users = await connection.QueryAsync<ApplicationUser>(@"
            SELECT u.* 
            FROM AspNetUsers u 
            INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id 
            INNER JOIN AspNetRoles r ON r.Id = ur.RoleId 
            WHERE r.NormalizedName = @NormalizedName",
                new { NormalizedName = normalizedRoleName }
            );

            return users.ToList();
        }
        #endregion

        public void Dispose()
        {
            // Nothing to dispose
        }


    }
}
