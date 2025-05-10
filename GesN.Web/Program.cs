using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Areas.Identity.Data.Stores;
using GesN.Web.Areas.Infrastructure;
using GesN.Web.Data;
using GesN.Web.Data.Migrations;
using GesN.Web.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;

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


builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<ApplicationRole>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IUserStore<ApplicationUser>, DapperUserStore>();
builder.Services.AddScoped<IRoleStore<ApplicationRole>, DapperRoleStore>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<ApplicationRole>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddInfrastructureServices(connectionString);

builder.Services.AddScoped<IdentitySchemaInit>(provider => new IdentitySchemaInit(connectionString));

builder.Services.AddScoped<DbInit>(provider => new DbInit(connectionString));

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbIdentityInit = scope.ServiceProvider.GetRequiredService<IdentitySchemaInit>();
    dbIdentityInit.Initialize();
    var dbInit = scope.ServiceProvider.GetRequiredService<DbInit>();
    dbInit.Initialize();
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();

