using GesN.Web.Data;
using GesN.Web.Data.Repositories;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using System.Data;

namespace GesN.Web.Areas.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            // Configuração do contexto de dados
            services.AddScoped<IDbConnection>(db => new SqliteConnection(connectionString));

            /*services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders();

            services.AddTransient<IUserStore<IdentityUser>, CustomUserStore>();
            services.AddTransient<IRoleStore<IdentityRole>, CustomRoleStore>();
            */
            // Registro dos repositórios
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();

            services.AddScoped<UserStoreRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ProjectDataContext>(provider => { return new ProjectDataContext(connectionString); });

            services.AddScoped<IUserStore<IdentityUser>, UserStoreRepository>();
        }
    }
}
