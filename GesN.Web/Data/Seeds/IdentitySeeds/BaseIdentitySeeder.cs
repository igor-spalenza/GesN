using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data;
using System.Security.Claims;
using BCrypt.Net;
using System.Data;
using Dapper;
using GesN.Web.Infrastructure.Data;

namespace GesN.Web.Data.Seeds.IdentitySeeds
{
    public abstract class BaseIdentitySeeder
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        protected BaseIdentitySeeder(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        protected async Task<bool> CreateRoleIfNotExists(string roleName)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var existingRole = await connection.QuerySingleOrDefaultAsync(
                    "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                    new { NormalizedName = roleName.ToUpper() });

                if (existingRole == null)
                {
                    var roleId = Guid.NewGuid().ToString();
                    await connection.ExecuteAsync(@"
                        INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                        VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)",
                        new
                        {
                            Id = roleId,
                            Name = roleName,
                            NormalizedName = roleName.ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        });
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected async Task<bool> AddClaimsToRole(string roleName, Dictionary<string, List<string>> claims)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var role = await connection.QuerySingleOrDefaultAsync(
                    "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                    new { NormalizedName = roleName.ToUpper() });

                if (role == null)
                {
                    throw new InvalidOperationException($"Role '{roleName}' not found.");
                }

                var roleId = (string)role.Id;

                foreach (var claimType in claims.Keys)
                {
                    foreach (var claimValue in claims[claimType])
                    {
                        // Verificar se a claim já existe
                        var existingClaim = await connection.QuerySingleOrDefaultAsync(
                            "SELECT Id FROM AspNetRoleClaims WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                            new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });

                        if (existingClaim == null)
                        {
                            var result = await connection.ExecuteAsync(@"
                                INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                                VALUES (@RoleId, @ClaimType, @ClaimValue)",
                                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });

                            if (result == 0)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected async Task<bool> CreateUserIfNotExists(string email, string password)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var existingUser = await connection.QuerySingleOrDefaultAsync(
                    "SELECT Id FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail",
                    new { NormalizedEmail = email.ToUpper() });

                if (existingUser == null)
                {
                    var userId = Guid.NewGuid().ToString();
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    
                    await connection.ExecuteAsync(@"
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
                        )",
                        new
                        {
                            Id = userId,
                            UserName = email,
                            NormalizedUserName = email.ToUpper(),
                            Email = email,
                            NormalizedEmail = email.ToUpper(),
                            EmailConfirmed = true,
                            PasswordHash = hashedPassword,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                            PhoneNumber = "",
                            PhoneNumberConfirmed = false,
                            TwoFactorEnabled = false,
                            LockoutEnd = (DateTimeOffset?)null,
                            LockoutEnabled = true,
                            AccessFailedCount = 0,
                            FirstName = "Admin",
                            LastName = "User"
                        });
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected async Task<bool> AddRolesToUser(string email, IEnumerable<string> roles)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var user = await connection.QuerySingleOrDefaultAsync(
                    "SELECT Id FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail",
                    new { NormalizedEmail = email.ToUpper() });

                if (user == null)
                {
                    throw new InvalidOperationException($"User with email '{email}' not found.");
                }

                var userId = (string)user.Id;

                foreach (var roleName in roles)
                {
                    var role = await connection.QuerySingleOrDefaultAsync(
                        "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                        new { NormalizedName = roleName.ToUpper() });

                    if (role != null)
                    {
                        var roleId = (string)role.Id;
                        
                        // Verificar se a associação já existe
                        var existingUserRole = await connection.QuerySingleOrDefaultAsync(
                            "SELECT UserId FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
                            new { UserId = userId, RoleId = roleId });

                        if (existingUserRole == null)
                        {
                            await connection.ExecuteAsync(@"
                                INSERT INTO AspNetUserRoles (UserId, RoleId)
                                VALUES (@UserId, @RoleId)",
                                new { UserId = userId, RoleId = roleId });
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected async Task<bool> AddClaimsToUser(string email, Dictionary<string, List<string>> claims)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var user = await connection.QuerySingleOrDefaultAsync(
                    "SELECT Id FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail",
                    new { NormalizedEmail = email.ToUpper() });

                if (user == null)
                {
                    throw new InvalidOperationException($"User with email '{email}' not found.");
                }

                var userId = (string)user.Id;

                foreach (var claimType in claims.Keys)
                {
                    foreach (var claimValue in claims[claimType])
                    {
                        // Verificar se a claim já existe
                        var existingClaim = await connection.QuerySingleOrDefaultAsync(
                            "SELECT Id FROM AspNetUserClaims WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                            new { UserId = userId, ClaimType = claimType, ClaimValue = claimValue });

                        if (existingClaim == null)
                        {
                            var result = await connection.ExecuteAsync(@"
                                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                                VALUES (@UserId, @ClaimType, @ClaimValue)",
                                new { UserId = userId, ClaimType = claimType, ClaimValue = claimValue });

                            if (result == 0)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 