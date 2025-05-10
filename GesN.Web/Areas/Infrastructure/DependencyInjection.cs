using GesN.Web.Areas.Identity.Data.Stores;
using GesN.Web.Areas.Identity.Data;
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

            services.AddSingleton<IDbConnection>(db => new SqliteConnection(connectionString));

            // Registro dos repositórios do Domínio da aplicação
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();

            services.AddScoped<ProjectDataContext>(provider => { return new ProjectDataContext(connectionString); });
        }
    }
}
