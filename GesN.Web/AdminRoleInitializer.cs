using GesN.Web.Areas.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace GesN.Web
{
    public class AdminRoleInitializer
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await CreateAdminRole.Initialize(services);
                    Console.WriteLine("Inicialização da role Admin concluída com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro durante a inicialização: {ex.Message}");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Configurar o SQLite e os serviços necessários
                    string connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection") ?? 
                        "Data Source=./Data/Database/gesn.db";

                    services.AddInfrastructureServices(connectionString);
                    
                    services.AddIdentity<Areas.Identity.Data.Models.ApplicationUser, Areas.Identity.Data.Models.ApplicationRole>()
                        .AddUserStore<Areas.Identity.Data.Stores.DapperUserStore>()
                        .AddRoleStore<Areas.Identity.Data.Stores.DapperRoleStore>()
                        .AddDefaultTokenProviders();
                });
    }
} 