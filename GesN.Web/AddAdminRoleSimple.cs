using Microsoft.Data.Sqlite;
using System;

namespace GesN.Web
{
    public class AddAdminRoleSimple
    {
        public static void Main()
        {
            try
            {
                // Caminho para o banco de dados SQLite
                string connectionString = "Data Source=Data/Database/gesn.db";
                string email = "igor.spalenza94@gmail.com";
                
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Conexão com o banco de dados estabelecida.");
                    
                    // 1. Criar a role Admin
                    string roleId = Guid.NewGuid().ToString();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
                        command.Parameters.AddWithValue("@Id", roleId);
                        command.Parameters.AddWithValue("@Name", "Admin");
                        command.Parameters.AddWithValue("@NormalizedName", "ADMIN");
                        command.Parameters.AddWithValue("@ConcurrencyStamp", Guid.NewGuid().ToString());
                        
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                            Console.WriteLine("Role Admin criada com sucesso.");
                        else
                            Console.WriteLine("Role Admin já existe.");
                    }
                    
                    // 2. Obter o ID da role Admin (caso já existisse)
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN'";
                        var result = command.ExecuteScalar();
                        if (result != null)
                            roleId = result.ToString();
                    }
                    
                    // 3. Obter o ID do usuário
                    string userId = null;
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT Id FROM AspNetUsers WHERE NormalizedEmail = @Email";
                        command.Parameters.AddWithValue("@Email", email.ToUpper());
                        var result = command.ExecuteScalar();
                        
                        if (result != null)
                            userId = result.ToString();
                        else
                        {
                            Console.WriteLine($"Usuário com email {email} não encontrado.");
                            return;
                        }
                    }
                    
                    // 4. Adicionar o usuário à role Admin
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId)
                            VALUES (@UserId, @RoleId)";
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@RoleId", roleId);
                        
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                            Console.WriteLine($"Usuário {email} adicionado à role Admin com sucesso.");
                        else
                            Console.WriteLine($"Usuário {email} já está na role Admin.");
                    }
                }
                
                Console.WriteLine("Operação concluída com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
} 