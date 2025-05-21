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
    public class UsersController : Controller
    {
        private readonly IDbConnection _dbConnection;

        public UsersController(ProjectDataContext context)
        {
            _dbConnection = context.Connection;
        }

        public async Task<IActionResult> Index()
        {
            // Obter todos os usuários diretamente do banco de dados
            var users = await GetAllUsersAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await GetUserRolesAsync(user.Id);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userViewModels);
        }

        private async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            var query = "SELECT * FROM AspNetUsers";
            var users = await _dbConnection.QueryAsync<ApplicationUser>(query);
            return users.ToList();
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
                PhoneNumber = user.PhoneNumber,
                Roles = string.Join(", ", roles)
            };

            return View(viewModel);
        }

        private async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id",
                new { Id = id }
            );
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

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber
            };

            return View(viewModel);
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
                    var user = await GetUserByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Atualizar o usuário diretamente no banco de dados
                    var query = @"
                        UPDATE AspNetUsers SET 
                            UserName = @UserName,
                            NormalizedUserName = @NormalizedUserName,
                            Email = @Email,
                            NormalizedEmail = @NormalizedEmail,
                            PhoneNumber = @PhoneNumber
                        WHERE Id = @Id";

                    await _dbConnection.ExecuteAsync(query, new
                    {
                        Id = id,
                        UserName = model.UserName,
                        NormalizedUserName = model.UserName.ToUpper(),
                        Email = model.Email,
                        NormalizedEmail = model.Email.ToUpper(),
                        PhoneNumber = model.PhoneNumber
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

            var roles = await GetUserRolesAsync(id);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Roles = string.Join(", ", roles)
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var user = await GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Primeiro remover todas as relações com roles
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetUserRoles WHERE UserId = @UserId",
                    new { UserId = id });

                // Remover tokens
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetUserTokens WHERE UserId = @UserId",
                    new { UserId = id });

                // Remover logins
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetUserLogins WHERE UserId = @UserId",
                    new { UserId = id });

                // Remover claims
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetUserClaims WHERE UserId = @UserId",
                    new { UserId = id });

                // Finalmente remover o usuário
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM AspNetUsers WHERE Id = @Id",
                    new { Id = id });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao excluir usuário: {ex.Message}");
                
                var user = await GetUserByIdAsync(id);
                var roles = await GetUserRolesAsync(id);
                var viewModel = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = string.Join(", ", roles)
                };

                return View(viewModel);
            }
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Roles { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
} 