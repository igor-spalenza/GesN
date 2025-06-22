using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SupplierRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE StateCode = @StateCode
                ORDER BY Name";

            return await connection.QueryAsync<Supplier>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<Supplier?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Supplier>(sql, new { Id = id });
        }

        public async Task<Supplier?> GetByNameAsync(string name)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE Name = @Name AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<Supplier>(sql, 
                new { Name = name, StateCode = (int)ObjectState.Active });
        }

        public async Task<Supplier?> GetByEmailAsync(string email)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE Email = @Email AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<Supplier>(sql, 
                new { Email = email, StateCode = (int)ObjectState.Active });
        }

        public async Task<Supplier?> GetByDocumentAsync(string documentNumber)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE DocumentNumber = @DocumentNumber AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<Supplier>(sql, 
                new { DocumentNumber = documentNumber, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Supplier>> GetActiveAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE StateCode = @StateCode AND IsActive = 1
                ORDER BY Name";

            return await connection.QueryAsync<Supplier>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Supplier>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE StateCode = @StateCode
                  AND (Name LIKE @SearchTerm
                       OR CompanyName LIKE @SearchTerm
                       OR Email LIKE @SearchTerm
                       OR DocumentNumber LIKE @SearchTerm
                       OR Phone LIKE @SearchTerm)
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<Supplier>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<IEnumerable<Supplier>> SearchForAutocompleteAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE StateCode = @StateCode
                  AND IsActive = 1
                  AND (Name LIKE @SearchTerm
                       OR CompanyName LIKE @SearchTerm)
                ORDER BY 
                    CASE 
                        WHEN Name LIKE @ExactStart THEN 1
                        WHEN CompanyName LIKE @ExactStart THEN 2
                        WHEN Name LIKE @SearchTerm THEN 3
                        WHEN CompanyName LIKE @SearchTerm THEN 4
                        ELSE 5
                    END,
                    Name
                LIMIT 10";

            var searchPattern = $"%{searchTerm}%";
            var exactStartPattern = $"{searchTerm}%";
            
            return await connection.QueryAsync<Supplier>(sql, 
                new { 
                    StateCode = (int)ObjectState.Active, 
                    SearchTerm = searchPattern,
                    ExactStart = exactStartPattern
                });
        }

        public async Task<string> CreateAsync(Supplier supplier)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO Supplier (Id, Name, CompanyName, DocumentNumber, DocumentType, Email, Phone, AddressId, IsActive, StateCode, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
                VALUES (@Id, @Name, @CompanyName, @DocumentNumber, @DocumentType, @Email, @Phone, @AddressId, @IsActive, @StateCode, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy)";
            
            await connection.ExecuteAsync(sql, supplier);
            return supplier.Id;
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Supplier SET 
                Name = @Name, CompanyName = @CompanyName, DocumentNumber = @DocumentNumber, DocumentType = @DocumentType, 
                Email = @Email, Phone = @Phone, AddressId = @AddressId, IsActive = @IsActive,
                StateCode = @StateCode, LastModifiedAt = @LastModifiedAt, LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, supplier);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE Supplier SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM Supplier WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM Supplier WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Supplier>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Supplier 
                WHERE StateCode = @StateCode
                ORDER BY Name
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<Supplier>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }
    }
} 