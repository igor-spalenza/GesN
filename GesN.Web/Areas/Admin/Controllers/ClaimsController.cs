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
    public class ClaimsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ClaimsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var allClaims = await GetAllDistinctClaimsAsync();
            var claimViewModels = new List<ClaimViewModel>();

            foreach (var claim in allClaims)
            {
                var usersWithClaim = await GetUsersWithClaimAsync(claim.Type, claim.Value);
                var rolesWithClaim = await GetRolesWithClaimAsync(claim.Type, claim.Value);
                
                claimViewModels.Add(new ClaimViewModel
                {
                    Type = claim.Type,
                    Value = claim.Value,
                    Users = string.Join(", ", usersWithClaim.Select(u => u.UserName)),
                    Roles = string.Join(", ", rolesWithClaim.Select(r => r.Name)),
                    UserCount = usersWithClaim.Count,
                    RoleCount = rolesWithClaim.Count
                });
            }

            return View(claimViewModels);
        }

        private async Task<List<(string Type, string Value)>> GetAllDistinctClaimsAsync()
        {
            const string query = @"
                SELECT DISTINCT ClaimType as Type, ClaimValue as Value
                FROM AspNetUserClaims
                UNION
                SELECT DISTINCT ClaimType as Type, ClaimValue as Value
                FROM AspNetRoleClaims";

            var claims = await _unitOfWork.Connection.QueryAsync<(string Type, string Value)>(query, transaction: _unitOfWork.Transaction);
            return claims.ToList();
        }

        private async Task<List<ApplicationUser>> GetUsersWithClaimAsync(string type, string value)
        {
            const string query = @"
                SELECT u.*
                FROM AspNetUsers u
                INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                WHERE uc.ClaimType = @Type AND uc.ClaimValue = @Value";

            var users = await _unitOfWork.Connection.QueryAsync<ApplicationUser>(
                query,
                new { Type = type, Value = value },
                _unitOfWork.Transaction);
            return users.ToList();
        }

        private async Task<List<ApplicationRole>> GetRolesWithClaimAsync(string type, string value)
        {
            const string query = @"
                SELECT r.*
                FROM AspNetRoles r
                INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                WHERE rc.ClaimType = @Type AND rc.ClaimValue = @Value";

            var roles = await _unitOfWork.Connection.QueryAsync<ApplicationRole>(
                query,
                new { Type = type, Value = value },
                _unitOfWork.Transaction);
            return roles.ToList();
        }

        public async Task<IActionResult> Details(string type, string value)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value))
            {
                return NotFound();
            }

            var usersWithClaim = await GetUsersWithClaimAsync(type, value);
            var rolesWithClaim = await GetRolesWithClaimAsync(type, value);
            
            var viewModel = new ClaimDetailViewModel
            {
                Type = type,
                Value = value,
                UsersWithClaim = usersWithClaim.Select(u => new UserSelectionViewModel 
                { 
                    Id = u.Id, 
                    UserName = u.UserName, 
                    Email = u.Email, 
                    IsSelected = true 
                }).ToList(),
                RolesWithClaim = rolesWithClaim.Select(r => new RoleSelectionClaimViewModel 
                { 
                    Id = r.Id, 
                    Name = r.Name, 
                    IsSelected = true 
                }).ToList(),
                TotalUsers = usersWithClaim.Count,
                TotalRoles = rolesWithClaim.Count
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string type, string value)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value))
                return NotFound();

            var usersWithClaim = await GetUsersWithClaimAsync(type, value);
            var rolesWithClaim = await GetRolesWithClaimAsync(type, value);

            var model = new EditClaimViewModel
            {
                Type = type,
                Value = value,
                AssociatedUsers = usersWithClaim.Select(u => new UserSelectionViewModel 
                { 
                    Id = u.Id, 
                    UserName = u.UserName, 
                    Email = u.Email, 
                    IsSelected = true 
                }).ToList(),
                AssociatedRoles = rolesWithClaim.Select(r => new RoleSelectionClaimViewModel 
                { 
                    Id = r.Id, 
                    Name = r.Name, 
                    IsSelected = true 
                }).ToList()
            };

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditClaimViewModel model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Edit Claims POST chamado - Type: {model?.Type}, Value: {model?.Value}");
                
                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState inválido na edição de claims");
                    // Recarregar dados necessários em caso de erro
                    model.AssociatedUsers ??= new List<UserSelectionViewModel>();
                    model.AssociatedRoles ??= new List<RoleSelectionClaimViewModel>();
                    return PartialView("_Edit", model);
                }

                System.Diagnostics.Debug.WriteLine("ModelState válido, atualizando claims para usuários");

                // Atualizar claims para usuários
                var currentUsersWithClaim = await GetUsersWithClaimAsync(model.Type, model.Value);
                var selectedUserIds = model.AssociatedUsers.Where(u => u.IsSelected).Select(u => u.Id).ToList();
                var usersToRemove = currentUsersWithClaim.Where(u => !selectedUserIds.Contains(u.Id));
                var userIdsToAdd = selectedUserIds.Where(id => !currentUsersWithClaim.Any(u => u.Id == id));

                foreach (var user in usersToRemove)
                {
                    var userEntity = await _userManager.FindByIdAsync(user.Id);
                    if (userEntity != null)
                    {
                        var result = await _userManager.RemoveClaimAsync(userEntity, new Claim(model.Type, model.Value));
                        if (!result.Succeeded)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao remover claim do usuário {user.Id}");
                            ModelState.AddModelError("", "Erro ao remover claim do usuário.");
                            return PartialView("_Edit", model);
                        }
                    }
                }

                foreach (var userId in userIdsToAdd)
                {
                    var userEntity = await _userManager.FindByIdAsync(userId);
                    if (userEntity != null)
                    {
                        var result = await _userManager.AddClaimAsync(userEntity, new Claim(model.Type, model.Value));
                        if (!result.Succeeded)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim ao usuário {userId}");
                            ModelState.AddModelError("", "Erro ao adicionar claim ao usuário.");
                            return PartialView("_Edit", model);
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Claims de usuários atualizadas, atualizando claims para roles");

                // Atualizar claims para roles
                var currentRolesWithClaim = await GetRolesWithClaimAsync(model.Type, model.Value);
                var selectedRoleIds = model.AssociatedRoles.Where(r => r.IsSelected).Select(r => r.Id).ToList();
                var rolesToRemove = currentRolesWithClaim.Where(r => !selectedRoleIds.Contains(r.Id));
                var roleIdsToAdd = selectedRoleIds.Where(id => !currentRolesWithClaim.Any(r => r.Id == id));

                foreach (var role in rolesToRemove)
                {
                    var roleEntity = await _roleManager.FindByIdAsync(role.Id);
                    if (roleEntity != null)
                    {
                        var result = await _roleManager.RemoveClaimAsync(roleEntity, new Claim(model.Type, model.Value));
                        if (!result.Succeeded)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao remover claim da role {role.Id}");
                            ModelState.AddModelError("", "Erro ao remover claim da role.");
                            return PartialView("_Edit", model);
                        }
                    }
                }

                foreach (var roleId in roleIdsToAdd)
                {
                    var roleEntity = await _roleManager.FindByIdAsync(roleId);
                    if (roleEntity != null)
                    {
                        var result = await _roleManager.AddClaimAsync(roleEntity, new Claim(model.Type, model.Value));
                        if (!result.Succeeded)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim à role {roleId}");
                            ModelState.AddModelError("", "Erro ao adicionar claim à role.");
                            return PartialView("_Edit", model);
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Edição de claims completa com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no Edit Claims: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Ocorreu um erro ao salvar as alterações: " + ex.Message);
                return PartialView("_Edit", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string type, string value)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value))
            {
                return NotFound();
            }

            var usersWithClaim = await GetUsersWithClaimAsync(type, value);
            var rolesWithClaim = await GetRolesWithClaimAsync(type, value);
            
            var viewModel = new ClaimViewModel
            {
                Type = type,
                Value = value,
                Users = string.Join(", ", usersWithClaim.Select(u => u.UserName)),
                Roles = string.Join(", ", rolesWithClaim.Select(r => r.Name)),
                UserCount = usersWithClaim.Count,
                RoleCount = rolesWithClaim.Count
            };

            return PartialView("_Delete", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string type, string value)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DeleteConfirmed Claims chamado - Type: {type}, Value: {value}");

                // Remover claim de todos os usuários
                var usersWithClaim = await GetUsersWithClaimAsync(type, value);
                System.Diagnostics.Debug.WriteLine($"Removendo claim de {usersWithClaim.Count} usuários");
                
                foreach (var user in usersWithClaim)
                {
                    var userEntity = await _userManager.FindByIdAsync(user.Id);
                    if (userEntity != null)
                    {
                        var result = await _userManager.RemoveClaimAsync(userEntity, new Claim(type, value));
                        if (!result.Succeeded)
                        {
                            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                            System.Diagnostics.Debug.WriteLine($"Erro ao remover claim do usuário {user.Id}: {errorMessage}");
                            throw new InvalidOperationException($"Erro ao remover claim do usuário: {errorMessage}");
                        }
                    }
                }

                // Remover claim de todas as roles
                var rolesWithClaim = await GetRolesWithClaimAsync(type, value);
                System.Diagnostics.Debug.WriteLine($"Removendo claim de {rolesWithClaim.Count} roles");
                
                foreach (var role in rolesWithClaim)
                {
                    var roleEntity = await _roleManager.FindByIdAsync(role.Id);
                    if (roleEntity != null)
                    {
                        var result = await _roleManager.RemoveClaimAsync(roleEntity, new Claim(type, value));
                        if (!result.Succeeded)
                        {
                            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                            System.Diagnostics.Debug.WriteLine($"Erro ao remover claim da role {role.Id}: {errorMessage}");
                            throw new InvalidOperationException($"Erro ao remover claim da role: {errorMessage}");
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Claim excluída com sucesso de todos os usuários e roles");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no DeleteConfirmed Claims: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string type, string value)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value))
            {
                return NotFound();
            }

            var usersWithClaim = await GetUsersWithClaimAsync(type, value);
            var rolesWithClaim = await GetRolesWithClaimAsync(type, value);

            var viewModel = new ClaimDetailViewModel
            {
                Type = type,
                Value = value,
                UsersWithClaim = usersWithClaim.Select(u => new UserSelectionViewModel 
                { 
                    Id = u.Id, 
                    UserName = u.UserName, 
                    Email = u.Email, 
                    IsSelected = true 
                }).ToList(),
                RolesWithClaim = rolesWithClaim.Select(r => new RoleSelectionClaimViewModel 
                { 
                    Id = r.Id, 
                    Name = r.Name, 
                    IsSelected = true 
                }).ToList(),
                TotalUsers = usersWithClaim.Count,
                TotalRoles = rolesWithClaim.Count
            };

            return PartialView("_Details", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePartial()
        {
            var allUsers = _userManager.Users.ToList();
            var allRoles = _roleManager.Roles.ToList();

            var model = new CreateClaimViewModel
            {
                AvailableUsers = allUsers.Select(u => new UserSelectionViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    IsSelected = false
                }).ToList(),
                AvailableRoles = allRoles.Select(r => new RoleSelectionClaimViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsSelected = false
                }).ToList()
            };

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial([FromForm] CreateClaimViewModel model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"CreatePartial Claims POST chamado - Type: {model?.Type}, Value: {model?.Value}");
                
                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState inválido na criação de claims");
                    // Recarregar dados necessários em caso de erro
                    var allUsers = _userManager.Users.ToList();
                    var allRoles = _roleManager.Roles.ToList();

                    model.AvailableUsers = allUsers.Select(u => new UserSelectionViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        IsSelected = model.SelectedUsers?.Contains(u.Id) ?? false
                    }).ToList();

                    model.AvailableRoles = allRoles.Select(r => new RoleSelectionClaimViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        IsSelected = model.SelectedRoles?.Contains(r.Id) ?? false
                    }).ToList();

                    return PartialView("_Create", model);
                }

                System.Diagnostics.Debug.WriteLine("ModelState válido, criando claim");

                var claim = new Claim(model.Type, model.Value);

                // Adicionar claim aos usuários selecionados
                if (model.SelectedUsers?.Any() == true)
                {
                    System.Diagnostics.Debug.WriteLine($"Adicionando claim a {model.SelectedUsers.Count} usuários");
                    foreach (var userId in model.SelectedUsers)
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            var existingUserClaims = await _userManager.GetClaimsAsync(user);
                            if (!existingUserClaims.Any(c => c.Type == model.Type && c.Value == model.Value))
                            {
                                var result = await _userManager.AddClaimAsync(user, claim);
                                if (!result.Succeeded)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim ao usuário {userId}");
                                    var errors = result.Errors.Select(e => e.Description).ToList();
                                    return Json(new { success = false, message = "Erro ao adicionar claim ao usuário", errors = errors });
                                }
                            }
                        }
                    }
                }

                // Adicionar claim às roles selecionadas
                if (model.SelectedRoles?.Any() == true)
                {
                    System.Diagnostics.Debug.WriteLine($"Adicionando claim a {model.SelectedRoles.Count} roles");
                    foreach (var roleId in model.SelectedRoles)
                    {
                        var role = await _roleManager.FindByIdAsync(roleId);
                        if (role != null)
                        {
                            var existingRoleClaims = await _roleManager.GetClaimsAsync(role);
                            if (!existingRoleClaims.Any(c => c.Type == model.Type && c.Value == model.Value))
                            {
                                var result = await _roleManager.AddClaimAsync(role, claim);
                                if (!result.Succeeded)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim à role {roleId}");
                                    var errors = result.Errors.Select(e => e.Description).ToList();
                                    return Json(new { success = false, message = "Erro ao adicionar claim à role", errors = errors });
                                }
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Criação de claim completa com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no CreatePartial Claims: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Erro ao criar claim: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GridPartial()
        {
            try
            {
                var allClaims = await GetAllDistinctClaimsAsync();
                var claimViewModels = new List<ClaimViewModel>();

                foreach (var claim in allClaims)
                {
                    try
                    {
                        var usersWithClaim = await GetUsersWithClaimAsync(claim.Type, claim.Value);
                        var rolesWithClaim = await GetRolesWithClaimAsync(claim.Type, claim.Value);
                        
                        claimViewModels.Add(new ClaimViewModel
                        {
                            Type = claim.Type ?? "",
                            Value = claim.Value ?? "",
                            Users = string.Join(", ", usersWithClaim.Select(u => u.UserName)),
                            Roles = string.Join(", ", rolesWithClaim.Select(r => r.Name)),
                            UserCount = usersWithClaim.Count,
                            RoleCount = rolesWithClaim.Count
                        });
                    }
                    catch (Exception claimEx)
                    {
                        // Se houver erro com uma claim específica, pular para a próxima
                        System.Diagnostics.Debug.WriteLine($"Erro ao processar claim {claim.Type}:{claim.Value}: {claimEx.Message}");
                        continue;
                    }
                }

                return PartialView("_Grid", claimViewModels);
            }
            catch (Exception ex)
            {
                // Log do erro para debug
                System.Diagnostics.Debug.WriteLine($"Erro no GridPartial: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar grid de claims: {ex.Message}");
            }
        }
    }
} 