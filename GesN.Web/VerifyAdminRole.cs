using Microsoft.Data.Sqlite;
using System;

namespace GesN.Web
{
    public class VerifyAdminRole
    {
        public static void Main()
        {
            // Caminho para o banco de dados SQLite
            string connectionString = "Data Source=Data/Database/gesn.db";

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    
                    // Verificar se a role Admin existe
                    var roleCommand = connection.CreateCommand();
                    roleCommand.CommandText = "SELECT Id, Name FROM AspNetRoles WHERE NormalizedName = 'ADMIN'";
                    
                    using (var reader = roleCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string roleId = reader.GetString(0);
                            string roleName = reader.GetString(1);
                            Console.WriteLine($"Role encontrada: {roleName} (ID: {roleId})");
                            
                            // Verificar usuários com esta role
                            var userRoleCommand = connection.CreateCommand();
                            userRoleCommand.CommandText = @"
                                SELECT u.Id, u.UserName, u.Email 
                                FROM AspNetUsers u
                                JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                                WHERE ur.RoleId = @RoleId";
                            userRoleCommand.Parameters.AddWithValue("@RoleId", roleId);
                            
                            using (var userReader = userRoleCommand.ExecuteReader())
                            {
                                bool foundUsers = false;
                                
                                while (userReader.Read())
                                {
                                    foundUsers = true;
                                    string userId = userReader.GetString(0);
                                    string userName = userReader.GetString(1);
                                    string email = userReader.GetString(2);
                                    
                                    Console.WriteLine($"Usuário com role Admin: {userName} ({email})");
                                }
                                
                                if (!foundUsers)
                                {
                                    Console.WriteLine("Nenhum usuário com a role Admin encontrado.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Role Admin não encontrada no banco de dados.");
                        }
                    }
                    
                    // Verificar especificamente o usuário igor.spalenza94@gmail.com
                    var specificUserCommand = connection.CreateCommand();
                    specificUserCommand.CommandText = "SELECT Id, UserName, Email FROM AspNetUsers WHERE NormalizedEmail = 'IGOR.SPALENZA94@GMAIL.COM'";
                    
                    using (var reader = specificUserCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userId = reader.GetString(0);
                            string userName = reader.GetString(1);
                            string email = reader.GetString(2);
                            
                            Console.WriteLine($"\nUsuário encontrado: {userName} ({email})");
                            
                            // Verificar roles deste usuário
                            var userRolesCommand = connection.CreateCommand();
                            userRolesCommand.CommandText = @"
                                SELECT r.Name
                                FROM AspNetRoles r
                                JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                                WHERE ur.UserId = @UserId";
                            userRolesCommand.Parameters.AddWithValue("@UserId", userId);
                            
                            using (var rolesReader = userRolesCommand.ExecuteReader())
                            {
                                bool foundRoles = false;
                                
                                while (rolesReader.Read())
                                {
                                    foundRoles = true;
                                    string roleName = rolesReader.GetString(0);
                                    Console.WriteLine($"- Role: {roleName}");
                                }
                                
                                if (!foundRoles)
                                {
                                    Console.WriteLine("- Usuário não possui nenhuma role atribuída.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nUsuário igor.spalenza94@gmail.com não encontrado no banco de dados.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar roles: {ex.Message}");
            }
            
            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
} 