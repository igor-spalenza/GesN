using Microsoft.Data.Sqlite;
using System;

namespace GesN.Web
{
    public class AddAdminRole
    {
        public static void Main(string[] args)
        {
            // Caminho para o banco de dados SQLite
            string connectionString = "Data Source=Data/Database/gesn.db";
            string userName = "igor.spalenza94@gmail.com";

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    
                    // 1. Verificar se a role Admin existe, se não, criar
                    string roleId = null;
                    var checkRoleCommand = connection.CreateCommand();
                    checkRoleCommand.CommandText = "SELECT Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN'";
                    
                    var roleIdResult = checkRoleCommand.ExecuteScalar();
                    if (roleIdResult == null)
                    {
                        // Criar a role Admin
                        roleId = Guid.NewGuid().ToString();
                        var createRoleCommand = connection.CreateCommand();
                        createRoleCommand.CommandText = @"
                            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
                            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
                        createRoleCommand.Parameters.AddWithValue("@Id", roleId);
                        createRoleCommand.Parameters.AddWithValue("@Name", "Admin");
                        createRoleCommand.Parameters.AddWithValue("@NormalizedName", "ADMIN");
                        createRoleCommand.Parameters.AddWithValue("@ConcurrencyStamp", Guid.NewGuid().ToString());
                        
                        createRoleCommand.ExecuteNonQuery();
                        Console.WriteLine("Role Admin criada com sucesso.");
                    }
                    else
                    {
                        roleId = roleIdResult.ToString();
                        Console.WriteLine("Role Admin já existe.");
                    }
                    
                    // 2. Verificar se o usuário existe
                    var checkUserCommand = connection.CreateCommand();
                    checkUserCommand.CommandText = "SELECT Id FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName OR NormalizedEmail = @NormalizedEmail";
                    checkUserCommand.Parameters.AddWithValue("@NormalizedUserName", userName.ToUpper());
                    checkUserCommand.Parameters.AddWithValue("@NormalizedEmail", userName.ToUpper());
                    
                    var userIdResult = checkUserCommand.ExecuteScalar();
                    if (userIdResult == null)
                    {
                        Console.WriteLine($"Usuário {userName} não encontrado no banco de dados.");
                        return;
                    }
                    
                    string userId = userIdResult.ToString();
                    
                    // 3. Verificar se o usuário já tem a role
                    var checkUserRoleCommand = connection.CreateCommand();
                    checkUserRoleCommand.CommandText = "SELECT COUNT(*) FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
                    checkUserRoleCommand.Parameters.AddWithValue("@UserId", userId);
                    checkUserRoleCommand.Parameters.AddWithValue("@RoleId", roleId);
                    
                    int userRoleCount = Convert.ToInt32(checkUserRoleCommand.ExecuteScalar());
                    
                    if (userRoleCount == 0)
                    {
                        // 4. Adicionar o usuário à role
                        var addUserRoleCommand = connection.CreateCommand();
                        addUserRoleCommand.CommandText = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                        addUserRoleCommand.Parameters.AddWithValue("@UserId", userId);
                        addUserRoleCommand.Parameters.AddWithValue("@RoleId", roleId);
                        
                        addUserRoleCommand.ExecuteNonQuery();
                        Console.WriteLine($"Usuário {userName} adicionado à role Admin com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine($"Usuário {userName} já possui a role Admin.");
                    }
                }
                
                Console.WriteLine("\nOperação concluída com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            
            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
} 