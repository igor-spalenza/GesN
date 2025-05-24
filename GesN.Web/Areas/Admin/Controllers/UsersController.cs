using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GesN.Web.Areas.Admin.Models;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await GetAllUsersAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await GetUserRolesAsync(user.Id);
                var userClaims = await _userManager.GetClaimsAsync(user);
                
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = string.Join(", ", roles),
                    Claims = userClaims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList()
                });
            }

            return View(userViewModels);
        }

        private async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            const string query = "SELECT * FROM AspNetUsers";
            var users = await _unitOfWork.Connection.QueryAsync<ApplicationUser>(query, transaction: _unitOfWork.Transaction);
            return users.ToList();
        }

        private async Task<List<string>> GetUserRolesAsync(string userId)
        {
            const string query = @"
                SELECT r.Name
                FROM AspNetRoles r
                INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId";

            var roles = await _unitOfWork.Connection.QueryAsync<string>(
                query,
                new { UserId = userId },
                _unitOfWork.Transaction);
            return roles.ToList();
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await GetUserRolesAsync(id);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = string.Join(", ", roles)
            };

            return View(viewModel);
        }

        private async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            const string query = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ApplicationUser>(
                query,
                new { Id = id },
                _unitOfWork.Transaction);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var userClaims = await _userManager.GetClaimsAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                SelectedRoles = userRoles.ToList(),
                AvailableRoles = allRoles,
                Claims = userClaims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList()
            };

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Recarregar dados necessários em caso de erro
                var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                model.AvailableRoles = allRoles;
                model.Claims ??= new List<ClaimViewModel>();
                return PartialView("_Edit", model);
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"Edit Users POST chamado - Id: {model?.Id}, UserName: {model?.UserName}");

                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("Usuário não encontrado");
                    ModelState.AddModelError("", "Usuário não encontrado.");
                    return PartialView("_Edit", model);
                }

                System.Diagnostics.Debug.WriteLine("Usuário encontrado, atualizando dados");

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao atualizar usuário: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return PartialView("_Edit", model);
                }

                System.Diagnostics.Debug.WriteLine("Usuário atualizado, gerenciando roles");

                // Atualizar roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(model.SelectedRoles ?? new List<string>());
                var rolesToAdd = (model.SelectedRoles ?? new List<string>()).Except(currentRoles);

                if (rolesToRemove.Any())
                {
                    result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine("Erro ao remover roles do usuário");
                        ModelState.AddModelError("", "Erro ao remover roles.");
                        return PartialView("_Edit", model);
                    }
                }

                if (rolesToAdd.Any())
                {
                    result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine("Erro ao adicionar roles ao usuário");
                        ModelState.AddModelError("", "Erro ao adicionar roles.");
                        return PartialView("_Edit", model);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Roles atualizadas, gerenciando claims");

                // Atualizar claims
                var currentClaims = await _userManager.GetClaimsAsync(user);
                var claimsToRemove = currentClaims.Where(c => !model.Claims.Any(mc => mc.Type == c.Type && mc.Value == c.Value));
                var claimsToAdd = model.Claims.Where(mc => !currentClaims.Any(c => c.Type == mc.Type && c.Value == mc.Value))
                                             .Select(c => new Claim(c.Type, c.Value));

                foreach (var claim in claimsToRemove)
                {
                    result = await _userManager.RemoveClaimAsync(user, claim);
                    if (!result.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao remover claim do usuário: {claim.Type}={claim.Value}");
                        ModelState.AddModelError("", "Erro ao remover claims.");
                        return PartialView("_Edit", model);
                    }
                }

                foreach (var claim in claimsToAdd)
                {
                    result = await _userManager.AddClaimAsync(user, claim);
                    if (!result.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim ao usuário: {claim.Type}={claim.Value}");
                        ModelState.AddModelError("", "Erro ao adicionar claims.");
                        return PartialView("_Edit", model);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Edição de usuário completa com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no Edit Users: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Ocorreu um erro ao salvar as alterações: " + ex.Message);
                return PartialView("_Edit", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await GetUserRolesAsync(id);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = string.Join(", ", roles)
            };

            return PartialView("_Delete", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DeleteConfirmed Users chamado - Id: {id}");

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("Usuário não encontrado para exclusão");
                    return NotFound();
                }

                System.Diagnostics.Debug.WriteLine("Usuário encontrado, excluindo");

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    System.Diagnostics.Debug.WriteLine($"Erro ao excluir usuário: {errorMessage}");
                    throw new InvalidOperationException($"Erro ao excluir usuário: {errorMessage}");
                }

                System.Diagnostics.Debug.WriteLine("Usuário excluído com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no DeleteConfirmed Users: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await GetUserRolesAsync(id);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = string.Join(", ", roles),
                Claims = userClaims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList()
            };

            return PartialView("_Details", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePartial()
        {
            var roles = _roleManager.Roles.ToList();
            var model = new CreateUserViewModel
            {
                AvailableRoles = roles.Select(r => new RoleSelectionViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsSelected = false
                }).ToList(),
                Claims = new List<ClaimViewModel>()
            };

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial([FromForm] CreateUserViewModel model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"CreatePartial Users POST chamado - UserName: {model?.UserName}");
                
                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState inválido na criação de usuário");
                    // Recarregar as roles disponíveis em caso de erro
                    const string query = "SELECT * FROM AspNetRoles";
                    var roles = await _unitOfWork.Connection.QueryAsync<ApplicationRole>(query, transaction: _unitOfWork.Transaction);
                
                    model.AvailableRoles = roles.Select(r => new RoleSelectionViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        IsSelected = model.SelectedRoles?.Contains(r.Name) ?? false
                    }).ToList();

                    // Garantir que Claims não seja null
                    model.Claims ??= new List<ClaimViewModel>();

                    return PartialView("_Create", model);
                }

                System.Diagnostics.Debug.WriteLine("ModelState válido, criando usuário");

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao criar usuário: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Json(new { success = false, message = "Erro ao criar usuário", errors = errors });
                }

                System.Diagnostics.Debug.WriteLine("Usuário criado, adicionando roles");

                // Adicionar roles selecionadas
                var selectedRoles = model.AvailableRoles.Where(r => r.IsSelected).Select(r => r.Name).ToList();
                if (selectedRoles.Any())
                {
                    result = await _userManager.AddToRolesAsync(user, selectedRoles);
                    if (!result.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine("Erro ao adicionar roles ao usuário");
                        await _userManager.DeleteAsync(user);
                        var errors = result.Errors.Select(e => e.Description).ToList();
                        return Json(new { success = false, message = "Erro ao adicionar roles", errors = errors });
                    }
                }

                System.Diagnostics.Debug.WriteLine("Roles adicionadas, adicionando claims");

                // Adicionar claims
                if (model.Claims?.Any() == true)
                {
                    var claims = model.Claims
                        .Where(c => !string.IsNullOrEmpty(c.Type) && !string.IsNullOrEmpty(c.Value))
                        .Select(c => new Claim(c.Type, c.Value));

                    foreach (var claim in claims)
                    {
                        result = await _userManager.AddClaimAsync(user, claim);
                        if (!result.Succeeded)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim ao usuário: {claim.Type}={claim.Value}");
                            await _userManager.DeleteAsync(user);
                            var errors = result.Errors.Select(e => e.Description).ToList();
                            return Json(new { success = false, message = "Erro ao adicionar claims", errors = errors });
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Criação de usuário completa com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no CreatePartial Users: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Erro ao criar usuário: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GridPartial()
        {
            try
            {
                // Usar apenas UserManager para evitar problemas com transações
                var users = _userManager.Users.ToList();
                var userViewModels = new List<UserViewModel>();

                foreach (var user in users)
                {
                    try
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var userClaims = await _userManager.GetClaimsAsync(user);
                        
                        userViewModels.Add(new UserViewModel
                        {
                            Id = user.Id,
                            Email = user.Email ?? "",
                            UserName = user.UserName ?? "",
                            FirstName = user.FirstName ?? "",
                            LastName = user.LastName ?? "",
                            PhoneNumber = user.PhoneNumber ?? "",
                            Roles = string.Join(", ", roles),
                            Claims = userClaims?.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList() ?? new List<ClaimViewModel>()
                        });
                    }
                    catch (Exception userEx)
                    {
                        // Se houver erro com um usuário específico, pular para o próximo
                        System.Diagnostics.Debug.WriteLine($"Erro ao processar usuário {user.Id}: {userEx.Message}");
                        continue;
                    }
                }

                return PartialView("_Grid", userViewModels);
            }
            catch (Exception ex)
            {
                // Log do erro para debug
                System.Diagnostics.Debug.WriteLine($"Erro no GridPartial: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar grid de usuários: {ex.Message}");
            }
        }
    }
} 