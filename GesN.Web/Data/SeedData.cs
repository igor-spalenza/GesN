using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data.Seeds.IdentitySeeds;
using GesN.Web.Infrastructure.Data;
using System.Data;

namespace GesN.Web.Data
{
    public class SeedData
    {
        private readonly IdentitySeeder _identitySeeder;

        public SeedData(IDbConnectionFactory connectionFactory)
        {
            _identitySeeder = new IdentitySeeder(connectionFactory);
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