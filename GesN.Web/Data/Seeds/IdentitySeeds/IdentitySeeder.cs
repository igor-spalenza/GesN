using GesN.Web.Areas.Identity.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Data.Seeds.IdentitySeeds
{
    public class IdentitySeeder : BaseIdentitySeeder
    {
        public IdentitySeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager) 
            : base(userManager, roleManager)
        {
        }

        public async Task SeedAsync()
        {
            // 1. Criar todas as roles
            foreach (var roleName in IdentityConfiguration.Roles.GetAllRoles())
            {
                var result = await CreateRoleIfNotExists(roleName);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // 2. Adicionar claims às roles
            var roleClaimsMap = IdentityConfiguration.GetRoleClaimsMap();
            foreach (var roleName in roleClaimsMap.Keys)
            {
                var result = await AddClaimsToRole(roleName, roleClaimsMap[roleName]);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to add claims to role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // 3. Criar usuário admin
            var createUserResult = await CreateUserIfNotExists(
                IdentityConfiguration.AdminUser.Email,
                IdentityConfiguration.AdminUser.Password);

            if (!createUserResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
            }

            // 4. Adicionar todas as roles ao usuário admin
            var addRolesResult = await AddRolesToUser(
                IdentityConfiguration.AdminUser.Email,
                IdentityConfiguration.Roles.GetAllRoles());

            if (!addRolesResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to add roles to admin user: {string.Join(", ", addRolesResult.Errors.Select(e => e.Description))}");
            }

            // 5. Adicionar todas as claims ao usuário admin
            var addClaimsResult = await AddClaimsToUser(
                IdentityConfiguration.AdminUser.Email,
                IdentityConfiguration.Claims.GetAdminClaims());

            if (!addClaimsResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to add claims to admin user: {string.Join(", ", addClaimsResult.Errors.Select(e => e.Description))}");
            }
        }
    }
} 