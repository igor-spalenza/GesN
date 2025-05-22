using GesN.Web.Areas.Identity.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                    // Tentar encontrar o usuário pelo email ou nome de usuário
                    ApplicationUser user = null;
                    
                    // Primeiro tenta encontrar por email
                    if (Input.Email.Contains('@'))
                    {
                        user = await _userManager.FindByEmailAsync(Input.Email);
                    }
                    
                    // Se não encontrou por email, tenta pelo nome de usuário
                    if (user == null)
                    {
                        user = await _userManager.FindByNameAsync(Input.Email);
                    }
                    
                    // Se ainda não encontrou, retorna erro
                    if (user == null)
                    {
                        _logger.LogWarning($"Usuário não encontrado: {Input.Email}");
                        ModelState.AddModelError(string.Empty, "Nome de usuário/Email ou senha inválidos.");
                        return Page();
                    }

                    // Verificar mais informações para debugging
                    string debug = $"Usuário encontrado: Id={user.Id}, UserName={user.UserName}, Email={user.Email}, " +
                                $"EmailConfirmed={user.EmailConfirmed}, LockoutEnabled={user.LockoutEnabled}, " +
                                $"LockoutEnd={user.LockoutEnd}, AccessFailedCount={user.AccessFailedCount}";
                    _logger.LogInformation(debug);
                    DebugInfo = debug;

                    // Importante: Usar o UserName para login
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    
                    // Log detalhado do resultado da tentativa de login
                    _logger.LogInformation($"Resultado do login: Succeeded={result.Succeeded}, RequiresTwoFactor={result.RequiresTwoFactor}, " +
                                        $"IsLockedOut={result.IsLockedOut}, IsNotAllowed={result.IsNotAllowed}");
                    
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    if (result.IsNotAllowed)
                    {
                        _logger.LogWarning("Login não permitido. Verifique se o e-mail foi confirmado.");
                        ModelState.AddModelError(string.Empty, "Login não permitido. Verifique se o e-mail foi confirmado.");
                        return Page();
                    }
                    
                    // Verificar se a senha está correta
                    var passwordCorrect = await _userManager.CheckPasswordAsync(user, Input.Password);
                    if (!passwordCorrect)
                    {
                        _logger.LogWarning("Senha incorreta fornecida.");
                        ModelState.AddModelError(string.Empty, "Nome de usuário/Email ou senha inválidos.");
                    }
                    else
                    {
                        _logger.LogWarning("Senha correta, mas login falhou por outro motivo.");
                        ModelState.AddModelError(string.Empty, "Tentativa de login inválida. Contate o administrador.");
                    }
                    
                    return Page();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro no login: {ex.Message}");
                    ModelState.AddModelError(string.Empty, $"Erro no login: {ex.Message}");
                    return Page();
                }
            }

            return Page();
        }
    }
}
