using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public IngredientRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Ingredient>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE StateCode = @StateCode
                ORDER BY Name";

            return await connection.QueryAsync<Ingredient>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<Ingredient?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Ingredient>(sql, new { Id = id });
        }

        public async Task<Ingredient?> GetByNameAsync(string name)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE Name = @Name AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<Ingredient>(sql, 
                new { Name = name, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Ingredient>> GetActiveAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE StateCode = @StateCode AND IsActive = 1
                ORDER BY Name";

            return await connection.QueryAsync<Ingredient>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Ingredient>> GetBySupplierId(string supplierId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE SupplierId = @SupplierId AND StateCode = @StateCode
                ORDER BY Name";

            return await connection.QueryAsync<Ingredient>(sql, 
                new { SupplierId = supplierId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Ingredient>> GetLowStockAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE StateCode = @StateCode 
                  AND IsActive = 1
                  AND CurrentStock <= MinStock
                ORDER BY Name";

            return await connection.QueryAsync<Ingredient>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Ingredient>> GetPerishableAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE StateCode = @StateCode 
                  AND IsActive = 1
                  AND ExpirationDays IS NOT NULL 
                  AND ExpirationDays > 0
                ORDER BY ExpirationDays ASC, Name";

            return await connection.QueryAsync<Ingredient>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Ingredient>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE StateCode = @StateCode
                  AND (Name LIKE @SearchTerm
                       OR Description LIKE @SearchTerm
                       OR Unit LIKE @SearchTerm)
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<Ingredient>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<IEnumerable<Ingredient>> SearchForAutocompleteAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE StateCode = @StateCode
                  AND IsActive = 1
                  AND Name LIKE @SearchTerm
                ORDER BY 
                    CASE 
                        WHEN Name LIKE @ExactStart THEN 1
                        WHEN Name LIKE @SearchTerm THEN 2
                        ELSE 3
                    END,
                    Name
                LIMIT 10";

            var searchPattern = $"%{searchTerm}%";
            var exactStartPattern = $"{searchTerm}%";
            
            return await connection.QueryAsync<Ingredient>(sql, 
                new { 
                    StateCode = (int)ObjectState.Active, 
                    SearchTerm = searchPattern,
                    ExactStart = exactStartPattern
                });
        }

        public async Task<string> CreateAsync(Ingredient ingredient)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO Ingredient (Id, Name, Description, Unit, CostPerUnit, SupplierId, MinStock, CurrentStock, ExpirationDays, IsActive, StateCode, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
                VALUES (@Id, @Name, @Description, @Unit, @CostPerUnit, @SupplierId, @MinStock, @CurrentStock, @ExpirationDays, @IsActive, @StateCode, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy)";
            
            await connection.ExecuteAsync(sql, ingredient);
            return ingredient.Id;
        }

        public async Task<bool> UpdateAsync(Ingredient ingredient)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Ingredient SET 
                Name = @Name, Description = @Description, Unit = @Unit, CostPerUnit = @CostPerUnit, 
                SupplierId = @SupplierId, MinStock = @MinStock, CurrentStock = @CurrentStock, 
                ExpirationDays = @ExpirationDays, IsActive = @IsActive,
                StateCode = @StateCode, LastModifiedAt = @LastModifiedAt, LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, ingredient);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStockAsync(string id, decimal newStock)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Ingredient SET 
                CurrentStock = @NewStock,
                LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, NewStock = newStock, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE Ingredient SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM Ingredient WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM Ingredient WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Ingredient>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Ingredient 
                WHERE StateCode = @StateCode
                ORDER BY Name
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<Ingredient>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }
    }
} 