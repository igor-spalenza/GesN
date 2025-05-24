using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq;

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
                                   IQueryableUserStore<ApplicationUser>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DapperUserStore(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region IUserStore Implementation
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.Id = user.Id ?? Guid.NewGuid().ToString();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.LockoutEnabled = true; // Habilitando por padrão para segurança
            user.EmailConfirmed = false;
            user.PhoneNumberConfirmed = false;
            user.TwoFactorEnabled = false;
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            user.PhoneNumber = user.PhoneNumber ?? "";
            
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
                user.AccessFailedCount,
                user.FirstName,
                user.LastName
            };

            try
            {
                _unitOfWork.BeginTransaction();

                var query = @"
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

                await _unitOfWork.Connection.ExecuteAsync(query, parameters, _unitOfWork.Transaction);
                _unitOfWork.Commit();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // Remove todas as relações primeiro
                await _unitOfWork.Connection.ExecuteAsync(
                    "DELETE FROM AspNetUserRoles WHERE UserId = @Id",
                    new { user.Id },
                    _unitOfWork.Transaction);

                await _unitOfWork.Connection.ExecuteAsync(
                    "DELETE FROM AspNetUserClaims WHERE UserId = @Id",
                    new { user.Id },
                    _unitOfWork.Transaction);

                await _unitOfWork.Connection.ExecuteAsync(
                    "DELETE FROM AspNetUserLogins WHERE UserId = @Id",
                    new { user.Id },
                    _unitOfWork.Transaction);

                await _unitOfWork.Connection.ExecuteAsync(
                    "DELETE FROM AspNetUserTokens WHERE UserId = @Id",
                    new { user.Id },
                    _unitOfWork.Transaction);

                // Remove o usuário
                await _unitOfWork.Connection.ExecuteAsync(
                    "DELETE FROM AspNetUsers WHERE Id = @Id",
                    new { user.Id },
                    _unitOfWork.Transaction);

                _unitOfWork.Commit();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id",
                new { Id = userId },
                _unitOfWork.Transaction);
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName",
                new { NormalizedUserName = normalizedUserName },
                _unitOfWork.Transaction);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                _unitOfWork.BeginTransaction();

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
                    user.AccessFailedCount,
                    user.FirstName,
                    user.LastName
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
                    AccessFailedCount = @AccessFailedCount,
                    FirstName = @FirstName,
                    LastName = @LastName
                WHERE Id = @Id";

                await _unitOfWork.Connection.ExecuteAsync(query, parameters, _unitOfWork.Transaction);
                _unitOfWork.Commit();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
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
        #endregion

        #region IUserEmailStore Implementation
        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail",
                new { NormalizedEmail = normalizedEmail },
                _unitOfWork.Transaction);
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
            var roleId = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<string>(
                "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                new { NormalizedName = normalizedRoleName },
                _unitOfWork.Transaction);

            if (roleId == null)
                throw new InvalidOperationException($"Role '{roleName}' not found.");

            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                new { UserId = user.Id, RoleId = roleId },
                _unitOfWork.Transaction);
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            var normalizedRoleName = roleName.ToUpper();
            await _unitOfWork.Connection.ExecuteAsync(@"
            DELETE FROM AspNetUserRoles 
            WHERE UserId = @UserId AND RoleId IN 
                (SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName)",
                new { UserId = user.Id, NormalizedName = normalizedRoleName },
                _unitOfWork.Transaction);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var roles = await _unitOfWork.Connection.QueryAsync<string>(@"
            SELECT r.Name 
            FROM AspNetRoles r 
            INNER JOIN AspNetUserRoles ur ON ur.RoleId = r.Id 
            WHERE ur.UserId = @UserId",
                new { UserId = user.Id },
                _unitOfWork.Transaction);

            return roles.ToList();
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            var normalizedRoleName = roleName.ToUpper();
            var roleExists = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<bool>(@"
            SELECT COUNT(1) 
            FROM AspNetUserRoles ur 
            INNER JOIN AspNetRoles r ON r.Id = ur.RoleId 
            WHERE ur.UserId = @UserId AND r.NormalizedName = @NormalizedName",
                new { UserId = user.Id, NormalizedName = normalizedRoleName },
                _unitOfWork.Transaction);

            return roleExists;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var normalizedRoleName = roleName.ToUpper();
            var users = await _unitOfWork.Connection.QueryAsync<ApplicationUser>(@"
            SELECT u.* 
            FROM AspNetUsers u 
            INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id 
            INNER JOIN AspNetRoles r ON r.Id = ur.RoleId 
            WHERE r.NormalizedName = @NormalizedName",
                new { NormalizedName = normalizedRoleName },
                _unitOfWork.Transaction);

            return users.ToList();
        }
        #endregion

        public void Dispose()
        {
            // UnitOfWork será disposto pelo container DI
        }

        #region IUserClaimStore Implementation
        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var userClaims = await _unitOfWork.Connection.QueryAsync<IdentityUserClaim<string>>(
                "SELECT * FROM AspNetUserClaims WHERE UserId = @UserId",
                new { UserId = user.Id },
                _unitOfWork.Transaction);

            return userClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    @"INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                    VALUES (@UserId, @ClaimType, @ClaimValue)",
                    new
                    {
                        UserId = user.Id,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    },
                    _unitOfWork.Transaction);
            }
        }

        public async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            await _unitOfWork.Connection.ExecuteAsync(
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
                },
                _unitOfWork.Transaction);
        }

        public async Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    @"DELETE FROM AspNetUserClaims
                    WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                    new
                    {
                        UserId = user.Id,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    },
                    _unitOfWork.Transaction);
            }
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var userIds = await _unitOfWork.Connection.QueryAsync<string>(
                @"SELECT UserId FROM AspNetUserClaims
                WHERE ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                new { ClaimType = claim.Type, ClaimValue = claim.Value },
                _unitOfWork.Transaction);

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

        public async Task<bool> HasClaimAsync(ApplicationUser user, string claimType, string claimValue)
        {
            var claim = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<IdentityUserClaim<string>>(
                @"SELECT * FROM AspNetUserClaims 
                WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                new { UserId = user.Id, ClaimType = claimType, ClaimValue = claimValue },
                _unitOfWork.Transaction);
            
            return claim != null;
        }

        public async Task<(bool tableExists, int claimCount, IEnumerable<string> claimTypes)> DiagnoseUserClaimsAsync(ApplicationUser user)
        {
            try
            {
                // Verificar se a tabela existe
                var tableExists = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name='AspNetUserClaims'",
                    _unitOfWork.Transaction);

                // Contar claims do usuário
                var claimCount = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM AspNetUserClaims WHERE UserId = @UserId",
                    new { UserId = user.Id },
                    _unitOfWork.Transaction);

                // Obter tipos únicos de claims
                var claimTypes = await _unitOfWork.Connection.QueryAsync<string>(
                    "SELECT DISTINCT ClaimType FROM AspNetUserClaims WHERE UserId = @UserId",
                    new { UserId = user.Id },
                    _unitOfWork.Transaction);

                return (tableExists > 0, claimCount, claimTypes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao diagnosticar claims: {ex.Message}");
                return (false, 0, Array.Empty<string>());
            }
        }
        #endregion

        #region IUserLoginStore Implementation
        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                await _unitOfWork.Connection.ExecuteAsync(
                    @"INSERT INTO AspNetUserLogins (LoginProvider, ProviderKey, ProviderDisplayName, UserId)
                    VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId)",
                    new
                    {
                        login.LoginProvider,
                        login.ProviderKey,
                        login.ProviderDisplayName,
                        UserId = user.Id
                    },
                    _unitOfWork.Transaction);
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                @"SELECT u.* FROM AspNetUsers u
                INNER JOIN AspNetUserLogins l ON u.Id = l.UserId
                WHERE l.LoginProvider = @LoginProvider AND l.ProviderKey = @ProviderKey",
                new { LoginProvider = loginProvider, ProviderKey = providerKey },
                _unitOfWork.Transaction);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var logins = await _unitOfWork.Connection.QueryAsync<UserLoginInfo>(
                @"SELECT LoginProvider, ProviderKey, ProviderDisplayName
                FROM AspNetUserLogins WHERE UserId = @UserId",
                new { UserId = user.Id },
                _unitOfWork.Transaction);

            return logins.ToList();
        }

        public async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                await _unitOfWork.Connection.ExecuteAsync(
                    @"DELETE FROM AspNetUserLogins
                    WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey",
                    new
                    {
                        UserId = user.Id,
                        LoginProvider = loginProvider,
                        ProviderKey = providerKey
                    },
                    _unitOfWork.Transaction);
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
        #endregion

        #region IUserTokenStore Implementation
        public async Task SetTokenAsync(ApplicationUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            // Primeiro tenta atualizar o token se ele já existir
            var updated = await _unitOfWork.Connection.ExecuteAsync(
                @"UPDATE AspNetUserTokens
            SET Value = @Value
            WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name",
                new
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value
                },
                _unitOfWork.Transaction);
            //await _connection.ExecuteAsync();

            // Se não existe, insere um novo
            if (updated == 0)
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    @"INSERT INTO AspNetUserTokens (UserId, LoginProvider, Name, Value)
                    VALUES (@UserId, @LoginProvider, @Name, @Value)",
                    new
                    {
                        UserId = user.Id,
                        LoginProvider = loginProvider,
                        Name = name,
                        Value = value
                    },
                    _unitOfWork.Transaction);
            }
        }

        public async Task RemoveTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"DELETE FROM AspNetUserTokens
                WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name",
                new
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name
                },
                _unitOfWork.Transaction);
        }

        public async Task<string> GetTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<string>(
                @"SELECT Value FROM AspNetUserTokens
            WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name",
                new
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name
                },
                _unitOfWork.Transaction);
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
            return Task.FromResult(user.LockoutEnd);
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
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }
        #endregion

        public IQueryable<ApplicationUser> Users
        {
            get
            {
                var users = _unitOfWork.Connection.Query<ApplicationUser>(
                    "SELECT * FROM AspNetUsers",
                    transaction: _unitOfWork.Transaction);
                return users.AsQueryable();
            }
        }
    }
}
