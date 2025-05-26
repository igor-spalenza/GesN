using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Infrastructure.Data;
using System.Data;

namespace GesN.Web.Data.Seeds.IdentitySeeds
{
    public class IdentitySeeder : BaseIdentitySeeder
    {
        public IdentitySeeder(IDbConnectionFactory connectionFactory) 
            : base(connectionFactory)
        {
        }

        public async Task SeedAsync()
        {
            // 1. Criar todas as roles
            foreach (var roleName in IdentityConfiguration.Roles.GetAllRoles())
            {
                var result = await CreateRoleIfNotExists(roleName);
                if (!result)
                {
                    throw new InvalidOperationException($"Failed to create role '{roleName}'");
                }
            }

            // 2. Adicionar claims às roles
            var roleClaimsMap = IdentityConfiguration.GetRoleClaimsMap();
            foreach (var roleName in roleClaimsMap.Keys)
            {
                var result = await AddClaimsToRole(roleName, roleClaimsMap[roleName]);
                if (!result)
                {
                    throw new InvalidOperationException($"Failed to add claims to role '{roleName}'");
                }
            }

            // 3. Criar usuário admin
            var createUserResult = await CreateUserIfNotExists(
                IdentityConfiguration.AdminUser.Email,
                IdentityConfiguration.AdminUser.Password);

            if (!createUserResult)
            {
                throw new InvalidOperationException("Failed to create admin user");
            }

            // 4. Adicionar todas as roles ao usuário admin
            var addRolesResult = await AddRolesToUser(
                IdentityConfiguration.AdminUser.Email,
                IdentityConfiguration.Roles.GetAllRoles());

            if (!addRolesResult)
            {
                throw new InvalidOperationException("Failed to add roles to admin user");
            }

            // 5. Adicionar todas as claims ao usuário admin
            var addClaimsResult = await AddClaimsToUser(
                IdentityConfiguration.AdminUser.Email,
                IdentityConfiguration.Claims.GetAdminClaims());

            if (!addClaimsResult)
            {
                throw new InvalidOperationException("Failed to add claims to admin user");
            }
        }
    }
} 