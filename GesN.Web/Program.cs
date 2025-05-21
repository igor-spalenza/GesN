using GesN.Web;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Areas.Identity.Data.Stores;
using GesN.Web.Areas.Infrastructure;
using GesN.Web.Data;
using GesN.Web.Data.Migrations;
using GesN.Web.Data.Repositories;
using GesN.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;

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
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddUserStore<DapperUserStore>()
.AddRoleStore<DapperRoleStore>()
.AddDefaultTokenProviders();

// Add services directly instead of separately registering them
builder.Services.AddTransient<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddInfrastructureServices(connectionString);

builder.Services.AddScoped<IdentitySchemaInit>(provider => new IdentitySchemaInit(connectionString));

builder.Services.AddScoped<DbInit>(provider => new DbInit(connectionString));

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbIdentityInit = scope.ServiceProvider.GetRequiredService<IdentitySchemaInit>();
    dbIdentityInit.Initialize();
    var dbInit = scope.ServiceProvider.GetRequiredService<DbInit>();
    dbInit.Initialize();
    
    // Inicializar a role Admin e atribuir ao usuário
    try
    {
        await CreateAdminRole.Initialize(scope.ServiceProvider);
        Console.WriteLine("Role Admin inicializada com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao inicializar a role Admin: {ex.Message}");
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

