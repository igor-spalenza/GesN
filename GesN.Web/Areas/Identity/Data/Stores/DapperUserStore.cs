using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data;
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
                                   IUserRoleStore<ApplicationUser>,
                                   IUserClaimStore<ApplicationUser>,
                                   IUserLoginStore<ApplicationUser>,
                                   IUserAuthenticationTokenStore<ApplicationUser>,
                                   IUserPhoneNumberStore<ApplicationUser>,
                                   IUserTwoFactorStore<ApplicationUser>,
                                   IUserLockoutStore<ApplicationUser>,
                                   IUserAuthenticatorKeyStore<ApplicationUser>,
                                   IUserTwoFactorRecoveryCodeStore<ApplicationUser>
    {
        private readonly IDbConnection _connection;

        public DapperUserStore(ProjectDataContext context)
        {
            _connection = context.Connection;
        }

        #region IUserStore Implementation
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.Id = user.Id ?? Guid.NewGuid().ToString();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.LockoutEnabled = false;
            user.EmailConfirmed = false;
            user.PhoneNumberConfirmed = false;
            user.TwoFactorEnabled = false;
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            user.PhoneNumber = user.PhoneNumber ?? "";
            
            // Inicializa os valores normalizados se estiverem vazios
            user.NormalizedUserName = user.NormalizedUserName ?? user.UserName?.ToUpper();
            user.NormalizedEmail = user.NormalizedEmail ?? user.Email?.ToUpper();

            var parameters = new
            {
                user.Id,
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PasswordHash,
                user.SecurityStamp,
                user.ConcurrencyStamp,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEnd,
                user.LockoutEnabled,
                user.AccessFailedCount
            };

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
                await _connection.ExecuteAsync(query, parameters);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                await _connection.ExecuteAsync("DELETE FROM AspNetUsers WHERE Id = @Id", new { user.Id });
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id",
                new { Id = userId }
            );
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _connection.QuerySingleOrDefaultAsync<ApplicationUser>(
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
            // Garantir que os valores normalizados estejam presentes
            user.NormalizedUserName = user.NormalizedUserName ?? user.UserName?.ToUpper();
            user.NormalizedEmail = user.NormalizedEmail ?? user.Email?.ToUpper();
            
            var parameters = new
            {
                user.Id,
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PasswordHash,
                user.SecurityStamp,
                user.ConcurrencyStamp,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEnd,
                user.LockoutEnabled,
                user.AccessFailedCount
            };
            
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
                await _connection.ExecuteAsync(query, parameters);
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
            return await _connection.QuerySingleOrDefaultAsync<ApplicationUser>(
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
            var normalizedRoleName = roleName.ToUpper();
            var roleId = await _connection.QuerySingleOrDefaultAsync<string>(
                "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                new { NormalizedName = normalizedRoleName }
            );

            if (roleId == null)
                throw new InvalidOperationException($"Role '{roleName}' not found.");

            await _connection.ExecuteAsync(
                "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                new { UserId = user.Id, RoleId = roleId }
            );
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            var normalizedRoleName = roleName.ToUpper();
            await _connection.ExecuteAsync(@"
            DELETE FROM AspNetUserRoles 
            WHERE UserId = @UserId AND RoleId IN 
                (SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName)",
                new { UserId = user.Id, NormalizedName = normalizedRoleName }
            );
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var roles = await _connection.QueryAsync<string>(@"
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
            var normalizedRoleName = roleName.ToUpper();
            var roleExists = await _connection.QuerySingleOrDefaultAsync<bool>(@"
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
            var normalizedRoleName = roleName.ToUpper();
            var users = await _connection.QueryAsync<ApplicationUser>(@"
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

        #region IUserClaimStore Implementation
        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var userClaims = await _connection.QueryAsync<IdentityUserClaim<string>>(
                "SELECT * FROM AspNetUserClaims WHERE UserId = @UserId",
                new { UserId = user.Id }
            );

            return userClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                await _connection.ExecuteAsync(
                    @"INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                    VALUES (@UserId, @ClaimType, @ClaimValue)",
                    new
                    {
                        UserId = user.Id,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    });
            }
        }

        public async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            await _connection.ExecuteAsync(
                @"UPDATE AspNetUserClaims
                SET ClaimType = @NewClaimType, ClaimValue = @NewClaimValue
                WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                new
                {
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value,
                    NewClaimType = newClaim.Type,
                    NewClaimValue = newClaim.Value
                });
        }

        public async Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                await _connection.ExecuteAsync(
                    @"DELETE FROM AspNetUserClaims
                    WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                    new
                    {
                        UserId = user.Id,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    });
            }
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var userIds = await _connection.QueryAsync<string>(
                @"SELECT UserId FROM AspNetUserClaims
                WHERE ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                new { ClaimType = claim.Type, ClaimValue = claim.Value });

            var users = new List<ApplicationUser>();
            foreach (var userId in userIds)
            {
                var user = await FindByIdAsync(userId, cancellationToken);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;
        }
        #endregion

        #region IUserLoginStore Implementation
        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            await _connection.ExecuteAsync(
                @"INSERT INTO AspNetUserLogins (LoginProvider, ProviderKey, ProviderDisplayName, UserId)
                VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId)",
                new
                {
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    ProviderDisplayName = login.ProviderDisplayName,
                    UserId = user.Id
                });
        }

        public async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            await _connection.ExecuteAsync(
                @"DELETE FROM AspNetUserLogins
                WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey",
                new
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey
                });
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var logins = await _connection.QueryAsync<UserLoginInfo>(
                @"SELECT LoginProvider, ProviderKey, ProviderDisplayName
                FROM AspNetUserLogins WHERE UserId = @UserId",
                new { UserId = user.Id });

            return logins.ToList();
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            
            var userId = await _connection.QuerySingleOrDefaultAsync<string>(
                @"SELECT UserId FROM AspNetUserLogins
                WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey",
                new { LoginProvider = loginProvider, ProviderKey = providerKey });

            if (userId == null)
                return null;

            return await FindByIdAsync(userId, cancellationToken);
        }
        #endregion

        #region IUserTokenStore Implementation
        public async Task SetTokenAsync(ApplicationUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            // Primeiro tenta atualizar o token se ele já existir
            var updated = await _connection.ExecuteAsync(
                @"UPDATE AspNetUserTokens
            SET Value = @Value
            WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name",
                new
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value
                });
            //await _connection.ExecuteAsync();

            // Se não existe, insere um novo
            if (updated == 0)
            {
                await _connection.ExecuteAsync(
                    @"INSERT INTO AspNetUserTokens (UserId, LoginProvider, Name, Value)
                    VALUES (@UserId, @LoginProvider, @Name, @Value)",
                    new
                    {
                        UserId = user.Id,
                        LoginProvider = loginProvider,
                        Name = name,
                        Value = value
                    });
            }
        }

        public async Task RemoveTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            await _connection.ExecuteAsync(
                @"DELETE FROM AspNetUserTokens
                WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name",
                new
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name
                });
        }

        public async Task<string> GetTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return await _connection.QuerySingleOrDefaultAsync<string>(
                @"SELECT Value FROM AspNetUserTokens
            WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name",
                new
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name
                });
        }
        #endregion

        #region IUserPhoneNumberStore Implementation
        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserTwoFactorStore Implementation
        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserLockoutStore Implementation
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            // Se LockoutEnd é nulo, retorna nulo
            if (string.IsNullOrEmpty(user.LockoutEnd?.ToString()))
            {
                return Task.FromResult<DateTimeOffset?>(null);
            }
            
            // Tenta converter a string para DateTimeOffset
            if (DateTimeOffset.TryParse(user.LockoutEnd.ToString(), out DateTimeOffset result))
            {
                return Task.FromResult<DateTimeOffset?>(result);
            }
            
            return Task.FromResult<DateTimeOffset?>(null);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            // Armazena o DateTimeOffset como uma string ISO8601
            user.LockoutEnd = lockoutEnd?.ToString("o");
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserAuthenticatorKeyStore Implementation
        public async Task<string> GetAuthenticatorKeyAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return await GetTokenAsync(user, "AuthenticatorKey", "AuthenticatorKey", cancellationToken);
        }

        public async Task SetAuthenticatorKeyAsync(ApplicationUser user, string key, CancellationToken cancellationToken)
        {
            await SetTokenAsync(user, "AuthenticatorKey", "AuthenticatorKey", key, cancellationToken);
        }
        #endregion

        #region IUserTwoFactorRecoveryCodeStore Implementation
        public async Task<int> CountCodesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var recoveryCodes = await GetTokenAsync(user, "RecoveryCodes", "RecoveryCodes", cancellationToken);
            if (string.IsNullOrEmpty(recoveryCodes))
                return 0;

            return recoveryCodes.Split(';').Length;
        }

        public async Task<bool> RedeemCodeAsync(ApplicationUser user, string code, CancellationToken cancellationToken)
        {
            var recoveryCodes = await GetTokenAsync(user, "RecoveryCodes", "RecoveryCodes", cancellationToken);
            if (string.IsNullOrEmpty(recoveryCodes))
                return false;

            var codes = recoveryCodes.Split(';').ToList();
            if (!codes.Contains(code))
                return false;

            codes.Remove(code);
            await SetTokenAsync(user, "RecoveryCodes", "RecoveryCodes", string.Join(";", codes), cancellationToken);
            return true;
        }

        public async Task ReplaceCodesAsync(ApplicationUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            await SetTokenAsync(user, "RecoveryCodes", "RecoveryCodes", string.Join(";", recoveryCodes), cancellationToken);
        }
        #endregion
    }
}
