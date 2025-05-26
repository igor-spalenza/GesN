using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Dapper;
using System.Security.Claims;
using BCrypt.Net;

namespace GesN.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IDbConnectionFactory connectionFactory, ILogger<LoginModel> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }
        
        [TempData]
        public string DebugInfo { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Email ou Nome de Usuário")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Lembrar-me")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                try
                {
                    using var connection = await _connectionFactory.CreateConnectionAsync();
                    
                    // Buscar usuário por email ou username
                    var user = await connection.QuerySingleOrDefaultAsync<ApplicationUser>(@"
                        SELECT * FROM AspNetUsers 
                        WHERE NormalizedEmail = @Email OR NormalizedUserName = @Email",
                        new { Email = Input.Email.ToUpper() });

                    if (user == null)
                    {
                        _logger.LogWarning("Usuário não encontrado: {Email}", Input.Email);
                        ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                        return Page();
                    }

                    // Verificar senha usando BCrypt
                    if (!BCrypt.Net.BCrypt.Verify(Input.Password, user.PasswordHash))
                    {
                        _logger.LogWarning("Senha incorreta para usuário: {Email}", Input.Email);
                        ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                        return Page();
                    }

                    // Verificar se a conta está bloqueada
                    if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                    {
                        _logger.LogWarning("Conta bloqueada para usuário: {Email}", Input.Email);
                        ModelState.AddModelError(string.Empty, "Conta temporariamente bloqueada.");
                        return Page();
                    }

                    // Debug info
                    DebugInfo = $"Login bem-sucedido: {user.Email} (ID: {user.Id})";
                    _logger.LogInformation("Login bem-sucedido para usuário: {Email}", Input.Email);

                    // Buscar roles do usuário
                    var roles = await connection.QueryAsync<string>(@"
                        SELECT r.Name
                        FROM AspNetRoles r
                        INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                        WHERE ur.UserId = @UserId",
                        new { UserId = user.Id });

                    // Buscar claims do usuário
                    var userClaims = await connection.QueryAsync(@"
                        SELECT ClaimType, ClaimValue
                        FROM AspNetUserClaims
                        WHERE UserId = @UserId",
                        new { UserId = user.Id });

                    // Buscar claims das roles
                    var roleClaims = await connection.QueryAsync(@"
                        SELECT rc.ClaimType, rc.ClaimValue
                        FROM AspNetRoleClaims rc
                        INNER JOIN AspNetUserRoles ur ON rc.RoleId = ur.RoleId
                        WHERE ur.UserId = @UserId",
                        new { UserId = user.Id });

                    // Criar claims para autenticação
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("FirstName", user.FirstName ?? ""),
                        new Claim("LastName", user.LastName ?? "")
                    };

                    // Adicionar roles
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    // Adicionar claims do usuário
                    foreach (var claim in userClaims)
                    {
                        claims.Add(new Claim((string)claim.ClaimType, (string)claim.ClaimValue));
                    }

                    // Adicionar claims das roles
                    foreach (var claim in roleClaims)
                    {
                        claims.Add(new Claim((string)claim.ClaimType, (string)claim.ClaimValue));
                    }

                    // Fazer login
                    var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(
                        IdentityConstants.ApplicationScheme,
                        claimsPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = Input.RememberMe,
                            ExpiresUtc = Input.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
                        });

                    return LocalRedirect(returnUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no login: {Message}", ex.Message);
                    ModelState.AddModelError(string.Empty, $"Erro no login: {ex.Message}");
                    return Page();
                }
            }

            return Page();
        }
    }
}
