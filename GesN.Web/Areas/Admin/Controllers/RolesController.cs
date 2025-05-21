using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly IDbConnection _dbConnection;

        public RolesController(ProjectDataContext context)
        {
            _dbConnection = context.Connection;
        }

        public async Task<IActionResult> Index()
        {
            // Obter todas as roles diretamente do banco de dados em vez de usar _roleManager.Roles
            var roles = await GetAllRolesAsync();
            return View(roles);
        }

        private async Task<List<ApplicationRole>> GetAllRolesAsync()
        {
            var query = "SELECT * FROM AspNetRoles";
            var roles = await _dbConnection.QueryAsync<ApplicationRole>(query);
            return roles.ToList();
        }

        public IActionResult Create()
        {
            return View();
        }

        // Nova action para criar roles de forma simples
        public IActionResult CreateSimple()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSimple(string roleName)
        {
            try
            {
                // Verificar se o nome foi fornecido
                if (string.IsNullOrEmpty(roleName))
                {
                    ViewBag.ErrorMessage = "O nome da role é obrigatório.";
                    return View();
                }

                // Verificar se a role já existe
                var existingRole = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                    "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                    new { NormalizedName = roleName.ToUpper() });

                if (existingRole != null)
                {
                    ViewBag.ErrorMessage = $"Role '{roleName}' já existe.";
                    return View();
                }

                // Criar a role
                var roleId = Guid.NewGuid().ToString();
                var query = @"
                    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                    VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

                await _dbConnection.ExecuteAsync(query, new
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });

                ViewBag.SuccessMessage = $"Role '{roleName}' criada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Erro ao criar role: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            // Verificar o estado do ModelState para diagnóstico
            if (!ModelState.IsValid)
            {
                // Adicionar diagnóstico ao ViewBag para entender o que está falhando
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add($"{state.Key}: {error.ErrorMessage}");
                    }
                }
                ViewBag.ValidationErrors = errors;
            }

            // Continuar mesmo se ModelState for inválido (para diagnóstico)
            try
            {
                // Verificar se o nome foi fornecido
                if (string.IsNullOrEmpty(model.Name))
                {
                    ModelState.AddModelError("Name", "O nome da role é obrigatório");
                    return View(model);
                }

                // Verificar se a role já existe
                var existingRole = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                    "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                    new { NormalizedName = model.Name.ToUpper() });

                if (existingRole != null)
                {
                    ModelState.AddModelError(string.Empty, $"Role '{model.Name}' já existe.");
                    return View(model);
                }

                // Criar a role
                var roleId = Guid.NewGuid().ToString();
                var query = @"
                    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                    VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

                await _dbConnection.ExecuteAsync(query, new
                {
                    Id = roleId,
                    Name = model.Name,
                    NormalizedName = model.Name.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao criar role: {ex.Message}");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                "SELECT * FROM AspNetRoles WHERE Id = @Id",
                new { Id = id });

            if (role == null)
            {
                return NotFound();
            }

            var model = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RoleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar se a role existe
                    var existingRole = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                        "SELECT * FROM AspNetRoles WHERE Id = @Id",
                        new { Id = id });

                    if (existingRole == null)
                    {
                        return NotFound();
                    }

                    // Verificar se o novo nome já está em uso
                    if (existingRole.Name != model.Name)
                    {
                        var nameExists = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                            "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName AND Id != @Id",
                            new { NormalizedName = model.Name.ToUpper(), Id = id });

                        if (nameExists != null)
                        {
                            ModelState.AddModelError(string.Empty, $"Role '{model.Name}' já existe.");
                            return View(model);
                        }
                    }

                    // Atualizar a role
                    var query = @"
                        UPDATE AspNetRoles 
                        SET Name = @Name,
                            NormalizedName = @NormalizedName,
                            ConcurrencyStamp = @ConcurrencyStamp
                        WHERE Id = @Id";

                    await _dbConnection.ExecuteAsync(query, new
                    {
                        Id = id,
                        Name = model.Name,
                        NormalizedName = model.Name.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erro ao atualizar role: {ex.Message}");
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

            var role = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                "SELECT * FROM AspNetRoles WHERE Id = @Id",
                new { Id = id });

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                // Verificar se a role existe
                var role = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                    "SELECT * FROM AspNetRoles WHERE Id = @Id",
                    new { Id = id });

                if (role == null)
                {
                    return NotFound();
                }

                // Primeiro remover todas as atribuições de usuários a esta role
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetUserRoles WHERE RoleId = @RoleId",
                    new { RoleId = id });

                // Depois remover a role
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetRoles WHERE Id = @Id",
                    new { Id = id });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao excluir role: {ex.Message}");
                
                var role = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                    "SELECT * FROM AspNetRoles WHERE Id = @Id",
                    new { Id = id });
                
                return View(role);
            }
        }
    }

    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "O nome da role é obrigatório")]
        [Display(Name = "Nome da Role")]
        public string Name { get; set; }
    }
} 