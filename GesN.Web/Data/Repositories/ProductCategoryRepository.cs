using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductCategoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductCategory 
                WHERE StateCode = @StateCode
                ORDER BY Name";

            return await connection.QueryAsync<ProductCategory>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductCategory?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductCategory 
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<ProductCategory>(sql, new { Id = id });
        }

        public async Task<ProductCategory?> GetByNameAsync(string name)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductCategory 
                WHERE Name = @Name AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<ProductCategory>(sql, 
                new { Name = name, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductCategory>> GetActiveAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductCategory 
                WHERE StateCode = @StateCode AND IsActive = 1
                ORDER BY Name";

            return await connection.QueryAsync<ProductCategory>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductCategory>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductCategory 
                WHERE StateCode = @StateCode
                  AND (Name LIKE @SearchTerm
                       OR Description LIKE @SearchTerm)
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<ProductCategory>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<IEnumerable<ProductCategory>> SearchForAutocompleteAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductCategory 
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
            
            return await connection.QueryAsync<ProductCategory>(sql, 
                new { 
                    StateCode = (int)ObjectState.Active, 
                    SearchTerm = searchPattern,
                    ExactStart = exactStartPattern
                });
        }

        public async Task<string> CreateAsync(ProductCategory category)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductCategory (Id, Name, Description, IsActive, StateCode, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
                VALUES (@Id, @Name, @Description, @IsActive, @StateCode, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy)";
            
            await connection.ExecuteAsync(sql, category);
            return category.Id;
        }

        public async Task<bool> UpdateAsync(ProductCategory category)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductCategory SET 
                Name = @Name, Description = @Description, IsActive = @IsActive,
                StateCode = @StateCode, LastModifiedAt = @LastModifiedAt, LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, category);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE ProductCategory SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM ProductCategory WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM ProductCategory WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductCategory>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductCategory 
                WHERE StateCode = @StateCode
                ORDER BY Name
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductCategory>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }
    }
} 