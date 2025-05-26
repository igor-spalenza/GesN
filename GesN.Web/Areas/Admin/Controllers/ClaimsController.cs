using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GesN.Web.Areas.Admin.Models;
using System.Data;
using GesN.Web.Infrastructure.Data;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ClaimsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ClaimsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Obter todas as claims únicas (tanto de usuários quanto de roles)
                var allClaims = await connection.QueryAsync(@"
                    SELECT DISTINCT ClaimType, ClaimValue 
                    FROM (
                        SELECT ClaimType, ClaimValue FROM AspNetUserClaims
                        UNION
                        SELECT ClaimType, ClaimValue FROM AspNetRoleClaims
                    ) AS AllClaims
                    ORDER BY ClaimType, ClaimValue");

                var claimViewModels = new List<ClaimViewModel>();

                foreach (var claim in allClaims)
                {
                    var claimType = (string)claim.ClaimType;
                    var claimValue = (string)claim.ClaimValue;
                    
                    // Obter usuários com esta claim
                    var usersWithClaim = await connection.QueryAsync(@"
                        SELECT u.Id, u.UserName, u.Email 
                        FROM AspNetUsers u
                        INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                        WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue", 
                        new { ClaimType = claimType, ClaimValue = claimValue });
                    
                    // Obter roles com esta claim
                    var rolesWithClaim = await connection.QueryAsync(@"
                        SELECT r.Id, r.Name 
                        FROM AspNetRoles r
                        INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                        WHERE rc.ClaimType = @ClaimType AND rc.ClaimValue = @ClaimValue", 
                        new { ClaimType = claimType, ClaimValue = claimValue });
                
                    var usersList = usersWithClaim.ToList();
                    var rolesList = rolesWithClaim.ToList();
                    
                    var userNames = new List<string>();
                    foreach (var user in usersList)
                    {
                        userNames.Add((string)user.UserName);
                    }
                    
                    var roleNames = new List<string>();
                    foreach (var role in rolesList)
                    {
                        roleNames.Add((string)role.Name);
                    }
                    
                    claimViewModels.Add(new ClaimViewModel
                    {
                        Type = claimType,
                        Value = claimValue,
                        Users = string.Join(", ", userNames),
                        Roles = string.Join(", ", roleNames),
                        UserCount = usersList.Count,
                        RoleCount = rolesList.Count
                    });
                }

                return View(claimViewModels);
            }
            catch (Exception ex)
            {
                // Log do erro para debug
                System.Diagnostics.Debug.WriteLine($"Erro no Index: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar claims: {ex.Message}");
            }
        }

        public async Task<IActionResult> Details(string type, string value)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value))
            {
                return NotFound();
            }

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Obter usuários com esta claim
                var usersWithClaim = await connection.QueryAsync(@"
                    SELECT u.Id, u.UserName, u.Email 
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                    WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
                
                // Obter roles com esta claim
                var rolesWithClaim = await connection.QueryAsync(@"
                    SELECT r.Id, r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                    WHERE rc.ClaimType = @ClaimType AND rc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
            
                var usersList = usersWithClaim.ToList();
                var rolesList = rolesWithClaim.ToList();
                
                var usersViewModels = new List<UserSelectionViewModel>();
                foreach (var user in usersList)
                {
                    usersViewModels.Add(new UserSelectionViewModel 
                    { 
                        Id = (string)user.Id, 
                        UserName = (string)user.UserName, 
                        Email = (string)user.Email, 
                        IsSelected = true 
                    });
                }
                
                var rolesViewModels = new List<RoleSelectionClaimViewModel>();
                foreach (var role in rolesList)
                {
                    rolesViewModels.Add(new RoleSelectionClaimViewModel 
                    { 
                        Id = (string)role.Id, 
                        Name = (string)role.Name, 
                        IsSelected = true 
                    });
                }
                
                var viewModel = new ClaimDetailViewModel
                {
                    Type = type,
                    Value = value,
                    UsersWithClaim = usersViewModels,
                    RolesWithClaim = rolesViewModels,
                    TotalUsers = usersList.Count,
                    TotalRoles = rolesList.Count
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Details: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar detalhes da claim: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string type, string value)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value))
                return NotFound();

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Obter todos os usuários e roles do sistema
                var allUsers = await connection.QueryAsync(@"
                    SELECT Id, UserName, Email FROM AspNetUsers ORDER BY UserName");
                var allRoles = await connection.QueryAsync(@"
                    SELECT Id, Name FROM AspNetRoles ORDER BY Name");
                
                // Obter usuários que já possuem esta claim
                var usersWithClaim = await connection.QueryAsync(@"
                    SELECT u.Id, u.UserName, u.Email 
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                    WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
                
                // Obter roles que já possuem esta claim
                var rolesWithClaim = await connection.QueryAsync(@"
                    SELECT r.Id, r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                    WHERE rc.ClaimType = @ClaimType AND rc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
            
            var usersList = usersWithClaim.ToList();
            var rolesList = rolesWithClaim.ToList();
            
            var userIdsWithClaim = new List<string>();
            foreach (var user in usersList)
            {
                userIdsWithClaim.Add((string)user.Id);
            }
            var userIdsWithClaimSet = userIdsWithClaim.ToHashSet();
            
            var roleIdsWithClaim = new List<string>();
            foreach (var role in rolesList)
            {
                roleIdsWithClaim.Add((string)role.Id);
            }
            var roleIdsWithClaimSet = roleIdsWithClaim.ToHashSet();

            var availableUsers = new List<UserSelectionViewModel>();
            foreach (var user in allUsers)
            {
                availableUsers.Add(new UserSelectionViewModel
                { 
                    Id = (string)user.Id, 
                    UserName = (string)user.UserName, 
                    Email = (string)user.Email, 
                    IsSelected = userIdsWithClaimSet.Contains((string)user.Id)
                });
            }
            
            var availableRoles = new List<RoleSelectionClaimViewModel>();
            foreach (var role in allRoles)
            {
                availableRoles.Add(new RoleSelectionClaimViewModel
                { 
                    Id = (string)role.Id, 
                    Name = (string)role.Name, 
                    IsSelected = roleIdsWithClaimSet.Contains((string)role.Id)
                });
            }
            
            var associatedUsers = new List<UserSelectionViewModel>();
            foreach (var user in usersList)
            {
                associatedUsers.Add(new UserSelectionViewModel 
                { 
                    Id = (string)user.Id, 
                    UserName = (string)user.UserName, 
                    Email = (string)user.Email, 
                    IsSelected = true 
                });
            }
            
            var associatedRoles = new List<RoleSelectionClaimViewModel>();
            foreach (var role in rolesList)
            {
                associatedRoles.Add(new RoleSelectionClaimViewModel 
                { 
                    Id = (string)role.Id, 
                    Name = (string)role.Name, 
                    IsSelected = true 
                });
            }

                var model = new EditClaimViewModel
                {
                    Type = type,
                    Value = value,
                    AvailableUsers = availableUsers,
                    AvailableRoles = availableRoles,
                    SelectedUserIds = userIdsWithClaim,
                    SelectedRoleIds = roleIdsWithClaim,
                    AssociatedUsers = associatedUsers,
                    AssociatedRoles = associatedRoles
                };

                return PartialView("_Edit", model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Edit GET: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar dados para edição: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditClaimViewModel model)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                System.Diagnostics.Debug.WriteLine($"Edit Claims POST chamado - Type: {model?.Type}, Value: {model?.Value}");
                
                // Inicializar listas se forem null
                model.SelectedUserIds ??= new List<string>();
                model.SelectedRoleIds ??= new List<string>();
                
                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState inválido na edição de claims");
                    
                    // Recarregar todos os dados em caso de erro
                    var allUsers = await connection.QueryAsync(@"
                        SELECT Id, UserName, Email FROM AspNetUsers ORDER BY UserName");
                    var allRoles = await connection.QueryAsync(@"
                        SELECT Id, Name FROM AspNetRoles ORDER BY Name");
                    
                    model.AvailableUsers = allUsers.Select(u => new UserSelectionViewModel
                    {
                        Id = (string)u.Id,
                        UserName = (string)u.UserName,
                        Email = (string)u.Email,
                        IsSelected = model.SelectedUserIds?.Contains((string)u.Id) ?? false
                    }).ToList();

                    model.AvailableRoles = allRoles.Select(r => new RoleSelectionClaimViewModel
                    {
                        Id = (string)r.Id,
                        Name = (string)r.Name,
                        IsSelected = model.SelectedRoleIds?.Contains((string)r.Id) ?? false
                    }).ToList();
                    
                    return PartialView("_Edit", model);
                }

                System.Diagnostics.Debug.WriteLine("ModelState válido, atualizando claims para usuários");

                // Atualizar claims para usuários
                var currentUsersWithClaim = await connection.QueryAsync(@"
                    SELECT u.Id, u.UserName, u.Email 
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                    WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue", 
                    new { ClaimType = model.Type, ClaimValue = model.Value });
                
                var selectedUserIds = model.SelectedUserIds;
                var usersToRemove = currentUsersWithClaim.Where(u => !selectedUserIds.Contains((string)u.Id));
                var userIdsToAdd = selectedUserIds.Where(id => !currentUsersWithClaim.Any(u => u.Id == id));

                foreach (var user in usersToRemove)
                {
                    var result = await connection.ExecuteAsync(@"
                        DELETE FROM AspNetUserClaims 
                        WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue", 
                        new { UserId = user.Id, ClaimType = model.Type, ClaimValue = model.Value });
                    if (result == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao remover claim do usuário {(string)user.Id}");
                        ModelState.AddModelError("", "Erro ao remover claim do usuário.");
                        return PartialView("_Edit", model);
                    }
                }

                foreach (var userId in userIdsToAdd)
                {
                    var result = await connection.ExecuteAsync(@"
                        INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) 
                        VALUES (@UserId, @ClaimType, @ClaimValue)", 
                        new { UserId = userId, ClaimType = model.Type, ClaimValue = model.Value });
                    if (result == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim ao usuário {(string)userId}");
                        ModelState.AddModelError("", "Erro ao adicionar claim ao usuário.");
                        return PartialView("_Edit", model);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Claims de usuários atualizadas, atualizando claims para roles");

                // Atualizar claims para roles
                var currentRolesWithClaim = await connection.QueryAsync(@"
                    SELECT r.Id, r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                    WHERE rc.ClaimType = @ClaimType AND rc.ClaimValue = @ClaimValue", 
                    new { ClaimType = model.Type, ClaimValue = model.Value });
                
                var selectedRoleIds = model.SelectedRoleIds;
                var rolesToRemove = currentRolesWithClaim.Where(r => !selectedRoleIds.Contains((string)r.Id));
                var roleIdsToAdd = selectedRoleIds.Where(id => !currentRolesWithClaim.Any(r => r.Id == id));

                foreach (var role in rolesToRemove)
                {
                    var result = await connection.ExecuteAsync(@"
                        DELETE FROM AspNetRoleClaims 
                        WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue", 
                        new { RoleId = role.Id, ClaimType = model.Type, ClaimValue = model.Value });
                    if (result == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao remover claim da role {(string)role.Id}");
                        ModelState.AddModelError("", "Erro ao remover claim da role.");
                        return PartialView("_Edit", model);
                    }
                }

                foreach (var roleId in roleIdsToAdd)
                {
                    var result = await connection.ExecuteAsync(@"
                        INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) 
                        VALUES (@RoleId, @ClaimType, @ClaimValue)", 
                        new { RoleId = roleId, ClaimType = model.Type, ClaimValue = model.Value });
                    if (result == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim à role {(string)roleId}");
                        ModelState.AddModelError("", "Erro ao adicionar claim à role.");
                        return PartialView("_Edit", model);
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

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Obter usuários com esta claim
                var usersWithClaim = await connection.QueryAsync(@"
                    SELECT u.Id, u.UserName, u.Email 
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                    WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
                
                // Obter roles com esta claim
                var rolesWithClaim = await connection.QueryAsync(@"
                    SELECT r.Id, r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                    WHERE rc.ClaimType = @ClaimType AND rc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
            
                var usersList = usersWithClaim.ToList();
                var rolesList = rolesWithClaim.ToList();
                
                var userNames = new List<string>();
                foreach (var user in usersList)
                {
                    userNames.Add((string)user.UserName);
                }
                
                var roleNames = new List<string>();
                foreach (var role in rolesList)
                {
                    roleNames.Add((string)role.Name);
                }
                
                var viewModel = new ClaimViewModel
                {
                    Type = type,
                    Value = value,
                    Users = string.Join(", ", userNames),
                    Roles = string.Join(", ", roleNames),
                    UserCount = usersList.Count,
                    RoleCount = rolesList.Count
                };

                return PartialView("_Delete", viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Delete GET: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar dados para exclusão: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string type, string value)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                System.Diagnostics.Debug.WriteLine($"DeleteConfirmed Claims chamado - Type: {type}, Value: {value}");

                // Remover claim de todos os usuários
                var usersWithClaim = await connection.QueryAsync(@"
                    SELECT u.Id, u.UserName, u.Email 
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                    WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
                
                var usersList = usersWithClaim.ToList();
                System.Diagnostics.Debug.WriteLine($"Removendo claim de {usersList.Count} usuários");
                
                foreach (var user in usersList)
                {
                    var result = await connection.ExecuteAsync(@"
                        DELETE FROM AspNetUserClaims 
                        WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue", 
                        new { UserId = user.Id, ClaimType = type, ClaimValue = value });
                    if (result == 0)
                    {
                        var errorMessage = string.Join(", ", ModelState.Values.Select(v => v.Errors.Select(e => e.ErrorMessage)));
                        System.Diagnostics.Debug.WriteLine($"Erro ao remover claim do usuário {(string)user.Id}: {errorMessage}");
                        throw new InvalidOperationException($"Erro ao remover claim do usuário: {errorMessage}");
                    }
                }

                // Remover claim de todas as roles
                var rolesWithClaim = await connection.QueryAsync(@"
                    SELECT r.Id, r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                    WHERE rc.ClaimType = @ClaimType AND rc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
                
                var rolesList = rolesWithClaim.ToList();
                System.Diagnostics.Debug.WriteLine($"Removendo claim de {rolesList.Count} roles");
                
                foreach (var role in rolesList)
                {
                    var result = await connection.ExecuteAsync(@"
                        DELETE FROM AspNetRoleClaims 
                        WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue", 
                        new { RoleId = role.Id, ClaimType = type, ClaimValue = value });
                    if (result == 0)
                    {
                        var errorMessage = string.Join(", ", ModelState.Values.Select(v => v.Errors.Select(e => e.ErrorMessage)));
                        System.Diagnostics.Debug.WriteLine($"Erro ao remover claim da role {(string)role.Id}: {errorMessage}");
                        throw new InvalidOperationException($"Erro ao remover claim da role: {errorMessage}");
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

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Obter usuários com esta claim
                var usersWithClaim = await connection.QueryAsync(@"
                    SELECT u.Id, u.UserName, u.Email 
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                    WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
                
                // Obter roles com esta claim
                var rolesWithClaim = await connection.QueryAsync(@"
                    SELECT r.Id, r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                    WHERE rc.ClaimType = @ClaimType AND rc.ClaimValue = @ClaimValue", 
                    new { ClaimType = type, ClaimValue = value });
            
                var usersList = usersWithClaim.ToList();
                var rolesList = rolesWithClaim.ToList();

                var viewModel = new ClaimDetailViewModel
                {
                    Type = type,
                    Value = value,
                    UsersWithClaim = usersList.Select(u => new UserSelectionViewModel 
                    { 
                        Id = (string)u.Id, 
                        UserName = (string)u.UserName, 
                        Email = (string)u.Email, 
                        IsSelected = true 
                    }).ToList(),
                    RolesWithClaim = rolesList.Select(r => new RoleSelectionClaimViewModel 
                    { 
                        Id = (string)r.Id, 
                        Name = (string)r.Name, 
                        IsSelected = true 
                    }).ToList(),
                    TotalUsers = usersList.Count,
                    TotalRoles = rolesList.Count
                };

                return PartialView("_Details", viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no DetailsPartial: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar detalhes da claim: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreatePartial()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var allUsers = await connection.QueryAsync(@"
                    SELECT Id, UserName, Email FROM AspNetUsers ORDER BY UserName");
                var allRoles = await connection.QueryAsync(@"
                    SELECT Id, Name FROM AspNetRoles ORDER BY Name");

                var model = new CreateClaimViewModel
                {
                    AvailableUsers = allUsers.Select(u => new UserSelectionViewModel
                    {
                        Id = (string)u.Id,
                        UserName = (string)u.UserName,
                        Email = (string)u.Email,
                        IsSelected = false
                    }).ToList(),
                    AvailableRoles = allRoles.Select(r => new RoleSelectionClaimViewModel
                    {
                        Id = (string)r.Id,
                        Name = (string)r.Name,
                        IsSelected = false
                    }).ToList()
                };

                return PartialView("_Create", model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no CreatePartial GET: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar dados para criação: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial([FromForm] CreateClaimViewModel model)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                System.Diagnostics.Debug.WriteLine($"CreatePartial Claims POST chamado - Type: {model?.Type}, Value: {model?.Value}");
                
                // Inicializar listas se forem null
                model.SelectedUsers ??= new List<string>();
                model.SelectedRoles ??= new List<string>();
                
                if (!model.SelectedUsers.Any() && !model.SelectedRoles.Any())
                {
                    ModelState.AddModelError("", "Selecione pelo menos um usuário ou uma role para atribuir a claim.");
                }

                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState inválido na criação de claims");
                    // Recarregar dados necessários em caso de erro
                    var allUsers = await connection.QueryAsync(@"
                        SELECT Id, UserName, Email FROM AspNetUsers ORDER BY UserName");
                    var allRoles = await connection.QueryAsync(@"
                        SELECT Id, Name FROM AspNetRoles ORDER BY Name");

                    model.AvailableUsers = allUsers.Select(u => new UserSelectionViewModel
                    {
                        Id = (string)u.Id,
                        UserName = (string)u.UserName,
                        Email = (string)u.Email,
                        IsSelected = model.SelectedUsers.Contains((string)u.Id)
                    }).ToList();

                    model.AvailableRoles = allRoles.Select(r => new RoleSelectionClaimViewModel
                    {
                        Id = (string)r.Id,
                        Name = (string)r.Name,
                        IsSelected = model.SelectedRoles.Contains((string)r.Id)
                    }).ToList();

                    return PartialView("_Create", model);
                }

                System.Diagnostics.Debug.WriteLine("ModelState válido, criando claim");

                var claim = new Claim(model.Type, model.Value);

                // Adicionar claim aos usuários selecionados
                foreach (var userId in model.SelectedUsers)
                {
                    // Verificar se a claim já existe para evitar duplicatas
                    var existingUserClaim = await connection.QuerySingleOrDefaultAsync(@"
                        SELECT Id FROM AspNetUserClaims 
                        WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                        new { UserId = userId, ClaimType = model.Type, ClaimValue = model.Value });

                    if (existingUserClaim == null)
                    {
                        var result = await connection.ExecuteAsync(@"
                            INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) 
                            VALUES (@UserId, @ClaimType, @ClaimValue)", 
                            new { UserId = userId, ClaimType = model.Type, ClaimValue = model.Value });
                        if (result == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim ao usuário {(string)userId}");
                            ModelState.AddModelError("", $"Erro ao adicionar claim ao usuário {(string)userId}.");
                            return PartialView("_Create", model);
                        }
                        System.Diagnostics.Debug.WriteLine($"Claim adicionada ao usuário: {(string)userId}");
                    }
                }

                // Adicionar claim às roles selecionadas
                foreach (var roleId in model.SelectedRoles)
                {
                    // Verificar se a claim já existe para evitar duplicatas
                    var existingRoleClaim = await connection.QuerySingleOrDefaultAsync(@"
                        SELECT Id FROM AspNetRoleClaims 
                        WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                        new { RoleId = roleId, ClaimType = model.Type, ClaimValue = model.Value });

                    if (existingRoleClaim == null)
                    {
                        var result = await connection.ExecuteAsync(@"
                            INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) 
                            VALUES (@RoleId, @ClaimType, @ClaimValue)", 
                            new { RoleId = roleId, ClaimType = model.Type, ClaimValue = model.Value });
                        if (result == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao adicionar claim à role {(string)roleId}");
                            ModelState.AddModelError("", $"Erro ao adicionar claim à role {(string)roleId}.");
                            return PartialView("_Create", model);
                        }
                        System.Diagnostics.Debug.WriteLine($"Claim adicionada à role: {(string)roleId}");
                    }
                }

                System.Diagnostics.Debug.WriteLine("Criação de claim completa com sucesso");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exceção no CreatePartial Claims: {ex.Message}");
                ModelState.AddModelError("", "Erro ao criar claim: " + ex.Message);
                return PartialView("_Create", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GridPartial()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Obter todas as claims únicas (tanto de usuários quanto de roles)
                var allClaims = await connection.QueryAsync(@"
                    SELECT DISTINCT ClaimType, ClaimValue 
                    FROM (
                        SELECT ClaimType, ClaimValue FROM AspNetUserClaims
                        UNION
                        SELECT ClaimType, ClaimValue FROM AspNetRoleClaims
                    ) AS AllClaims
                    ORDER BY ClaimType, ClaimValue");

                var claimViewModels = new List<ClaimViewModel>();

                foreach (var claim in allClaims)
                {
                    try
                    {
                        var claimType = (string)claim.ClaimType;
                        var claimValue = (string)claim.ClaimValue;
                        
                        // Obter usuários com esta claim
                        var usersWithClaim = await connection.QueryAsync(@"
                            SELECT u.Id, u.UserName, u.Email 
                            FROM AspNetUsers u
                            INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                            WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue", 
                            new { ClaimType = claimType, ClaimValue = claimValue });
                        
                        // Obter roles com esta claim
                        var rolesWithClaim = await connection.QueryAsync(@"
                            SELECT r.Id, r.Name 
                            FROM AspNetRoles r
                            INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                            WHERE rc.ClaimType = @ClaimType AND rc.ClaimValue = @ClaimValue", 
                            new { ClaimType = claimType, ClaimValue = claimValue });
                        
                        var usersList = usersWithClaim.ToList();
                        var rolesList = rolesWithClaim.ToList();
                        
                        var userNames = new List<string>();
                        foreach (var user in usersList)
                        {
                            userNames.Add((string)user.UserName);
                        }
                        
                        var roleNames = new List<string>();
                        foreach (var role in rolesList)
                        {
                            roleNames.Add((string)role.Name);
                        }
                        
                        claimViewModels.Add(new ClaimViewModel
                        {
                            Type = claimType ?? "",
                            Value = claimValue ?? "",
                            Users = string.Join(", ", userNames),
                            Roles = string.Join(", ", roleNames),
                            UserCount = usersList.Count,
                            RoleCount = rolesList.Count
                        });
                    }
                    catch (Exception claimEx)
                    {
                        // Se houver erro com uma claim específica, pular para a próxima
                        System.Diagnostics.Debug.WriteLine($"Erro ao processar claim {(string)claim.ClaimType}:{(string)claim.ClaimValue}: {claimEx.Message}");
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