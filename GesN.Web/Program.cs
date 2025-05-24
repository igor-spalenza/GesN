using GesN.Web;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Areas.Identity.Data.Stores;
using GesN.Web.Infrastructure.Configuration;
using GesN.Web.Data;
using GesN.Web.Data.Migrations;
using GesN.Web.Data.Repositories;
using GesN.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using GesN.Web.Infrastructure.Data.Repositories;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces;
using GesN.Web.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

/*builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // HTTP
    options.ListenAnyIP(81, listenOptions => listenOptions.UseHttps()); // HTTPS
    options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps()); // HTTPS
});*/

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

SQLitePCL.Batteries.Init();
string dbPath = Path.Combine(AppContext.BaseDirectory, "/GesN.Web/Data/Database/gesn.db");

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Configurações de conta
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    
    // Configurações de senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    
    // Configurações de usuário
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    
    // Configurações de bloqueio
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // Configuração de tokens
    options.Tokens.EmailConfirmationTokenProvider = "EmailTokenProvider";
    options.Tokens.PasswordResetTokenProvider = "EmailTokenProvider";
})
.AddUserStore<DapperUserStore>()
.AddRoleStore<DapperRoleStore>()
.AddDefaultTokenProviders()
.AddTokenProvider<EmailTokenProvider<ApplicationUser>>("EmailTokenProvider");

// Configuração de tokens
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(3); // Token padrão dura 3 horas
});

// Configuração do provedor de token de email
builder.Services.Configure<DataProtectionTokenProviderOptions>("EmailTokenProvider", options =>
{
    options.TokenLifespan = TimeSpan.FromDays(1); // Tokens de email duram 1 dia
});

// Adicionar o RoleManager
builder.Services.AddScoped<RoleManager<ApplicationRole>>();

// Adicionar serviço de seed de dados
builder.Services.AddScoped<SeedData>();

// Add services directly instead of separately registering them
builder.Services.AddTransient<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Configuração de caminhos
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    
    // Configuração de cookies
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Em produção, use 'Always'
    options.Cookie.SameSite = SameSiteMode.Lax;
    
    // Manipuladores de eventos
    options.Events.OnRedirectToLogin = context =>
    {
        // Para requisições AJAX, retornar 401 em vez de redirecionar
        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// Adicionar autorização baseada em políticas
builder.Services.AddAuthorization(options =>
{
    // Exemplo de política que requer uma claim específica
    options.AddPolicy("GerenciarUsuarios", policy =>
        policy.RequireClaim("permissao", "usuarios:gerenciar"));
        
    options.AddPolicy("GerenciarClientes", policy =>
        policy.RequireClaim("permissao", "clientes:gerenciar"));
        
    options.AddPolicy("GerenciarPedidos", policy =>
        policy.RequireClaim("permissao", "pedidos:gerenciar"));
});

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddInfrastructureServices(connectionString);

builder.Services.AddScoped<IdentitySchemaInit>(provider => new IdentitySchemaInit(connectionString));

builder.Services.AddScoped<DbInit>(provider => new DbInit(connectionString));

// Registrar UnitOfWork e Repositories
builder.Services.AddScoped<IUnitOfWork>(provider =>
{
    var context = provider.GetRequiredService<ProjectDataContext>();
    return new UnitOfWork(context.Connection);
});

builder.Services.AddScoped<UserRepository>();

builder.Services.AddMemoryCache();

// Add Identity configuration
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbIdentityInit = scope.ServiceProvider.GetRequiredService<IdentitySchemaInit>();
    dbIdentityInit.Initialize();
    var dbInit = scope.ServiceProvider.GetRequiredService<DbInit>();
    dbInit.Initialize();
    
    // Inicializar dados padrão (roles, usuários e permissões)
    try
    {
        var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();
        
        // Verificar se o banco está vazio antes de fazer seed
        if (dbIdentityInit.IsDatabaseEmpty())
        {
            Console.WriteLine("Banco vazio detectado. Iniciando processo de seed...");
            await seedData.Initialize();
            Console.WriteLine("Dados iniciais criados com sucesso!");
            Console.WriteLine("Usuário admin criado:");
            Console.WriteLine("Email: igor.gesn@gmail.com");
            Console.WriteLine("Senha: Admin@123");
        }
        else
        {
            Console.WriteLine("Banco já contém dados. Seed não será executado.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao inicializar dados: {ex.Message}");
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();  Remoção -> EF Core
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseLoginRateLimiting();
app.UseAuthorization();

// Mapear a rota da área Admin primeiro
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Depois a rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

