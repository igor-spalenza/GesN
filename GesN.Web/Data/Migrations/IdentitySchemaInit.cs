using Microsoft.Data.Sqlite;

namespace GesN.Web.Data.Migrations
{
    public class IdentitySchemaInit
    {
        private readonly string _connectionString;

        public IdentitySchemaInit(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Método utilitário para reset manual do banco (usar com cuidado)
        /// </summary>
        public void ResetDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                // Desabilitar foreign keys temporariamente
                var disableFKCommand = connection.CreateCommand();
                disableFKCommand.CommandText = "PRAGMA foreign_keys = OFF;";
                disableFKCommand.ExecuteNonQuery();

                // Limpar tabelas na ordem correta
                var tables = new[]
                {
                    "AspNetUserTokens",
                    "AspNetUserRoles", 
                    "AspNetUserLogins",
                    "AspNetUserClaims",
                    "AspNetRoleClaims",
                    "AspNetUsers",
                    "AspNetRoles"
                };

                foreach (var table in tables)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = $"DELETE FROM {table};";
                    command.ExecuteNonQuery();
                }

                // Reabilitar foreign keys
                var enableFKCommand = connection.CreateCommand();
                enableFKCommand.CommandText = "PRAGMA foreign_keys = ON;";
                enableFKCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Verifica se as tabelas do Identity estão vazias
        /// </summary>
        public bool IsDatabaseEmpty()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                
                var checkQuery = @"
                    SELECT 
                        (SELECT COUNT(*) FROM AspNetUsers) + 
                        (SELECT COUNT(*) FROM AspNetRoles) + 
                        (SELECT COUNT(*) FROM AspNetUserClaims) + 
                        (SELECT COUNT(*) FROM AspNetRoleClaims) + 
                        (SELECT COUNT(*) FROM AspNetUserRoles) AS TotalRecords";
                
                using (var command = new SqliteCommand(checkQuery, connection))
                {
                    var totalRecords = Convert.ToInt32(command.ExecuteScalar());
                    return totalRecords == 0;
                }
            }
        }

        public void Initialize()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                Console.WriteLine("Iniciando criação/verificação das tabelas do Identity...");

                var createRolesTable = @"
                CREATE TABLE IF NOT EXISTS AspNetRoles (
                    Id TEXT PRIMARY KEY,
                    Name TEXT,
                    NormalizedName TEXT,
                    ConcurrencyStamp TEXT,
                    CreatedDate TEXT DEFAULT (datetime('now'))
                );";

                var createUsersTable = @"
                CREATE TABLE IF NOT EXISTS AspNetUsers (
                    Id TEXT PRIMARY KEY,
                    UserName TEXT,
                    NormalizedUserName TEXT,
                    Email TEXT,
                    NormalizedEmail TEXT,
                    EmailConfirmed BOOLEAN,
                    PasswordHash TEXT,
                    SecurityStamp TEXT,
                    ConcurrencyStamp TEXT,
                    PhoneNumber TEXT,
                    PhoneNumberConfirmed BOOLEAN,
                    TwoFactorEnabled BOOLEAN,
                    LockoutEnd TEXT,
                    LockoutEnabled BOOLEAN,
                    AccessFailedCount INTEGER,
                    FirstName TEXT,
                    LastName TEXT,
                    CreatedDate TEXT DEFAULT (datetime('now'))
                );";

                var createRoleClaimsTable = @"
                CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RoleId TEXT NOT NULL,
                    ClaimType TEXT,
                    ClaimValue TEXT,
                    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
                );";

                var createUserClaimsTable = @"
                CREATE TABLE IF NOT EXISTS AspNetUserClaims (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId TEXT NOT NULL,
                    ClaimType TEXT,
                    ClaimValue TEXT,
                    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
                );";

                var createUserLoginsTable = @"
                CREATE TABLE IF NOT EXISTS AspNetUserLogins (
                    LoginProvider TEXT NOT NULL,
                    ProviderKey TEXT NOT NULL,
                    ProviderDisplayName TEXT,
                    UserId TEXT NOT NULL,
                    PRIMARY KEY (LoginProvider, ProviderKey),
                    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
                );";

                var createUserRolesTable = @"
                CREATE TABLE IF NOT EXISTS AspNetUserRoles (
                    UserId TEXT NOT NULL,
                    RoleId TEXT NOT NULL,
                    PRIMARY KEY (UserId, RoleId),
                    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE,
                    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
                );";

                var createUserTokensTable = @"
                CREATE TABLE IF NOT EXISTS AspNetUserTokens (
                    UserId TEXT NOT NULL,
                    LoginProvider TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Value TEXT,
                    PRIMARY KEY (UserId, LoginProvider, Name),
                    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
                );";

                using (var command = new SqliteCommand(createRolesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SqliteCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SqliteCommand(createRoleClaimsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SqliteCommand(createUserClaimsTable, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Tabela AspNetUserClaims verificada/criada com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao criar/verificar tabela AspNetUserClaims: {ex.Message}");
                        throw;
                    }
                }
                using (var command = new SqliteCommand(createUserLoginsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SqliteCommand(createUserRolesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SqliteCommand(createUserTokensTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Adicionar colunas FirstName, LastName e CreatedDate se elas não existirem
                try
                {
                    var checkColumnQuery = @"
                    PRAGMA table_info(AspNetUsers);
                    ";

                    bool hasFirstName = false;
                    bool hasLastName = false;
                    bool hasCreatedDate = false;

                    using (var command = new SqliteCommand(checkColumnQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader.GetString(1); // O índice 1 contém o nome da coluna
                                if (columnName == "FirstName")
                                    hasFirstName = true;
                                else if (columnName == "LastName")
                                    hasLastName = true;
                                else if (columnName == "CreatedDate")
                                    hasCreatedDate = true;
                            }
                        }
                    }

                    if (!hasFirstName)
                    {
                        var addFirstNameQuery = @"
                        ALTER TABLE AspNetUsers ADD COLUMN FirstName TEXT;
                        ";
                        using (var command = new SqliteCommand(addFirstNameQuery, connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine("Coluna FirstName adicionada à tabela AspNetUsers.");
                        }
                    }

                    if (!hasLastName)
                    {
                        var addLastNameQuery = @"
                        ALTER TABLE AspNetUsers ADD COLUMN LastName TEXT;
                        ";
                        using (var command = new SqliteCommand(addLastNameQuery, connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine("Coluna LastName adicionada à tabela AspNetUsers.");
                        }
                    }

                    if (!hasCreatedDate)
                    {
                        var addCreatedDateQuery = @"
                        ALTER TABLE AspNetUsers ADD COLUMN CreatedDate TEXT DEFAULT (datetime('now'));
                        ";
                        using (var command = new SqliteCommand(addCreatedDateQuery, connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine("Coluna CreatedDate adicionada à tabela AspNetUsers.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao verificar/adicionar colunas: {ex.Message}");
                }

                // Verificar/adicionar CreatedDate em AspNetRoles
                try
                {
                    var checkRolesColumnQuery = @"PRAGMA table_info(AspNetRoles);";
                    bool roleHasCreatedDate = false;
                    using (var command = new SqliteCommand(checkRolesColumnQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(1) == "CreatedDate")
                            {
                                roleHasCreatedDate = true;
                                break;
                            }
                        }
                    }

                    if (!roleHasCreatedDate)
                    {
                        var addRoleCreatedQuery = @"
                        ALTER TABLE AspNetRoles ADD COLUMN CreatedDate TEXT DEFAULT (datetime('now'));
                        ";
                        using (var command = new SqliteCommand(addRoleCreatedQuery, connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine("Coluna CreatedDate adicionada à tabela AspNetRoles.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao verificar/adicionar coluna CreatedDate em AspNetRoles: {ex.Message}");
                }
            }
        }
    }
}
