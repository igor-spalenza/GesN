using GesN.Web.Areas.Identity.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GesN.Web.Data.Seeds.IdentitySeeds
{
    public abstract class BaseIdentitySeeder
    {
        protected readonly UserManager<ApplicationUser> UserManager;
        protected readonly RoleManager<ApplicationRole> RoleManager;

        protected BaseIdentitySeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        protected async Task<IdentityResult> CreateRoleIfNotExists(string roleName)
        {
            if (!await RoleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                return await RoleManager.CreateAsync(role);
            }
            return IdentityResult.Success;
        }

        protected async Task<IdentityResult> AddClaimsToRole(string roleName, Dictionary<string, List<string>> claims)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new InvalidOperationException($"Role '{roleName}' not found.");
            }

            foreach (var claimType in claims.Keys)
            {
                foreach (var claimValue in claims[claimType])
                {
                    var claim = new Claim(claimType, claimValue);
                    var result = await RoleManager.AddClaimAsync(role, claim);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
            }

            return IdentityResult.Success;
        }

        protected async Task<IdentityResult> CreateUserIfNotExists(string email, string password)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                return await UserManager.CreateAsync(user, password);
            }
            return IdentityResult.Success;
        }

        protected async Task<IdentityResult> AddRolesToUser(string email, IEnumerable<string> roles)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException($"User with email '{email}' not found.");
            }

            foreach (var role in roles)
            {
                var result = await UserManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            return IdentityResult.Success;
        }

        protected async Task<IdentityResult> AddClaimsToUser(string email, Dictionary<string, List<string>> claims)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException($"User with email '{email}' not found.");
            }

            foreach (var claimType in claims.Keys)
            {
                foreach (var claimValue in claims[claimType])
                {
                    var claim = new Claim(claimType, claimValue);
                    var result = await UserManager.AddClaimAsync(user, claim);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
            }

            return IdentityResult.Success;
        }
    }
} 