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
    public class RolesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await GetAllRolesAsync();
            var roleViewModels = new List<RoleViewModel>();

            foreach (var role in roles)
            {
                var users = await GetRoleUsersAsync(role.Id);
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                
                roleViewModels.Add(new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    NormalizedName = role.NormalizedName,
                    Users = string.Join(", ", users.Select(u => u.UserName)),
                    UserCount = users.Count,
                    Claims = roleClaims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList()
                });
            }

            return View(roleViewModels);
        }

        private async Task<List<ApplicationRole>> GetAllRolesAsync()
        {
            const string query = "SELECT * FROM AspNetRoles";
            var roles = await _unitOfWork.Connection.QueryAsync<ApplicationRole>(query, transaction: _unitOfWork.Transaction);
            return roles.ToList();
        }

        private async Task<List<ApplicationUser>> GetRoleUsersAsync(string roleId)
        {
            const string query = @"
                SELECT u.*
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                WHERE ur.RoleId = @RoleId";

            var users = await _unitOfWork.Connection.QueryAsync<ApplicationUser>(
                query,
                new { RoleId = roleId },
                _unitOfWork.Transaction);
            return users.ToList();
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var users = await GetRoleUsersAsync(id);
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            
            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                Users = string.Join(", ", users.Select(u => u.UserName)),
                UserCount = users.Count,
                Claims = roleClaims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList()
            };

            return View(viewModel);
        }

        private async Task<ApplicationRole> GetRoleByIdAsync(string id)
        {
            const string query = "SELECT * FROM AspNetRoles WHERE Id = @Id";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ApplicationRole>(
                query,
                new { Id = id },
                _unitOfWork.Transaction);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var associatedUsers = await GetRoleUsersAsync(id);

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                Claims = roleClaims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList(),
                AssociatedUsers = associatedUsers.Select(u => new UserSelectionViewModel 
                { 
                    Id = u.Id, 
                    UserName = u.UserName, 
                    Email = u.Email, 
                    IsSelected = true 
                }).ToList()
            };

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Edit POST chamado - Id: {model?.Id}, Name: {model?.Name}");
                
                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState inválido na edição");
                    // Recarregar dados necessários em caso de erro
                    model.Claims ??= new List<ClaimViewModel>();
                    model.AssociatedUsers ??= new List<UserSelectionViewModel>();
                    return PartialView("_Edit", model);
                }

                System.Diagnostics.Debug.WriteLine("ModelState válido, buscando role");

                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    System.Diagnostics.Debug.WriteLine("Role não encontrada");
                    ModelState.AddModelError("", "Role não encontrada.");
                    return PartialView("_Edit", model);
                }

                System.Diagnostics.Debug.WriteLine("Role encontrada, atualizando dados");

                role.Name = model.Name;
                role.NormalizedName = model.Name.ToUpper();

                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao atualizar role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return PartialView("_Edit", model);
                }

                System.Diagnostics.Debug.WriteLine("Role atualizada, gerenciando claims");

                // Atualizar claims
                var currentClaims = await _roleManager.GetClaimsAsync(role);
                var claimsToRemove = currentClaims.Where(c => !model.Claims.Any(mc => mc.Type == c.Type && mc.Value == c.Value));
                var claimsToAdd = model.Claims.Where(mc => !currentClaims.Any(c => c.Type == mc.Type && c.Value == mc.Value))
                                             .Select(c => new Claim(c.Type, c.Value));

                foreach (var claim in claimsToRemove)
                {
                    result = await _roleManager.RemoveClaimAsync(role, claim);
                    if (!result.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao remover claim: {claim.Type}={claim.Value}");
                        ModelState.AddModelError("", "Erro ao remover claims.");
                        return PartialView("_Edit", model);
                    }
                }

                foreach (var claim in claimsToAdd)
                {
                    result = await _roleManager.AddClaimAsync(role, claim);
                    if (!result.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim: {claim.Type}={claim.Value}");
                        ModelState.AddModelError("", "Erro ao adicionar claims.");
                        return PartialView("_Edit", model);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Edição completa com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no Edit: {ex.Message}");
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

            var role = await GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var users = await GetRoleUsersAsync(id);
            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                Users = string.Join(", ", users.Select(u => u.UserName)),
                UserCount = users.Count
            };

            return PartialView("_Delete", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DeleteConfirmed chamado - Id: {id}");

                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    System.Diagnostics.Debug.WriteLine("Role não encontrada para exclusão");
                    return NotFound();
                }

                System.Diagnostics.Debug.WriteLine("Role encontrada, verificando usuários associados");

                // Verificar se a role possui usuários associados
                var users = await GetRoleUsersAsync(id);
                if (users.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"Role possui {users.Count} usuários associados");
                    return Json(new { success = false, message = "Não é possível excluir uma role que possui usuários associados. Remova todos os usuários desta role antes de excluí-la." });
                }

                System.Diagnostics.Debug.WriteLine("Nenhum usuário associado, excluindo role");

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    System.Diagnostics.Debug.WriteLine($"Erro ao excluir role: {errorMessage}");
                    throw new InvalidOperationException($"Erro ao excluir role: {errorMessage}");
                }

                System.Diagnostics.Debug.WriteLine("Role excluída com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no DeleteConfirmed: {ex.Message}");
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

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var users = await GetRoleUsersAsync(id);
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                Users = string.Join(", ", users.Select(u => u.UserName)),
                UserCount = users.Count,
                Claims = roleClaims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList()
            };

            return PartialView("_Details", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePartial()
        {
            var model = new CreateRoleViewModel
            {
                Claims = new List<ClaimViewModel>()
            };

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial([FromForm] CreateRoleViewModel model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"CreatePartial POST chamado - Name: {model?.Name}");
                
                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState inválido");
                    // Garantir que Claims não seja null
                    model.Claims ??= new List<ClaimViewModel>();
                    return PartialView("_Create", model);
                }

                System.Diagnostics.Debug.WriteLine("ModelState válido, verificando se role já existe");

                // Verificar se a role já existe
                var existingRole = await _roleManager.FindByNameAsync(model.Name);
                if (existingRole != null)
                {
                    System.Diagnostics.Debug.WriteLine("Role já existe");
                    return Json(new { success = false, message = "Uma role com este nome já existe." });
                }

                System.Diagnostics.Debug.WriteLine("Criando nova role");

                var role = new ApplicationRole
                {
                    Name = model.Name,
                    NormalizedName = model.Name.ToUpper()
                };

                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao criar role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Json(new { success = false, message = "Erro ao criar role", errors = errors });
                }

                System.Diagnostics.Debug.WriteLine("Role criada com sucesso");

                // Adicionar claims
                if (model.Claims?.Any() == true)
                {
                    System.Diagnostics.Debug.WriteLine($"Adicionando {model.Claims.Count} claims");
                    var claims = model.Claims
                        .Where(c => !string.IsNullOrEmpty(c.Type) && !string.IsNullOrEmpty(c.Value))
                        .Select(c => new Claim(c.Type, c.Value));

                    foreach (var claim in claims)
                    {
                        result = await _roleManager.AddClaimAsync(role, claim);
                        if (!result.Succeeded)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim: {claim.Type}={claim.Value}");
                            await _roleManager.DeleteAsync(role);
                            var errors = result.Errors.Select(e => e.Description).ToList();
                            return Json(new { success = false, message = "Erro ao adicionar claims", errors = errors });
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Operação completa com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no CreatePartial: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Erro ao criar role: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GridPartial()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
                var roleViewModels = new List<RoleViewModel>();

                foreach (var role in roles)
                {
                    try
                    {
                        var users = await GetRoleUsersAsync(role.Id);
                        var roleClaims = await _roleManager.GetClaimsAsync(role);
                        
                        roleViewModels.Add(new RoleViewModel
                        {
                            Id = role.Id,
                            Name = role.Name ?? "",
                            NormalizedName = role.NormalizedName ?? "",
                            Users = string.Join(", ", users.Select(u => u.UserName)),
                            UserCount = users.Count,
                            Claims = roleClaims?.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList() ?? new List<ClaimViewModel>()
                        });
                    }
                    catch (Exception roleEx)
                    {
                        // Se houver erro com uma role específica, pular para a próxima
                        System.Diagnostics.Debug.WriteLine($"Erro ao processar role {role.Id}: {roleEx.Message}");
                        continue;
                    }
                }

                return PartialView("_Grid", roleViewModels);
            }
            catch (Exception ex)
            {
                // Log do erro para debug
                System.Diagnostics.Debug.WriteLine($"Erro no GridPartial: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar grid de roles: {ex.Message}");
            }
        }
    }
} 