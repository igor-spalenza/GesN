using GesN.Web;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Infrastructure.Configuration;
using GesN.Web.Data.Migrations;
using GesN.Web.Data;
using GesN.Web.Services;
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

// Configurações organizadas usando DependencyInjection
builder.Services.AddIdentityServices();
builder.Services.AddAuthenticationServices();
builder.Services.AddAuthorizationServices();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddInfrastructureServices(connectionString);

// ✅ TESTE: Inicialização mínima para verificar se é a causa da lentidão
try
{
    // Verificar se o arquivo do banco existe
    string dbPath2 = connectionString.Replace("Data Source=", "").Split(';')[0];
    if (!File.Exists(dbPath2))
    {
        Console.WriteLine("Banco não existe. Criando...");
        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            var identityInit = new IdentitySchemaInit(connectionString);
            identityInit.Initialize();
            
            var dbInit = new DbInit(connectionString);
            dbInit.Initialize();
            
            var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();
            await seedData.Initialize();
            Console.WriteLine("Banco criado com sucesso!");
        }
    }
    else
    {
        Console.WriteLine("Banco já existe. Pulando inicialização.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao inicializar banco: {ex.Message}");
    // Não fazer throw - deixar a aplicação continuar
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

