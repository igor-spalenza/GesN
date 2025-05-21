using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GesN.Web
{
    public static class CreateAdminRole
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var projectDataContext = serviceProvider.GetRequiredService<ProjectDataContext>();
            
            // Criar a role Admin se não existir
            string roleName = "Admin";
            
            using (var dbConnection = projectDataContext.Connection)
            {
                // Verificar se a role já existe
                var roleExists = await dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                    "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                    new { NormalizedName = roleName.ToUpper() });
                
                if (roleExists == null)
                {
                    // Criar a role
                    var roleId = Guid.NewGuid().ToString();
                    var query = @"
                        INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                        VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
                    
                    await dbConnection.ExecuteAsync(query, new
                    {
                        Id = roleId,
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                    
                    Console.WriteLine($"Role '{roleName}' criada com sucesso.");
                }
                else
                {
                    Console.WriteLine($"Role '{roleName}' já existe.");
                }
            }

            // Buscar o usuário específico
            string userName = "igor.spalenza94@gmail.com";
            var user = await userManager.FindByNameAsync(userName);
            
            if (user != null)
            {
                // Verificar se o usuário já tem a role
                var isInRole = await userManager.IsInRoleAsync(user, "Admin");
                if (!isInRole)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine($"Role 'Admin' atribuída ao usuário {user.UserName}.");
                }
                else
                {
                    Console.WriteLine($"Usuário {user.UserName} já possui a role 'Admin'.");
                }
            }
            else
            {
                Console.WriteLine($"Usuário {userName} não encontrado no banco de dados.");
            }
        }
    }
} 