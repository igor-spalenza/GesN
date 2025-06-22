using GesN.Web.Areas.Identity.Data.Stores;
using GesN.Web.Areas.Identity.Data;
using GesN.Web.Data;
using GesN.Web.Data.Repositories;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using System.Data;
using GesN.Web.Areas.Identity.Data.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using GesN.Web.Data.Migrations;
using GesN.Web.Areas.Integration.Models.Settings;
using GesN.Web.Areas.Integration.Services;
using Dapper;

namespace GesN.Web.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbConnectionFactory>(provider => new ProjectDataContext(connectionString));

            services.AddHttpContextAccessor();

            // Google Workspace Integration (moved to Integration area)
            services.AddScoped<IGooglePeopleService, GooglePeopleService>();

            // Domínio Sales
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<IContractService, ContractService>();

            // Legados
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();

            services.AddMemoryCache();
        }

        public static void AddGoogleWorkspaceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar Google Workspace settings
            services.Configure<GoogleWorkspaceSettings>(
                configuration.GetSection(GoogleWorkspaceSettings.SectionName));
        }

        public static void EnsureDatabaseInitialized(this IServiceProvider serviceProvider, string connectionString)
        {
            var dbInit = new DbInit(connectionString);
            dbInit.Initialize();

            DefaultTypeMap.MatchNamesWithUnderscores = false;
        }

        public static void AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                
                // Senhas mais simples para teste
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = false; // Desabilitar lockout para teste
            })
            .AddUserStore<DapperUserStore>()
            .AddRoleStore<DapperRoleStore>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>();
            services.AddTransient<IEmailSender, EmailSender>();
        }

        public static void AddAuthenticationServices(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });
        }

        public static void AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("GerenciarUsuarios", policy =>
                    policy.RequireClaim("permissao", "usuarios:gerenciar"));
                    
                options.AddPolicy("GerenciarClientes", policy =>
                    policy.RequireClaim("permissao", "clientes:gerenciar"));
                    
                options.AddPolicy("GerenciarPedidos", policy =>
                    policy.RequireClaim("permissao", "pedidos:gerenciar"));
            });
        }
    }
}
