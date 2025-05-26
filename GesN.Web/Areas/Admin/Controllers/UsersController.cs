using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GesN.Web.Data;
using System.Data;
using BCrypt.Net;
using GesN.Web.Areas.Admin.Models;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Infrastructure.Data;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsersController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Buscar usuários com suas roles em uma única query
                var usersWithRoles = await connection.QueryAsync(@"
                    SELECT 
                        u.Id,
                        u.UserName,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.PhoneNumber,
                        r.Name as RoleName
                    FROM AspNetUsers u
                    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
                    ORDER BY u.UserName");

                // Agrupar por usuário
                var userGroups = usersWithRoles.GroupBy(u => new
                {
                    Id = (string)u.Id,
                    UserName = (string)u.UserName,
                    Email = (string)u.Email,
                    FirstName = (string)u.FirstName,
                    LastName = (string)u.LastName,
                    PhoneNumber = (string)u.PhoneNumber
                });

                var userViewModels = new List<UserViewModel>();

                foreach (var group in userGroups)
                {
                    var roles = group
                        .Where(x => !string.IsNullOrEmpty((string)x.RoleName))
                        .Select(x => (string)x.RoleName)
                        .ToList();

                    userViewModels.Add(new UserViewModel
                    {
                        Id = group.Key.Id,
                        UserName = group.Key.UserName,
                        Email = group.Key.Email,
                        FirstName = group.Key.FirstName,
                        LastName = group.Key.LastName,
                        PhoneNumber = group.Key.PhoneNumber,
                        Roles = string.Join(", ", roles),
                        Claims = new List<ClaimViewModel>() // Claims não são necessárias na view principal
                    });
                }

                return View(userViewModels);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Index: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar usuários: {ex.Message}");
            }
        }

        private async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var query = "SELECT * FROM AspNetUsers";
            var users = await connection.QueryAsync<ApplicationUser>(query);
            return users.ToList();
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var user = await GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Carregar roles do usuário
                var userRoles = await connection.QueryAsync(@"
                    SELECT r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                    WHERE ur.UserId = @UserId", 
                    new { UserId = id });

                // Carregar claims do usuário
                var userClaims = await connection.QueryAsync(@"
                    SELECT ClaimType as Type, ClaimValue as Value 
                    FROM AspNetUserClaims 
                    WHERE UserId = @UserId", 
                    new { UserId = id });

                var model = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = string.Join(", ", userRoles.Select(r => (string)r.Name)),
                    Claims = userClaims.Select(c => new ClaimViewModel
                    {
                        Type = (string)c.Type,
                        Value = (string)c.Value
                    }).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no Details: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar detalhes do usuário: {ex.Message}");
            }
        }

        private async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var query = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { Id = id });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using var connection = await _connectionFactory.CreateConnectionAsync();
                    
                    var existingUser = await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                        "SELECT * FROM AspNetUsers WHERE Email = @Email OR UserName = @UserName",
                        new { Email = model.Email, UserName = model.Email });

                    if (existingUser != null)
                    {
                        ModelState.AddModelError(string.Empty, "Usuário com este email ou nome de usuário já existe.");
                        return View(model);
                    }

                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    var userId = Guid.NewGuid().ToString();

                    var query = @"
                        INSERT INTO AspNetUsers (
                            Id, UserName, NormalizedUserName, Email, NormalizedEmail,
                            EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
                            PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                            LockoutEnd, LockoutEnabled, AccessFailedCount,
                            FirstName, LastName
                        ) VALUES (
                            @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                            @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp,
                            @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled,
                            @LockoutEnd, @LockoutEnabled, @AccessFailedCount,
                            @FirstName, @LastName
                        )";

                    await connection.ExecuteAsync(query, new
                    {
                        Id = userId,
                        UserName = model.Email,
                        NormalizedUserName = model.Email.ToUpper(),
                        Email = model.Email,
                        NormalizedEmail = model.Email.ToUpper(),
                        EmailConfirmed = false,
                        PasswordHash = hashedPassword,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        PhoneNumber = model.PhoneNumber ?? "",
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnd = (DateTimeOffset?)null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    });

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erro ao criar usuário: {ex.Message}");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
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

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using var connection = await _connectionFactory.CreateConnectionAsync();
                    
                    var query = @"
                        UPDATE AspNetUsers 
                        SET UserName = @UserName,
                            NormalizedUserName = @NormalizedUserName,
                            Email = @Email,
                            NormalizedEmail = @NormalizedEmail,
                            FirstName = @FirstName,
                            LastName = @LastName,
                            PhoneNumber = @PhoneNumber,
                            ConcurrencyStamp = @ConcurrencyStamp
                        WHERE Id = @Id";

                    await connection.ExecuteAsync(query, new
                    {
                        Id = id,
                        UserName = model.UserName,
                        NormalizedUserName = model.UserName.ToUpper(),
                        Email = model.Email,
                        NormalizedEmail = model.Email.ToUpper(),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erro ao atualizar usuário: {ex.Message}");
                }
            }

            return View(model);
        }

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

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                await connection.ExecuteAsync("DELETE FROM AspNetUsers WHERE Id = @Id", new { Id = id });
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao excluir usuário: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===== MÉTODOS PARTIAL PARA AJAX =====

        [HttpGet]
        public async Task<IActionResult> CreatePartial()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Carregar todas as roles disponíveis
                var allRoles = await connection.QueryAsync(@"
                    SELECT Id, Name FROM AspNetRoles ORDER BY Name");

                var model = new CreateUserViewModel
                {
                    AvailableRoles = allRoles.Select(r => new RoleSelectionViewModel
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
        public async Task<IActionResult> CreatePartial([FromForm] CreateUserViewModel model)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                if (!ModelState.IsValid)
                {
                    // Recarregar roles disponíveis em caso de erro
                    var allRoles = await connection.QueryAsync(@"
                        SELECT Id, Name FROM AspNetRoles ORDER BY Name");

                    model.AvailableRoles = allRoles.Select(r => new RoleSelectionViewModel
                    {
                        Id = (string)r.Id,
                        Name = (string)r.Name,
                        IsSelected = model.SelectedRoles?.Contains((string)r.Name) ?? false
                    }).ToList();

                    return PartialView("_Create", model);
                }

                // Usar transação para garantir atomicidade e evitar locks
                using var transaction = connection.BeginTransaction();
                
                try
                {
                    var existingUser = await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                        "SELECT * FROM AspNetUsers WHERE Email = @Email OR UserName = @UserName",
                        new { Email = model.Email, UserName = model.Email }, transaction);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError(string.Empty, "Usuário com este email ou nome de usuário já existe.");
                        return PartialView("_Create", model);
                    }

                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    var userId = Guid.NewGuid().ToString();

                    var query = @"
                        INSERT INTO AspNetUsers (
                            Id, UserName, NormalizedUserName, Email, NormalizedEmail,
                            EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
                            PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                            LockoutEnd, LockoutEnabled, AccessFailedCount,
                            FirstName, LastName
                        ) VALUES (
                            @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                            @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp,
                            @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled,
                            @LockoutEnd, @LockoutEnabled, @AccessFailedCount,
                            @FirstName, @LastName
                        )";

                    await connection.QueryAsync(query, new
                    {
                        Id = userId,
                        UserName = model.Email,
                        NormalizedUserName = model.Email.ToUpper(),
                        Email = model.Email,
                        NormalizedEmail = model.Email.ToUpper(),
                        EmailConfirmed = false,
                        PasswordHash = hashedPassword,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        PhoneNumber = model.PhoneNumber ?? "",
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnd = (DateTimeOffset?)null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    }, transaction);

                    // Adicionar roles selecionadas ao usuário
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        foreach (var roleName in model.SelectedRoles)
                        {
                            // Buscar o ID da role pelo nome
                            var role = await connection.QuerySingleOrDefaultAsync(@"
                                SELECT Id FROM AspNetRoles WHERE Name = @RoleName", 
                                new { RoleName = roleName }, transaction);

                            if (role != null)
                            {
                                // Verificar se a associação já existe
                                var existingUserRole = await connection.QuerySingleOrDefaultAsync(@"
                                    SELECT UserId FROM AspNetUserRoles 
                                    WHERE UserId = @UserId AND RoleId = @RoleId",
                                    new { UserId = userId, RoleId = role.Id }, transaction);

                                if (existingUserRole == null)
                                {
                                    await connection.ExecuteAsync(@"
                                        INSERT INTO AspNetUserRoles (UserId, RoleId)
                                        VALUES (@UserId, @RoleId)",
                                        new { UserId = userId, RoleId = role.Id }, transaction);
                                }
                            }
                        }
                    }

                    // Adicionar claims selecionadas ao usuário
                    if (model.Claims != null && model.Claims.Any())
                    {
                        foreach (var claim in model.Claims)
                        {
                            if (!string.IsNullOrEmpty(claim.Type) && !string.IsNullOrEmpty(claim.Value))
                            {
                                // Verificar se a claim já existe
                                var existingClaim = await connection.QuerySingleOrDefaultAsync(@"
                                    SELECT Id FROM AspNetUserClaims 
                                    WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue",
                                    new { UserId = userId, ClaimType = claim.Type, ClaimValue = claim.Value }, transaction);

                                if (existingClaim == null)
                                {
                                    await connection.ExecuteAsync(@"
                                        INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                                        VALUES (@UserId, @ClaimType, @ClaimValue)",
                                        new { UserId = userId, ClaimType = claim.Type, ClaimValue = claim.Value }, transaction);
                                }
                            }
                        }
                    }

                    // Commit da transação
                    transaction.Commit();
                    return Json(new { success = true });
                }
                catch
                {
                    // Rollback em caso de erro
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Recarregar roles disponíveis em caso de erro
                try
                {
                    using var errorConnection = await _connectionFactory.CreateConnectionAsync();
                    var allRoles = await errorConnection.QueryAsync(@"
                        SELECT Id, Name FROM AspNetRoles ORDER BY Name");

                    model.AvailableRoles = allRoles.Select(r => new RoleSelectionViewModel
                    {
                        Id = (string)r.Id,
                        Name = (string)r.Name,
                        IsSelected = model.SelectedRoles?.Contains((string)r.Name) ?? false
                    }).ToList();
                }
                catch
                {
                    // Se falhar ao carregar roles, deixar lista vazia
                    model.AvailableRoles = new List<RoleSelectionViewModel>();
                }

                ModelState.AddModelError(string.Empty, $"Erro ao criar usuário: {ex.Message}");
                return PartialView("_Create", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var user = await GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Carregar todas as roles disponíveis
                var allRoles = await connection.QueryAsync(@"
                    SELECT Name FROM AspNetRoles ORDER BY Name");

                // Carregar roles do usuário
                var userRoles = await connection.QueryAsync(@"
                    SELECT r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                    WHERE ur.UserId = @UserId", 
                    new { UserId = id });

                // Carregar claims do usuário
                var userClaims = await connection.QueryAsync(@"
                    SELECT ClaimType as Type, ClaimValue as Value 
                    FROM AspNetUserClaims 
                    WHERE UserId = @UserId", 
                    new { UserId = id });

                var model = new EditUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    AvailableRoles = allRoles.Select(r => (string)r.Name).ToList(),
                    SelectedRoles = userRoles.Select(r => (string)r.Name).ToList(),
                    Claims = userClaims.Select(c => new ClaimViewModel
                    {
                        Type = (string)c.Type,
                        Value = (string)c.Value
                    }).ToList()
                };

                return PartialView("_Edit", model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no EditPartial GET: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar dados para edição: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial([FromForm] EditUserViewModel model)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                if (!ModelState.IsValid)
                {
                    // Recarregar dados em caso de erro
                    var allRoles = await connection.QueryAsync(@"
                        SELECT Name FROM AspNetRoles ORDER BY Name");
                    model.AvailableRoles = allRoles.Select(r => (string)r.Name).ToList();
                    
                    return PartialView("_Edit", model);
                }

                var query = @"
                    UPDATE AspNetUsers 
                    SET UserName = @UserName,
                        NormalizedUserName = @NormalizedUserName,
                        Email = @Email,
                        NormalizedEmail = @NormalizedEmail,
                        FirstName = @FirstName,
                        LastName = @LastName,
                        PhoneNumber = @PhoneNumber,
                        ConcurrencyStamp = @ConcurrencyStamp
                    WHERE Id = @Id";

                await connection.ExecuteAsync(query, new
                {
                    Id = model.Id,
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });

                // Atualizar roles do usuário
                // Primeiro, remover todas as roles atuais
                await connection.ExecuteAsync(@"
                    DELETE FROM AspNetUserRoles WHERE UserId = @UserId", 
                    new { UserId = model.Id });

                // Adicionar as roles selecionadas
                if (model.SelectedRoles != null && model.SelectedRoles.Any())
                {
                    foreach (var roleName in model.SelectedRoles)
                    {
                        var role = await connection.QuerySingleOrDefaultAsync(@"
                            SELECT Id FROM AspNetRoles WHERE Name = @RoleName", 
                            new { RoleName = roleName });

                        if (role != null)
                        {
                            await connection.ExecuteAsync(@"
                                INSERT INTO AspNetUserRoles (UserId, RoleId)
                                VALUES (@UserId, @RoleId)",
                                new { UserId = model.Id, RoleId = role.Id });
                        }
                    }
                }

                // Atualizar claims do usuário
                // Primeiro, remover todas as claims atuais
                await connection.ExecuteAsync(@"
                    DELETE FROM AspNetUserClaims WHERE UserId = @UserId", 
                    new { UserId = model.Id });

                // Adicionar as claims selecionadas
                if (model.Claims != null && model.Claims.Any())
                {
                    foreach (var claim in model.Claims)
                    {
                        if (!string.IsNullOrEmpty(claim.Type) && !string.IsNullOrEmpty(claim.Value))
                        {
                            await connection.ExecuteAsync(@"
                                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                                VALUES (@UserId, @ClaimType, @ClaimValue)",
                                new { UserId = model.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
                        }
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Recarregar dados em caso de erro
                try
                {
                    using var errorConnection = await _connectionFactory.CreateConnectionAsync();
                    var allRoles = await errorConnection.QueryAsync(@"
                        SELECT Name FROM AspNetRoles ORDER BY Name");
                    model.AvailableRoles = allRoles.Select(r => (string)r.Name).ToList();
                }
                catch
                {
                    model.AvailableRoles = new List<string>();
                }

                ModelState.AddModelError(string.Empty, $"Erro ao atualizar usuário: {ex.Message}");
                return PartialView("_Edit", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var user = await GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Carregar roles do usuário
                var userRoles = await connection.QueryAsync(@"
                    SELECT r.Name 
                    FROM AspNetRoles r
                    INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                    WHERE ur.UserId = @UserId", 
                    new { UserId = id });

                // Carregar claims do usuário
                var userClaims = await connection.QueryAsync(@"
                    SELECT ClaimType as Type, ClaimValue as Value 
                    FROM AspNetUserClaims 
                    WHERE UserId = @UserId", 
                    new { UserId = id });

                var model = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = string.Join(", ", userRoles.Select(r => (string)r.Name)),
                    Claims = userClaims.Select(c => new ClaimViewModel
                    {
                        Type = (string)c.Type,
                        Value = (string)c.Value
                    }).ToList()
                };

                return PartialView("_Details", model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no DetailsPartial: {ex.Message}");
                return StatusCode(500, $"Erro ao carregar detalhes do usuário: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeletePartial(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return PartialView("_Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePartialConfirmed(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Remove todas as relações primeiro
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUserRoles WHERE UserId = @Id",
                    new { Id = id });

                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUserClaims WHERE UserId = @Id",
                    new { Id = id });

                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUserLogins WHERE UserId = @Id",
                    new { Id = id });

                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUserTokens WHERE UserId = @Id",
                    new { Id = id });

                // Remove o usuário
                await connection.ExecuteAsync(
                    "DELETE FROM AspNetUsers WHERE Id = @Id",
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
                
                // Buscar usuários com suas roles em uma única query
                var usersWithRoles = await connection.QueryAsync(@"
                    SELECT 
                        u.Id,
                        u.UserName,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.PhoneNumber,
                        r.Name as RoleName
                    FROM AspNetUsers u
                    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
                    ORDER BY u.UserName");

                // Agrupar por usuário
                var userGroups = usersWithRoles.GroupBy(u => new
                {
                    Id = (string)u.Id,
                    UserName = (string)u.UserName,
                    Email = (string)u.Email,
                    FirstName = (string)u.FirstName,
                    LastName = (string)u.LastName,
                    PhoneNumber = (string)u.PhoneNumber
                });

                var userViewModels = new List<UserViewModel>();

                foreach (var group in userGroups)
                {
                    var roles = group
                        .Where(x => !string.IsNullOrEmpty((string)x.RoleName))
                        .Select(x => (string)x.RoleName)
                        .ToList();

                    userViewModels.Add(new UserViewModel
                    {
                        Id = group.Key.Id,
                        UserName = group.Key.UserName,
                        Email = group.Key.Email,
                        FirstName = group.Key.FirstName,
                        LastName = group.Key.LastName,
                        PhoneNumber = group.Key.PhoneNumber,
                        Roles = string.Join(", ", roles),
                        Claims = new List<ClaimViewModel>() // Claims não são necessárias na grid
                    });
                }

                return PartialView("_Grid", userViewModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao carregar grid de usuários: {ex.Message}");
            }
        }
    }
} 
