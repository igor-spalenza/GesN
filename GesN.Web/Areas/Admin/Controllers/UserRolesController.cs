using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserRolesController : Controller
    {
        private readonly IDbConnection _dbConnection;

        public UserRolesController(ProjectDataContext context)
        {
            _dbConnection = context.Connection;
        }

        public async Task<IActionResult> Index(string userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            var userRoles = await GetUserRolesAsync(userId);
            var allRoles = await GetAllRolesAsync();

            foreach (var role in allRoles)
            {
                model.Roles.Add(new RoleAssignmentViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = userRoles.Contains(role.Name)
                });
            }

            return View(model);
        }

        private async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id",
                new { Id = userId }
            );
        }

        private async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var query = @"
                SELECT r.Name
                FROM AspNetRoles r
                INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId";

            var roles = await _dbConnection.QueryAsync<string>(query, new { UserId = userId });
            return roles.ToList();
        }

        private async Task<List<ApplicationRole>> GetAllRolesAsync()
        {
            var query = "SELECT * FROM AspNetRoles";
            var roles = await _dbConnection.QueryAsync<ApplicationRole>(query);
            return roles.ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserRolesViewModel model)
        {
            var user = await GetUserByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Remover todas as roles do usuário
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetUserRoles WHERE UserId = @UserId",
                    new { UserId = model.UserId }
                );

                // Adicionar as roles selecionadas
                foreach (var role in model.Roles.Where(r => r.IsSelected))
                {
                    // Obter o ID da role pelo nome
                    var roleId = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                        "SELECT Id FROM AspNetRoles WHERE Name = @Name",
                        new { Name = role.RoleName }
                    );

                    if (!string.IsNullOrEmpty(roleId))
                    {
                        await _dbConnection.ExecuteAsync(
                            "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                            new { UserId = model.UserId, RoleId = roleId }
                        );
                    }
                }

                return RedirectToAction("Details", "Users", new { id = model.UserId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao atualizar roles do usuário: {ex.Message}");
                return View("Index", model);
            }
        }
    }

    public class UserRolesViewModel
    {
        public UserRolesViewModel()
        {
            Roles = new List<RoleAssignmentViewModel>();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<RoleAssignmentViewModel> Roles { get; set; }
    }

    public class RoleAssignmentViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
} 