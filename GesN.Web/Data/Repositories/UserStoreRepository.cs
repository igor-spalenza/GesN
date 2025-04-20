using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace GesN.Web.Data.Repositories
{
    public class UserStoreRepository :  IUserStore<IdentityUser>,
                                        IUserPasswordStore<IdentityUser>,
                                        IUserEmailStore<IdentityUser>,
                                        IUserClaimStore<IdentityUser>,
                                        IUserRoleStore<IdentityUser>,
                                        IUserPhoneNumberStore<IdentityUser>//, IUserSecurityStampStore<IdentityUser>
    {
        private readonly IDbConnection _dbConnection;

        public UserStoreRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<IdentityUser>(sql, new { Id = userId });
        }

        public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName";
            return await _dbConnection.QuerySingleOrDefaultAsync<IdentityUser>(sql, new { NormalizedUserName = normalizedUserName });
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            // Verifique se o usuário é nulo
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            // Defina um security stamp ao criar o usuário
            user.SecurityStamp = Guid.NewGuid().ToString(); // Gera um novo security stamp

            // Prepare a consulta SQL para inserir o usuário
            var sql = @"
                INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash, SecurityStamp)
                VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @PasswordHash, @SecurityStamp)";

            // Execute a inserção no banco de dados
            var result = await _dbConnection.ExecuteAsync(sql, new
            {
                Id = user.Id,
                UserName = user.UserName,
                NormalizedUserName = user.NormalizedUserName,
                Email = user.Email,
                NormalizedEmail = user.NormalizedEmail,
                PasswordHash = user.PasswordHash,
                SecurityStamp = user.SecurityStamp // Inclua o security stamp na inserção
            });

            // Retorne o resultado da operação
            return result > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Failed to create user." });
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            // Verifique se o usuário é nulo
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            // Atualize o security stamp para invalidar tokens de autenticação existentes
            user.SecurityStamp = Guid.NewGuid().ToString();

            // Prepare a consulta SQL para atualizar o usuário
            var sql = @"
                UPDATE AspNetUsers
                SET UserName = @UserName,
                    NormalizedUserName = @NormalizedUserName,
                    Email = @Email,
                    NormalizedEmail = @NormalizedEmail,
                    PasswordHash = @PasswordHash,
                    SecurityStamp = @SecurityStamp
                WHERE Id = @Id";

            try
            {
                // Execute a atualização no banco de dados
                var result = await _dbConnection.ExecuteAsync(sql, new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    NormalizedUserName = user.NormalizedUserName,
                    Email = user.Email,
                    NormalizedEmail = user.NormalizedEmail,
                    PasswordHash = user.PasswordHash,
                    SecurityStamp = user.SecurityStamp
                });

                // Retorne o resultado da operação
                return result > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Failed to update user." });
            }
            catch (Exception ex)
            {
                // Capture e trate exceções que possam ocorrer durante a atualização
                return IdentityResult.Failed(new IdentityError { Description = $"An error occurred while updating the user: {ex.Message}" });
            }
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var sql = "DELETE FROM AspNetUsers WHERE Id = @Id";
            var result = await _dbConnection.ExecuteAsync(sql, new { Id = user.Id });

            return result > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Failed to delete user." });
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Dispose of any resources if necessary
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        // Métodos da IUserEmailStore
        public Task<string> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task SetEmailAsync(IdentityUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public async Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            var sql = "UPDATE AspNetUsers SET EmailConfirmed = @EmailConfirmed WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { EmailConfirmed = confirmed, Id = user.Id });
        }

        public async Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var sql = "SELECT EmailConfirmed FROM AspNetUsers WHERE Id = @Id";
            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { Id = user.Id });
        }

        public async Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail";
            return await _dbConnection.QuerySingleOrDefaultAsync<IdentityUser>(sql, new { NormalizedEmail = normalizedEmail });
        }

        // Métodos da IUserClaimStore
        public async Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var sql = "INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) VALUES (@UserId, @ClaimType, @ClaimValue)";

            foreach (var claim in claims)
            {
                await _dbConnection.ExecuteAsync(sql, new { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            }
        }

        public async Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var sql = "DELETE FROM AspNetUserClaims WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";

            foreach (var claim in claims)
            {
                await _dbConnection.ExecuteAsync(sql, new { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            }
        }
        public async Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var sql = "SELECT ClaimType, ClaimValue FROM AspNetUserClaims WHERE UserId = @UserId";
            var claims = await _dbConnection.QueryAsync<ClaimDto>(sql, new { UserId = user.Id });

            // Criar uma lista de Claim manualmente
            var claimList = new List<Claim>();
            foreach (var c in claims)
            {
                claimList.Add(new Claim(c.ClaimType, c.ClaimValue));
            }

            return claimList;
        }

        public async Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            // Remove the old claim
            await RemoveClaimsAsync(user, new[] { claim }, cancellationToken);
            // Add the new claim
            await AddClaimsAsync(user, new[] { newClaim }, cancellationToken);
        }

        public async Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var sql = "SELECT u.* FROM AspNetUsers u INNER JOIN UserClaims uc ON u.Id = uc.UserId WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue";
            var users = await _dbConnection.QueryAsync<IdentityUser>(sql, new { ClaimType = claim.Type, ClaimValue = claim.Value });
            return users.ToList();
        }

        // Métodos da IUserRoleStore
        public async Task AddToRoleAsync(IdentityUser user, string roleId, CancellationToken cancellationToken)
        {
            var sql = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            await _dbConnection.ExecuteAsync(sql, new { UserId = user.Id, RoleId = roleId });
        }

        public async Task RemoveFromRoleAsync(IdentityUser user, string roleId, CancellationToken cancellationToken)
        {
            var sql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
            await _dbConnection.ExecuteAsync(sql, new { UserId = user.Id, RoleId = roleId });
        }

        public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var sql = "SELECT RoleId FROM AspNetUserRoles WHERE UserId = @UserId";
            var roles = await _dbConnection.QueryAsync<string>(sql, new { UserId = user.Id });
            return roles.ToList();
        }

        public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            var sql = "SELECT COUNT(1) FROM AspNetUserRoles WHERE UserId = @UserId AND RoleName = @RoleName";
            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { UserId = user.Id, RoleName = roleName });
            return count > 0;
        }

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var sql = "SELECT u.* FROM AspNetUsers u INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId WHERE ur.RoleName = @RoleName";
            var users = await _dbConnection.QueryAsync<IdentityUser>(sql, new { RoleName = roleName });
            return users.ToList();
        }
        public async Task SetPhoneNumberAsync(IdentityUser user, string? phoneNumber, CancellationToken cancellationToken)
        {
            // Verifique se o usuário é nulo
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            // Prepare a consulta SQL para atualizar o número de telefone
            var sql = "UPDATE AspNetUsers SET PhoneNumber = @PhoneNumber WHERE Id = @UserId";
            await _dbConnection.ExecuteAsync(sql, new { PhoneNumber = phoneNumber, UserId = user.Id });
        }

        // Métodos da IUserPhoneStore
        public async Task<string?> GetPhoneNumberAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            // Verifique se o usuário é nulo
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            // Prepare a consulta SQL para obter o número de telefone
            var sql = "SELECT PhoneNumber FROM AspNetUsers WHERE Id = @UserId";
            return await _dbConnection.ExecuteScalarAsync<string?>(sql, new { UserId = user.Id });
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            // Verifique se o usuário é nulo
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            // Prepare a consulta SQL para obter a confirmação do número de telefone
            var sql = "SELECT PhoneNumberConfirmed FROM AspNetUsers WHERE Id = @UserId";
            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { UserId = user.Id });
        }

        public async Task SetPhoneNumberConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            // Verifique se o usuário é nulo
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            // Prepare a consulta SQL para atualizar a confirmação do número de telefone
            var sql = "UPDATE AspNetUsers SET PhoneNumberConfirmed = @PhoneNumberConfirmed WHERE Id = @UserId";
            await _dbConnection.ExecuteAsync(sql, new { PhoneNumberConfirmed = confirmed, UserId = user.Id });
        }

        // Métodos da IUserSecurityStampStore
        /*public async Task<string> GetSecurityStampAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var sql = "SELECT SecurityStamp FROM AspNetUsers WHERE Id = @UserId";
            var securityStamp = await _dbConnection.ExecuteScalarAsync<string>(sql, new { UserId = user.Id });

            if (securityStamp == null)
            {
                throw new InvalidOperationException("User security stamp cannot be null.");
            }

            return securityStamp;
        }

        public async Task SetSecurityStampAsync(IdentityUser user, string stamp, CancellationToken cancellationToken)
        {
            var sql = "UPDATE AspNetUsers SET SecurityStamp = @SecurityStamp WHERE Id = @UserId";
            await _dbConnection.ExecuteAsync(sql, new { SecurityStamp = stamp, UserId = user.Id });
        }*/
    }
}
