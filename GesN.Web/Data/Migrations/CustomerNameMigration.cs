using Dapper;
using Microsoft.Data.Sqlite;

namespace GesN.Web.Data.Migrations
{
    /// <summary>
    /// Migração para atualizar a estrutura da tabela Customer
    /// Converte o campo Name para FirstName e LastName
    /// </summary>
    public class CustomerNameMigration
    {
        private readonly string _connectionString;

        public CustomerNameMigration(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Executa a migração da estrutura da tabela Customer
        /// </summary>
        public void Execute()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Verificar se a migração já foi executada
            if (HasBeenMigrated(connection))
            {
                Console.WriteLine("CustomerNameMigration: Migração já foi executada anteriormente.");
                return;
            }

            Console.WriteLine("CustomerNameMigration: Iniciando migração da tabela Customer...");

            try
            {
                // 1. Criar tabela temporária com nova estrutura
                var createTempTable = @"
                    CREATE TABLE Customer_temp (
                        Id TEXT NOT NULL UNIQUE,
                        CreatedAt TEXT NOT NULL,
                        CreatedBy TEXT NOT NULL,
                        LastModifiedAt TEXT,
                        LastModifiedBy TEXT,
                        StateCode INTEGER NOT NULL DEFAULT 1,
                        FirstName TEXT NOT NULL,
                        LastName TEXT,
                        Email TEXT,
                        Phone TEXT,
                        DocumentNumber TEXT,
                        DocumentType TEXT,
                        GoogleContactId TEXT,
                        AddressId TEXT,
                        PRIMARY KEY(Id),
                        FOREIGN KEY(AddressId) REFERENCES Address(Id)
                    );";

                connection.Execute(createTempTable);
                Console.WriteLine("CustomerNameMigration: Tabela temporária criada.");

                // 2. Migrar dados existentes (se houver)
                var migrateData = @"
                    INSERT INTO Customer_temp (
                        Id, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy, 
                        StateCode, FirstName, LastName, Email, Phone, 
                        DocumentNumber, DocumentType, GoogleContactId, AddressId
                    )
                    SELECT 
                        Id, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy,
                        StateCode, 
                        CASE 
                            WHEN Name LIKE '% %' THEN TRIM(SUBSTR(Name, 1, INSTR(Name, ' ') - 1))
                            ELSE TRIM(Name)
                        END as FirstName,
                        CASE 
                            WHEN Name LIKE '% %' THEN TRIM(SUBSTR(Name, INSTR(Name, ' ') + 1))
                            ELSE NULL
                        END as LastName,
                        Email, Phone, DocumentNumber, DocumentType, GoogleContactId, AddressId
                    FROM Customer;";

                var migratedRows = connection.Execute(migrateData);
                Console.WriteLine($"CustomerNameMigration: {migratedRows} registros migrados.");

                // 3. Remover tabela antiga
                connection.Execute("DROP TABLE Customer;");
                Console.WriteLine("CustomerNameMigration: Tabela antiga removida.");

                // 4. Renomear tabela temporária
                connection.Execute("ALTER TABLE Customer_temp RENAME TO Customer;");
                Console.WriteLine("CustomerNameMigration: Tabela renomeada.");

                // 5. Marcar migração como executada
                MarkAsExecuted(connection);

                Console.WriteLine("CustomerNameMigration: Migração concluída com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CustomerNameMigration: Erro durante migração: {ex.Message}");
                
                // Tentar fazer rollback se possível
                try
                {
                    connection.Execute("DROP TABLE IF EXISTS Customer_temp;");
                    Console.WriteLine("CustomerNameMigration: Rollback executado.");
                }
                catch
                {
                    // Ignorar erros de rollback
                }
                
                throw;
            }
        }

        /// <summary>
        /// Verifica se a migração já foi executada
        /// </summary>
        private bool HasBeenMigrated(SqliteConnection connection)
        {
            try
            {
                // Verificar se existe a coluna FirstName na tabela Customer
                var sql = @"
                    SELECT COUNT(*) 
                    FROM pragma_table_info('Customer') 
                    WHERE name = 'FirstName';";

                var count = connection.QuerySingle<int>(sql);
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Marca a migração como executada
        /// </summary>
        private void MarkAsExecuted(SqliteConnection connection)
        {
            try
            {
                // Criar tabela de controle de migrações se não existir
                var createMigrationsTable = @"
                    CREATE TABLE IF NOT EXISTS __Migrations (
                        Id TEXT NOT NULL UNIQUE,
                        Name TEXT NOT NULL,
                        ExecutedAt TEXT NOT NULL,
                        PRIMARY KEY(Id)
                    );";

                connection.Execute(createMigrationsTable);

                // Inserir registro da migração
                var insertMigration = @"
                    INSERT OR IGNORE INTO __Migrations (Id, Name, ExecutedAt)
                    VALUES (@Id, @Name, @ExecutedAt);";

                connection.Execute(insertMigration, new
                {
                    Id = "001_CustomerNameMigration",
                    Name = "CustomerNameMigration",
                    ExecutedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CustomerNameMigration: Aviso - Não foi possível marcar migração: {ex.Message}");
            }
        }
    }
} 