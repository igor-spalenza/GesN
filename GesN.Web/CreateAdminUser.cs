using GesN.Web.Areas.Identity.Data.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace GesN.Web
{
    public class CreateAdminUser
    {
        public static void Main()
        {
            // Caminho para o banco de dados SQLite
            string connectionString = "Data Source=./Data/Database/gesn.db";

            try
            {
                // Verificar se a role Admin existe, se não, criar
                if (!RoleExists(connectionString, "Admin"))
                {
                    CreateRole(connectionString, "Admin");
                    Console.WriteLine("Role 'Admin' criada com sucesso.");
                }
                else
                {
                    Console.WriteLine("Role 'Admin' já existe.");
                }

                // Buscar o usuário pelo nome de usuário
                string userName = "igor.spalenza94@gmail.com";
                string userId = GetUserIdByName(connectionString, userName);

                if (!string.IsNullOrEmpty(userId))
                {
                    // Verificar se o usuário já tem a role
                    if (!UserIsInRole(connectionString, userId, "Admin"))
                    {
                        // Adicionar usuário à role
                        AddUserToRole(connectionString, userId, "Admin");
                        Console.WriteLine($"Usuário '{userName}' adicionado à role 'Admin' com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine($"Usuário '{userName}' já possui a role 'Admin'.");
                    }
                }
                else
                {
                    Console.WriteLine($"Usuário '{userName}' não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }

            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        private static bool RoleExists(string connectionString, string roleName)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM AspNetRoles WHERE NormalizedName = @NormalizedName";
                command.Parameters.AddWithValue("@NormalizedName", roleName.ToUpper());
                
                var count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private static void CreateRole(string connectionString, string roleName)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
                command.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@Name", roleName);
                command.Parameters.AddWithValue("@NormalizedName", roleName.ToUpper());
                command.Parameters.AddWithValue("@ConcurrencyStamp", Guid.NewGuid().ToString());
                
                command.ExecuteNonQuery();
            }
        }

        private static string GetUserIdByName(string connectionString, string userName)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName OR NormalizedEmail = @NormalizedEmail";
                command.Parameters.AddWithValue("@NormalizedUserName", userName.ToUpper());
                command.Parameters.AddWithValue("@NormalizedEmail", userName.ToUpper());
                
                var result = command.ExecuteScalar();
                return result?.ToString();
            }
        }

        private static bool UserIsInRole(string connectionString, string userId, string roleName)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT COUNT(*) 
                    FROM AspNetUserRoles ur
                    JOIN AspNetRoles r ON ur.RoleId = r.Id
                    WHERE ur.UserId = @UserId AND r.NormalizedName = @NormalizedName";
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@NormalizedName", roleName.ToUpper());
                
                var count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private static void AddUserToRole(string connectionString, string userId, string roleName)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                
                // Primeiro, obter o ID da role
                var getRoleIdCommand = connection.CreateCommand();
                getRoleIdCommand.CommandText = "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedName";
                getRoleIdCommand.Parameters.AddWithValue("@NormalizedName", roleName.ToUpper());
                
                var roleId = getRoleIdCommand.ExecuteScalar()?.ToString();
                
                if (!string.IsNullOrEmpty(roleId))
                {
                    // Agora, adicionar o usuário à role
                    var addToRoleCommand = connection.CreateCommand();
                    addToRoleCommand.CommandText = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                    addToRoleCommand.Parameters.AddWithValue("@UserId", userId);
                    addToRoleCommand.Parameters.AddWithValue("@RoleId", roleId);
                    
                    addToRoleCommand.ExecuteNonQuery();
                }
                else
                {
                    throw new Exception($"Role '{roleName}' não encontrada.");
                }
            }
        }
    }
} 