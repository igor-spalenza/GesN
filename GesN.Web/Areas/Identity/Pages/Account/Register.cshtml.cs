using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Dapper;
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;

namespace GesN.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(IDbConnectionFactory connectionFactory, ILogger<RegisterModel> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome é obrigatório")]
            [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 2)]
            [Display(Name = "Nome")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "O sobrenome é obrigatório")]
            [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 2)]
            [Display(Name = "Sobrenome")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirme a senha")]
            [Compare("Password", ErrorMessage = "As senhas não correspondem.")]
            public string ConfirmPassword { get; set; }
        }
        
        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                try 
                {
                    _logger.LogInformation("Iniciando registro para: {Email}", Input.Email);
                    
                    // ✅ CORREÇÃO: Usar apenas UMA conexão sequencial (sem transação)
                    using var connection = await _connectionFactory.CreateConnectionAsync();
                    
                    // Verificar se o usuário já existe
                    var existingUser = await connection.QuerySingleOrDefaultAsync(
                        "SELECT Id FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail",
                        new { NormalizedEmail = Input.Email.ToUpper() });

                    if (existingUser != null)
                    {
                        _logger.LogWarning("Usuário já existe: {Email}", Input.Email);
                        ModelState.AddModelError(string.Empty, "Este email já está em uso.");
                        return Page();
                    }

                    // Criar hash da senha
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password);
                    var userId = Guid.NewGuid().ToString();
                    
                    // Inserir usuário
                    await connection.ExecuteAsync(@"
                        INSERT INTO AspNetUsers (
                            Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
                            EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
                            PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd,
                            LockoutEnabled, AccessFailedCount, FirstName, LastName
                        ) VALUES (
                            @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                            @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp,
                            @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd,
                            @LockoutEnabled, @AccessFailedCount, @FirstName, @LastName
                        )",
                        new
                        {
                            Id = userId,
                            UserName = Input.Email,
                            NormalizedUserName = Input.Email.ToUpper(),
                            Email = Input.Email,
                            NormalizedEmail = Input.Email.ToUpper(),
                            EmailConfirmed = true,
                            PasswordHash = passwordHash,
                            SecurityStamp = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                            PhoneNumber = "",
                            PhoneNumberConfirmed = false,
                            TwoFactorEnabled = false,
                            LockoutEnd = (DateTimeOffset?)null,
                            LockoutEnabled = true,
                            AccessFailedCount = 0,
                            FirstName = Input.FirstName,
                            LastName = Input.LastName
                        });

                    // Buscar role USER e adicionar ao usuário (usando a mesma conexão)
                    var userRoleId = await connection.QuerySingleOrDefaultAsync<string>(
                        "SELECT Id FROM AspNetRoles WHERE NormalizedName = 'USER'");

                    if (userRoleId != null)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                            new { UserId = userId, RoleId = userRoleId });
                    }
                    else
                    {
                        _logger.LogWarning("Role USER não encontrada");
                    }

                    _logger.LogInformation("Usuário criado com sucesso: {Email}", Input.Email);

                    // Fazer login automático
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.Name, Input.Email),
                        new Claim(ClaimTypes.Email, Input.Email),
                        new Claim("FirstName", Input.FirstName),
                        new Claim("LastName", Input.LastName)
                    };

                    // Adicionar role User se existir
                    if (userRoleId != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "User"));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(
                        IdentityConstants.ApplicationScheme,
                        claimsPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = false,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                        });

                    return LocalRedirect(returnUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao registrar usuário: {Message}", ex.Message);
                    ModelState.AddModelError(string.Empty, $"Erro ao criar usuário: {ex.Message}");
                }
            }

            return Page();
        }
    }
}
