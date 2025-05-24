using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Areas.Identity.Data.Stores;
using GesN.Web.Data.Seeds.IdentitySeeds;
using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Data
{
    public class SeedData
    {
        private readonly IdentitySeeder _identitySeeder;

        public SeedData(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _identitySeeder = new IdentitySeeder(userManager, roleManager);
        }

        public async Task Initialize()
        {
            try
            {
                await _identitySeeder.SeedAsync();
                Console.WriteLine("Identity seed completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during identity seed: {ex.Message}");
                throw;
            }
        }
    }
} 