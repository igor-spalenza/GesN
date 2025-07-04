using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GesN.Web.Areas.Admin.Models;
using GesN.Web.Infrastructure.Data;
using System.Data;
using GesN.Web.Data;
using System.IO;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RolesController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IActionResult> Index()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT 
                    r.Id, 
                    r.Name,
                    r.NormalizedName,
                    COUNT(rc.Id) as ClaimCount
                FROM AspNetRoles r
                LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                GROUP BY r.Id, r.Name, r.NormalizedName
                ORDER BY r.Name";

            var roles = await connection.QueryAsync(sql);
            var roleViewModels = new List<RoleViewModel>();

            foreach (var role in roles)
            {
                var roleId = (string)role.Id;
                var roleName = (string)role.Name;
                
                // Buscar claims da role
                var claims = await connection.QueryAsync(@"
                    SELECT ClaimType, ClaimValue 
                    FROM AspNetRoleClaims 
                    WHERE RoleId = @RoleId
                    ORDER BY ClaimType, ClaimValue", 
                    new { RoleId = roleId });
                
                var claimsViewModels = new List<ClaimViewModel>();
                foreach (var claim in claims)
                {
                    claimsViewModels.Add(new ClaimViewModel 
                    { 
                        Type = (string)claim.ClaimType, 
                        Value = (string)claim.ClaimValue 
                    });
                }
                
                roleViewModels.Add(new RoleViewModel
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleName?.ToUpper(),
                    Users = "", // Será carregado conforme necessário
                    UserCount = 0, // Será carregado conforme necessário
                    Claims = claimsViewModels
                });
            }

            return View(roleViewModels);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var role = await connection.QuerySingleOrDefaultAsync(
                "SELECT Id, Name, NormalizedName FROM AspNetRoles WHERE Id = @Id",
                new { Id = id });

            if (role == null)
            {
                return NotFound();
            }

            var roleId = (string)role.Id;
            var roleName = (string)role.Name;
            var roleNormalizedName = (string)role.NormalizedName;

            // Buscar usuários na role
            var users = await connection.QueryAsync(@"
                SELECT u.Id, u.UserName, u.Email, u.FirstName, u.LastName
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                WHERE ur.RoleId = @RoleId
                ORDER BY u.UserName",
                new { RoleId = roleId });

            // Buscar claims da role
            var roleClaims = await connection.QueryAsync(@"
                SELECT ClaimType, ClaimValue 
                FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId
                ORDER BY ClaimType, ClaimValue",
                new { RoleId = roleId });
            
            var usersList = users.ToList();
            var claimsList = roleClaims.ToList();
            
            var userNames = new List<string>();
            foreach (var user in usersList)
            {
                userNames.Add((string)user.UserName);
            }
            
            var claimsViewModels = new List<ClaimViewModel>();
            foreach (var claim in claimsList)
            {
                claimsViewModels.Add(new ClaimViewModel 
                { 
                    Type = (string)claim.ClaimType, 
                    Value = (string)claim.ClaimValue 
                });
            }
            
            var viewModel = new RoleViewModel
            {
                Id = roleId,
                Name = roleName,
                NormalizedName = roleNormalizedName,
                Users = string.Join(", ", userNames),
                UserCount = usersList.Count,
                Claims = claimsViewModels
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            using var connection = await _connectionFactory.CreateConnectionAsync();

            var role = await connection.QuerySingleOrDefaultAsync(
                "SELECT Id, Name, NormalizedName FROM AspNetRoles WHERE Id = @Id",
                new { Id = id });

            if (role == null)
                return NotFound();

            var roleId = (string)role.Id;
            var roleName = (string)role.Name;
            var roleNormalizedName = (string)role.NormalizedName;

            // Buscar claims da role
            var roleClaims = await connection.QueryAsync(@"
                SELECT ClaimType, ClaimValue 
                FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId
                ORDER BY ClaimType, ClaimValue",
                new { RoleId = roleId });

            // Buscar usuários associados à role
            var associatedUsers = await connection.QueryAsync(@"
                SELECT u.Id, u.UserName, u.Email, u.FirstName, u.LastName
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                WHERE ur.RoleId = @RoleId
                ORDER BY u.UserName",
                new { RoleId = roleId });

            var claimsList = roleClaims.ToList();
            var usersList = associatedUsers.ToList();

            var claimsViewModels = new List<ClaimViewModel>();
            foreach (var claim in claimsList)
            {
                claimsViewModels.Add(new ClaimViewModel 
                { 
                    Type = (string)claim.ClaimType, 
                    Value = (string)claim.ClaimValue 
                });
            }
            
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

            var model = new EditRoleViewModel
            {
                Id = roleId,
                Name = roleName,
                NormalizedName = roleNormalizedName,
                Claims = claimsViewModels,
                AssociatedUsers = usersViewModels
            };

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            if (model.Claims == null) model.Claims = new List<ClaimViewModel>();
            model.Claims.RemoveAll(c => string.IsNullOrWhiteSpace(c.Type) && string.IsNullOrWhiteSpace(c.Value));

            if (!ModelState.IsValid)
            {
                if (model.AssociatedUsers == null) model.AssociatedUsers = new List<UserSelectionViewModel>();
                return PartialView("_Edit", model);
            }

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var role = await connection.QuerySingleOrDefaultAsync(
                    "SELECT Id, Name, NormalizedName FROM AspNetRoles WHERE Id = @Id",
                    new { Id = model.Id });

                if (role == null)
                {
                    ModelState.AddModelError("", "Role não encontrada.");
                    if (model.AssociatedUsers == null) model.AssociatedUsers = new List<UserSelectionViewModel>();
                    return PartialView("_Edit", model);
                }

                // Verificar se o novo nome já existe (se foi alterado)
                if (model.Name != (string)role.Name)
                {
                    var existingRole = await connection.QuerySingleOrDefaultAsync(
                        "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName AND Id != @Id",
                        new { NormalizedName = model.Name.ToUpper(), Id = model.Id });

                    if (existingRole != null)
                    {
                        ModelState.AddModelError("Name", "Este nome de Role já está em uso.");
                        if (model.AssociatedUsers == null) model.AssociatedUsers = new List<UserSelectionViewModel>();
                        return PartialView("_Edit", model);
                    }
                }

                // Atualizar o nome da role
                var updateResult = await connection.ExecuteAsync(@"
                    UPDATE AspNetRoles 
                    SET Name = @Name, 
                        NormalizedName = @NormalizedName,
                        ConcurrencyStamp = @ConcurrencyStamp
                    WHERE Id = @Id",
                    new
                    {
                        Id = model.Id,
                        Name = model.Name,
                        NormalizedName = model.Name.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });

                if (updateResult == 0)
                {
                    ModelState.AddModelError("", "Erro ao atualizar role.");
                    if (model.AssociatedUsers == null) model.AssociatedUsers = new List<UserSelectionViewModel>();
                    return PartialView("_Edit", model);
                }

                // Gerenciar claims
                var currentClaims = await connection.QueryAsync(@"
                    SELECT ClaimType, ClaimValue 
                    FROM AspNetRoleClaims 
                    WHERE RoleId = @RoleId",
                    new { RoleId = model.Id });

                var currentClaimsList = currentClaims.ToList();

                // Claims para remover: existem em currentClaims mas não em model.Claims
                foreach (var currentClaim in currentClaimsList)
                {
                    bool found = model.Claims.Any(mc => mc.Type == (string)currentClaim.ClaimType && mc.Value == (string)currentClaim.ClaimValue);
                    
                    if (!found)
                    {
                        var removeResult = await connection.ExecuteAsync(@"
                            DELETE FROM AspNetRoleClaims 
                            WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                            new 
                            { 
                                RoleId = model.Id, 
                                ClaimType = (string)currentClaim.ClaimType, 
                                ClaimValue = (string)currentClaim.ClaimValue 
                            });

                        if (removeResult == 0)
                        {
                            ModelState.AddModelError("", $"Erro ao remover claim '{(string)currentClaim.ClaimType} - {(string)currentClaim.ClaimValue}'.");
                        }
                    }
                }

                // Claims para adicionar: existem em model.Claims mas não em currentClaims
                foreach (var modelClaim in model.Claims)
                {
                    bool found = currentClaimsList.Any(cc => (string)cc.ClaimType == modelClaim.Type && (string)cc.ClaimValue == modelClaim.Value);
                    
                    if (!found)
                    {
                        var addResult = await connection.ExecuteAsync(@"
                            INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                            VALUES (@RoleId, @ClaimType, @ClaimValue)",
                            new
                            {
                                RoleId = model.Id,
                                ClaimType = modelClaim.Type,
                                ClaimValue = modelClaim.Value
                            });

                        if (addResult == 0)
                        {
                            ModelState.AddModelError("", $"Erro ao adicionar claim '{modelClaim.Type} - {modelClaim.Value}'.");
                        }
                    }
                }

                if (!ModelState.IsValid) // Se houver erros de claim
                {
                    if (model.AssociatedUsers == null) model.AssociatedUsers = new List<UserSelectionViewModel>();
                    return PartialView("_Edit", model);
                }

                return Json(new { success = true, message = "Role atualizada com sucesso!" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao atualizar role: {ex.Message}");
                if (model.AssociatedUsers == null) model.AssociatedUsers = new List<UserSelectionViewModel>();
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
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var role = await connection.QuerySingleOrDefaultAsync(
                "SELECT Id, Name, NormalizedName FROM AspNetRoles WHERE Id = @Id",
                new { Id = id });

            if (role == null)
            {
                return NotFound();
            }

            var roleId = (string)role.Id;
            var roleName = (string)role.Name;
            var roleNormalizedName = (string)role.NormalizedName;

            // Buscar usuários na role
            var users = await connection.QueryAsync(@"
                SELECT u.Id, u.UserName, u.Email, u.FirstName, u.LastName
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                WHERE ur.RoleId = @RoleId
                ORDER BY u.UserName",
                new { RoleId = roleId });

            var usersList = users.ToList();
            
            var userNames = new List<string>();
            foreach (var user in usersList)
            {
                userNames.Add((string)user.UserName);
            }
            
            var viewModel = new RoleViewModel
            {
                Id = roleId,
                Name = roleName,
                NormalizedName = roleNormalizedName,
                Users = string.Join(", ", userNames),
                UserCount = usersList.Count
            };

            return PartialView("_Delete", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var role = await connection.QuerySingleOrDefaultAsync(
                    "SELECT Id, Name FROM AspNetRoles WHERE Id = @Id",
                    new { Id = id });

                if (role == null)
                {
                    return Json(new { success = false, message = "Role não encontrada." });
                }

                // Remover claims da role primeiro
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetRoleClaims WHERE RoleId = @RoleId",
                    new { RoleId = id });

                // Remover associações usuário-role
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUserRoles WHERE RoleId = @RoleId",
                    new { RoleId = id });

                // Remover a role
                var deleteResult = await connection.ExecuteAsync(
                    "DELETE FROM AspNetRoles WHERE Id = @Id",
                    new { Id = id });

                if (deleteResult > 0)
                {
                    return Json(new { success = true, message = "Role excluída com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao excluir role." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao excluir role: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var role = await connection.QuerySingleOrDefaultAsync(
                "SELECT Id, Name, NormalizedName FROM AspNetRoles WHERE Id = @Id",
                new { Id = id });

            if (role == null)
            {
                return NotFound();
            }

            var roleId = (string)role.Id;
            var roleName = (string)role.Name;
            var roleNormalizedName = (string)role.NormalizedName;

            // Buscar usuários na role
            var users = await connection.QueryAsync(@"
                SELECT u.Id, u.UserName, u.Email, u.FirstName, u.LastName
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                WHERE ur.RoleId = @RoleId
                ORDER BY u.UserName",
                new { RoleId = roleId });

            // Buscar claims da role
            var roleClaims = await connection.QueryAsync(@"
                SELECT ClaimType, ClaimValue 
                FROM AspNetRoleClaims 
                WHERE RoleId = @RoleId
                ORDER BY ClaimType, ClaimValue",
                new { RoleId = roleId });

            var usersList = users.ToList();
            var claimsList = roleClaims.ToList();
            
            var userNames = new List<string>();
            foreach (var user in usersList)
            {
                userNames.Add((string)user.UserName);
            }
            
            var claimsViewModels = new List<ClaimViewModel>();
            foreach (var claim in claimsList)
            {
                claimsViewModels.Add(new ClaimViewModel 
                { 
                    Type = (string)claim.ClaimType, 
                    Value = (string)claim.ClaimValue 
                });
            }
            
            var viewModel = new RoleViewModel
            {
                Id = roleId,
                Name = roleName,
                NormalizedName = roleNormalizedName,
                Users = string.Join(", ", userNames),
                UserCount = usersList.Count,
                Claims = claimsViewModels
            };

            return PartialView("_Details", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePartial()
        {
            try
            {
                // Carregar tipos de claims disponíveis
                var availableClaimTypes = await GetAvailableClaimTypesAsync();

                var model = new CreateRoleViewModel
                {
                    Claims = new List<ClaimViewModel> { new ClaimViewModel() }, // Começa com uma claim vazia
                    AvailableClaimTypes = availableClaimTypes
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
        public async Task<IActionResult> CreatePartial(CreateRoleViewModel model)
        {
            if (model.Claims == null)
            {
                model.Claims = new List<ClaimViewModel>();
            }

            // Remove claims vazias antes de validar o ModelState
            model.Claims.RemoveAll(c => string.IsNullOrWhiteSpace(c.Type) && string.IsNullOrWhiteSpace(c.Value));

            if (ModelState.IsValid)
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                try
                {
                    // Verificar se o nome da role já existe
                    var existingRole = await connection.QuerySingleOrDefaultAsync(
                        "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                        new { NormalizedName = model.Name.ToUpper() });

                    if (existingRole != null)
                    {
                        ModelState.AddModelError("Name", "Este nome de Role já está em uso.");
                        // ✅ CORREÇÃO: Usar mesma conexão para carregar claim types
                        model.AvailableClaimTypes = await GetAvailableClaimTypesWithConnectionAsync(connection);
                        return PartialView("_Create", model);
                    }

                    // Criar a role
                    var roleId = Guid.NewGuid().ToString();
                    var createRoleResult = await connection.ExecuteAsync(@"
                        INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                        VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)",
                        new
                        {
                            Id = roleId,
                            Name = model.Name,
                            NormalizedName = model.Name.ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        });

                    if (createRoleResult > 0)
                    {
                        // Adicionar claims se houver
                        if (model.Claims.Any())
                        {
                            foreach (var claimViewModel in model.Claims)
                            {
                                if (string.IsNullOrWhiteSpace(claimViewModel.Type) || string.IsNullOrWhiteSpace(claimViewModel.Value))
                                    continue;

                                var claimResult = await connection.ExecuteAsync(@"
                                    INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                                    VALUES (@RoleId, @ClaimType, @ClaimValue)",
                                    new
                                    {
                                        RoleId = roleId,
                                        ClaimType = claimViewModel.Type,
                                        ClaimValue = claimViewModel.Value
                                    });

                                if (claimResult == 0)
                                {
                                    ModelState.AddModelError("", $"Erro ao adicionar claim '{claimViewModel.Type} - {claimViewModel.Value}'.");
                                }
                            }
                        }

                        // Verifica se algum erro foi adicionado ao ModelState durante a adição de claims
                        if (!ModelState.IsValid)
                        {
                            // ✅ CORREÇÃO: Usar mesma conexão para carregar claim types
                            model.AvailableClaimTypes = await GetAvailableClaimTypesWithConnectionAsync(connection);
                            return PartialView("_Create", model);
                        }

                        return Json(new { success = true, message = "Role criada com sucesso!" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Erro ao criar role.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao criar role: {ex.Message}");
                }
            }

            // Se ModelState não é válido, retorna a partial view com o modelo e os erros
            if (model.Claims == null) model.Claims = new List<ClaimViewModel>();
            
            // ✅ CORREÇÃO: Evitar criar nova conexão em caso de erro
            model.AvailableClaimTypes = new List<string> { "Permission", "Department", "AccessLevel", "Module", "Feature" };
            
            return PartialView("_Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> DeletePartial(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var role = await connection.QuerySingleOrDefaultAsync(
                    "SELECT Id, Name, NormalizedName FROM AspNetRoles WHERE Id = @Id",
                    new { Id = id });

                if (role == null)
                {
                    return NotFound();
                }

                var roleId = (string)role.Id;
                var roleName = (string)role.Name;
                var roleNormalizedName = (string)role.NormalizedName;

                // Buscar usuários na role
                var users = await connection.QueryAsync(@"
                    SELECT u.Id, u.UserName, u.Email, u.FirstName, u.LastName
                    FROM AspNetUsers u
                    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                    WHERE ur.RoleId = @RoleId
                    ORDER BY u.UserName",
                    new { RoleId = roleId });

                // Buscar claims da role
                var roleClaims = await connection.QueryAsync(@"
                    SELECT ClaimType, ClaimValue 
                    FROM AspNetRoleClaims 
                    WHERE RoleId = @RoleId
                    ORDER BY ClaimType, ClaimValue",
                    new { RoleId = roleId });

                var usersList = users.ToList();
                var claimsList = roleClaims.ToList();

                var userNames = new List<string>();
                foreach (var user in usersList)
                {
                    userNames.Add((string)user.UserName);
                }

                var claimsViewModels = new List<ClaimViewModel>();
                foreach (var claim in claimsList)
                {
                    claimsViewModels.Add(new ClaimViewModel 
                    { 
                        Type = (string)claim.ClaimType, 
                        Value = (string)claim.ClaimValue 
                    });
                }

                var viewModel = new RoleViewModel
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleNormalizedName,
                    Users = string.Join(", ", userNames),
                    UserCount = usersList.Count,
                    Claims = claimsViewModels
                };

                return PartialView("_Delete", viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no DeletePartial: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar dados para exclusão: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePartialConfirmed(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                // Primeiro, remover todas as claims da role
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetRoleClaims WHERE RoleId = @Id",
                    new { Id = id });

                // Remover todas as associações usuário-role
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUserRoles WHERE RoleId = @Id",
                    new { Id = id });

                // Finalmente, remover a role
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetRoles WHERE Id = @Id",
                    new { Id = id });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GridPartial()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var sql = @"
                    SELECT 
                        r.Id, 
                        r.Name,
                        r.NormalizedName,
                        COUNT(rc.Id) as ClaimCount
                    FROM AspNetRoles r
                    LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                    GROUP BY r.Id, r.Name, r.NormalizedName
                    ORDER BY r.Name";

                var roles = await connection.QueryAsync(sql);
                var roleViewModels = new List<RoleViewModel>();

                foreach (var role in roles)
                {
                    try
                    {
                        var roleId = (string)role.Id;
                        var roleName = (string)role.Name;
                        var roleNormalizedName = (string)role.NormalizedName;
                        
                        // Buscar usuários na role
                        var users = await connection.QueryAsync(@"
                            SELECT u.Id, u.UserName, u.Email, u.FirstName, u.LastName
                            FROM AspNetUsers u
                            INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                            WHERE ur.RoleId = @RoleId
                            ORDER BY u.UserName",
                            new { RoleId = roleId });

                        // Buscar claims da role
                        var roleClaims = await connection.QueryAsync(@"
                            SELECT ClaimType, ClaimValue 
                            FROM AspNetRoleClaims 
                            WHERE RoleId = @RoleId
                            ORDER BY ClaimType, ClaimValue",
                            new { RoleId = roleId });
                        
                        var usersList = users.ToList();
                        var claimsList = roleClaims.ToList();
                        
                        var userNames = new List<string>();
                        foreach (var user in usersList)
                        {
                            userNames.Add((string)user.UserName);
                        }
                        
                        var claimsViewModels = new List<ClaimViewModel>();
                        foreach (var claim in claimsList)
                        {
                            claimsViewModels.Add(new ClaimViewModel 
                            { 
                                Type = (string)claim.ClaimType, 
                                Value = (string)claim.ClaimValue 
                            });
                        }
                        
                        roleViewModels.Add(new RoleViewModel
                        {
                            Id = roleId,
                            Name = roleName ?? "",
                            NormalizedName = roleNormalizedName ?? "",
                            Users = string.Join(", ", userNames),
                            UserCount = usersList.Count,
                            Claims = claimsViewModels
                        });
                    }
                    catch (Exception roleEx)
                    {
                        // Se houver erro com uma role específica, pular para a próxima
                        System.Diagnostics.Debug.WriteLine($"Erro ao processar role {(string)role.Id}: {roleEx.Message}");
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

        private async Task<List<string>> GetAvailableClaimTypesAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                return await GetAvailableClaimTypesWithConnectionAsync(connection);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar tipos de claims: {ex.Message}");
                // Retornar tipos básicos em caso de erro
                return new List<string> { "Permission", "Department", "AccessLevel", "Module", "Feature" };
            }
        }

        private async Task<List<string>> GetAvailableClaimTypesWithConnectionAsync(IDbConnection connection)
        {
            try
            {
                var claimTypes = await connection.QueryAsync<string>(@"
                    SELECT DISTINCT ClaimType 
                    FROM (
                        SELECT ClaimType FROM AspNetUserClaims
                        UNION
                        SELECT ClaimType FROM AspNetRoleClaims
                    ) AS AllClaimTypes
                    WHERE ClaimType IS NOT NULL AND ClaimType != ''
                    ORDER BY ClaimType");

                var result = claimTypes.ToList();

                // Adicionar tipos comuns se não existirem
                var commonClaimTypes = new List<string>
                {
                    "Permission",
                    "Department", 
                    "AccessLevel",
                    "Module",
                    "Feature",
                    "Action",
                    "Resource",
                    "Scope"
                };

                foreach (var commonType in commonClaimTypes)
                {
                    if (!result.Contains(commonType))
                    {
                        result.Add(commonType);
                    }
                }

                return result.OrderBy(x => x).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar tipos de claims: {ex.Message}");
                // Retornar tipos básicos em caso de erro
                return new List<string> { "Permission", "Department", "AccessLevel", "Module", "Feature" };
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            var diagnostics = new List<string>();
            
            try
            {
                // Teste 1: Verificar se consegue criar conexão
                diagnostics.Add("1. Tentando criar conexão...");
                using var connection = await _connectionFactory.CreateConnectionAsync();
                diagnostics.Add("✅ Conexão criada com sucesso");
                
                // Teste 2: Verificar estado da conexão
                diagnostics.Add($"2. Estado da conexão: {connection.State}");
                
                // Teste 3: Query simples de leitura
                diagnostics.Add("3. Tentando SELECT simples...");
                var count = await connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM AspNetRoles");
                diagnostics.Add($"✅ SELECT funcionou. Count: {count}");
                
                // Teste 4: Verificar locks ativos
                diagnostics.Add("4. Verificando locks ativos...");
                try
                {
                    var pragmaResult = await connection.QuerySingleAsync<string>("PRAGMA locking_mode");
                    diagnostics.Add($"   Locking mode: {pragmaResult}");
                }
                catch (Exception ex)
                {
                    diagnostics.Add($"   Erro ao verificar locking mode: {ex.Message}");
                }
                
                // Teste 5: Verificar journal mode
                try
                {
                    var journalMode = await connection.QuerySingleAsync<string>("PRAGMA journal_mode");
                    diagnostics.Add($"   Journal mode: {journalMode}");
                }
                catch (Exception ex)
                {
                    diagnostics.Add($"   Erro ao verificar journal mode: {ex.Message}");
                }
                
                // Teste 6: Verificar busy timeout
                try
                {
                    var busyTimeout = await connection.QuerySingleAsync<int>("PRAGMA busy_timeout");
                    diagnostics.Add($"   Busy timeout: {busyTimeout}");
                }
                catch (Exception ex)
                {
                    diagnostics.Add($"   Erro ao verificar busy timeout: {ex.Message}");
                }
                
                // Teste 7: Tentar INSERT simples
                diagnostics.Add("5. Tentando INSERT simples...");
                var testId = Guid.NewGuid().ToString();
                var testName = "TEST_ROLE_" + DateTime.Now.Ticks;
                
                await connection.ExecuteAsync(@"
                    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                    VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)",
                    new
                    {
                        Id = testId,
                        Name = testName,
                        NormalizedName = testName,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                diagnostics.Add("✅ INSERT funcionou");
                
                // Teste 8: Tentar DELETE
                diagnostics.Add("6. Tentando DELETE...");
                await connection.ExecuteAsync("DELETE FROM AspNetRoles WHERE Id = @Id", new { Id = testId });
                diagnostics.Add("✅ DELETE funcionou");
                
                return Json(new { 
                    success = true, 
                    message = "Todos os testes passaram!", 
                    diagnostics = diagnostics 
                });
            }
            catch (Exception ex)
            {
                diagnostics.Add($"❌ ERRO: {ex.Message}");
                diagnostics.Add($"❌ StackTrace: {ex.StackTrace}");
                
                return Json(new { 
                    success = false, 
                    message = $"ERRO: {ex.Message}", 
                    diagnostics = diagnostics,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        [HttpGet]
        public IActionResult DiagnosticInfo()
        {
            var diagnostics = new List<string>();
            
            try
            {
                // Informações sobre o arquivo de banco
                var connectionString = "Data Source=./Data/Database/gesn.db;Cache=Shared;Pooling=true;";
                var dbPath = connectionString.Replace("Data Source=", "").Split(';')[0];
                var fullDbPath = Path.GetFullPath(dbPath);
                
                diagnostics.Add($"Connection String: {connectionString}");
                diagnostics.Add($"DB Path: {dbPath}");
                diagnostics.Add($"Full DB Path: {fullDbPath}");
                diagnostics.Add($"DB File Exists: {System.IO.File.Exists(fullDbPath)}");
                
                if (System.IO.File.Exists(fullDbPath))
                {
                    var fileInfo = new FileInfo(fullDbPath);
                    diagnostics.Add($"DB File Size: {fileInfo.Length} bytes");
                    diagnostics.Add($"DB Last Modified: {fileInfo.LastWriteTime}");
                    
                    // Verificar arquivos relacionados
                    var walFile = fullDbPath + "-wal";
                    var shmFile = fullDbPath + "-shm";
                    var journalFile = fullDbPath + "-journal";
                    
                    diagnostics.Add($"WAL File Exists: {System.IO.File.Exists(walFile)}");
                    diagnostics.Add($"SHM File Exists: {System.IO.File.Exists(shmFile)}");
                    diagnostics.Add($"Journal File Exists: {System.IO.File.Exists(journalFile)}");
                    
                    if (System.IO.File.Exists(walFile))
                    {
                        var walInfo = new FileInfo(walFile);
                        diagnostics.Add($"WAL File Size: {walInfo.Length} bytes");
                    }
                }
                
                // Informações sobre o processo atual
                var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                diagnostics.Add($"Current Process ID: {currentProcess.Id}");
                diagnostics.Add($"Current Process Name: {currentProcess.ProcessName}");
                
                return Json(new { 
                    success = true, 
                    diagnostics = diagnostics 
                });
            }
            catch (Exception ex)
            {
                diagnostics.Add($"❌ ERRO: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = ex.Message,
                    diagnostics = diagnostics 
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestDirectDapper()
        {
            var diagnostics = new List<string>();
            
            try
            {
                diagnostics.Add("1. Teste DIRETO com Dapper (sem Identity)");
                
                using var connection = await _connectionFactory.CreateConnectionAsync();
                diagnostics.Add("✅ Conexão criada");
                
                // Teste INSERT direto
                var testId = Guid.NewGuid().ToString();
                var testName = "DIRECT_TEST_" + DateTime.Now.Ticks;
                
                var insertResult = await connection.ExecuteAsync(@"
                    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                    VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)",
                    new
                    {
                        Id = testId,
                        Name = testName,
                        NormalizedName = testName,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                diagnostics.Add($"✅ INSERT direto funcionou: {insertResult} linha(s) afetada(s)");
                
                // Teste UPDATE direto
                var updateResult = await connection.ExecuteAsync(@"
                    UPDATE AspNetRoles SET Name = @NewName WHERE Id = @Id",
                    new { NewName = testName + "_UPDATED", Id = testId });
                diagnostics.Add($"✅ UPDATE direto funcionou: {updateResult} linha(s) afetada(s)");
                
                // Teste DELETE direto
                var deleteResult = await connection.ExecuteAsync(@"
                    DELETE FROM AspNetRoles WHERE Id = @Id", new { Id = testId });
                diagnostics.Add($"✅ DELETE direto funcionou: {deleteResult} linha(s) afetada(s)");
                
                return Json(new { 
                    success = true, 
                    message = "Dapper direto funciona perfeitamente!", 
                    diagnostics = diagnostics 
                });
            }
            catch (Exception ex)
            {
                diagnostics.Add($"❌ ERRO no Dapper direto: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = ex.Message, 
                    diagnostics = diagnostics 
                });
            }
        }

    }
} 
