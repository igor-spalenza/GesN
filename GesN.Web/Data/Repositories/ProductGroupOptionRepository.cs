using System.Data;
using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class ProductGroupOptionRepository : IProductGroupOptionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductGroupOptionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductGroupOption>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE StateCode = @StateCode
                ORDER BY CreatedAt";

            return await connection.QueryAsync<ProductGroupOption>(sql, 
                new { StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductGroupOption?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = "SELECT * FROM ProductGroupOption WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<ProductGroupOption>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ProductGroupOption>> GetByProductGroupIdAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND StateCode = @StateCode
                ORDER BY DisplayOrder, Name";

            return await connection.QueryAsync<ProductGroupOption>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupOption>> GetByOptionTypeAsync(string productGroupId, string optionType)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND OptionType = @OptionType
                  AND StateCode = @StateCode
                ORDER BY DisplayOrder, Name";

            return await connection.QueryAsync<ProductGroupOption>(sql, 
                new { ProductGroupId = productGroupId, OptionType = optionType, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupOption>> GetRequiredOptionsAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND IsRequired = 1
                  AND StateCode = @StateCode
                ORDER BY DisplayOrder, Name";

            return await connection.QueryAsync<ProductGroupOption>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupOption>> GetOptionalOptionsAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND IsRequired = 0
                  AND StateCode = @StateCode
                ORDER BY DisplayOrder, Name";

            return await connection.QueryAsync<ProductGroupOption>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupOption>> GetByDisplayOrderRangeAsync(string productGroupId, int minOrder, int maxOrder)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND DisplayOrder >= @MinOrder
                  AND DisplayOrder <= @MaxOrder
                  AND StateCode = @StateCode
                ORDER BY DisplayOrder, Name";

            return await connection.QueryAsync<ProductGroupOption>(sql, 
                new { 
                    ProductGroupId = productGroupId, 
                    MinOrder = minOrder, 
                    MaxOrder = maxOrder,
                    StateCode = (int)ObjectState.Active 
                });
        }

        public async Task<ProductGroupOption?> GetByNameAsync(string productGroupId, string name)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND Name = @Name
                  AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<ProductGroupOption>(sql, 
                new { ProductGroupId = productGroupId, Name = name, StateCode = (int)ObjectState.Active });
        }

        public async Task<int> GetMaxDisplayOrderAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COALESCE(MAX(DisplayOrder), 0) FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND StateCode = @StateCode";

            return await connection.QuerySingleAsync<int>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<string> CreateAsync(ProductGroupOption option)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductGroupOption (
                    Id, ProductGroupId, Name, Description, OptionType, 
                    IsRequired, DisplayOrder, StateCode,
                    CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @ProductGroupId, @Name, @Description, @OptionType, 
                    @IsRequired, @DisplayOrder, @StateCode,
                    @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
                )";
            
            await connection.ExecuteAsync(sql, option);
            return option.Id;
        }

        public async Task<bool> UpdateAsync(ProductGroupOption option)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupOption SET 
                    ProductGroupId = @ProductGroupId,
                    Name = @Name,
                    Description = @Description,
                    OptionType = @OptionType,
                    IsRequired = @IsRequired,
                    DisplayOrder = @DisplayOrder,
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, option);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE ProductGroupOption SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM ProductGroupOption WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM ProductGroupOption WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<int> CountByProductGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(*) FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId AND StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<int> CountRequiredOptionsAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(*) FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND IsRequired = 1 
                  AND StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<bool> UpdateDisplayOrderAsync(string id, int displayOrder)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupOption SET 
                    DisplayOrder = @DisplayOrder,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, DisplayOrder = displayOrder, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<bool> SetRequiredAsync(string id, bool isRequired)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupOption SET 
                    IsRequired = @IsRequired,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, IsRequired = isRequired, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<bool> ReorderOptionsAsync(string productGroupId, Dictionary<string, int> orderMap)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupOption SET 
                    DisplayOrder = @DisplayOrder,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";

            foreach (var kvp in orderMap)
            {
                await connection.ExecuteAsync(sql, 
                    new { 
                        Id = kvp.Key, 
                        DisplayOrder = kvp.Value, 
                        LastModifiedAt = DateTime.UtcNow 
                    });
            }

            return true;
        }

        public async Task<IEnumerable<ProductGroupOption>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE StateCode = @StateCode
                  AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm)
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<ProductGroupOption>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<IEnumerable<ProductGroupOption>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE StateCode = @StateCode
                ORDER BY CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductGroupOption>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }

        public async Task<bool> BulkDeleteByProductGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupOption SET 
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt
                WHERE ProductGroupId = @ProductGroupId";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { 
                    ProductGroupId = productGroupId, 
                    StateCode = (int)ObjectState.Inactive, 
                    LastModifiedAt = DateTime.UtcNow 
                });
            return rowsAffected > 0;
        }

        public async Task<bool> ValidateUniqueNameAsync(string productGroupId, string name, string? excludeId = null)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            string sql = @"
                SELECT COUNT(1) FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND Name = @Name 
                  AND StateCode = @StateCode";

            object parameters = new { ProductGroupId = productGroupId, Name = name, StateCode = (int)ObjectState.Active };

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                sql += " AND Id != @ExcludeId";
                parameters = new { ProductGroupId = productGroupId, Name = name, StateCode = (int)ObjectState.Active, ExcludeId = excludeId };
            }

            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count == 0;
        }

        public async Task<IEnumerable<string>> GetDistinctOptionTypesAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT DISTINCT OptionType FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND StateCode = @StateCode
                  AND OptionType IS NOT NULL
                ORDER BY OptionType";

            return await connection.QueryAsync<string>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<bool> HasRequiredOptionsAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND IsRequired = 1 
                  AND StateCode = @StateCode";

            var count = await connection.QuerySingleAsync<int>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
            return count > 0;
        }

        // Métodos faltantes da interface
        public async Task<IEnumerable<ProductGroupOption>> GetRequiredByGroupAsync(string productGroupId)
        {
            return await GetRequiredOptionsAsync(productGroupId);
        }

        public async Task<IEnumerable<ProductGroupOption>> GetOptionalByGroupAsync(string productGroupId)
        {
            return await GetOptionalOptionsAsync(productGroupId);
        }

        public async Task<IEnumerable<ProductGroupOption>> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var offset = (page - 1) * pageSize;
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND StateCode = @StateCode
                ORDER BY DisplayOrder, Name
                LIMIT @PageSize OFFSET @Offset";

            return await connection.QueryAsync<ProductGroupOption>(sql, new
            {
                ProductGroupId = productGroupId,
                StateCode = (int)ObjectState.Active,
                PageSize = pageSize,
                Offset = offset
            });
        }

        public async Task<IEnumerable<ProductGroupOption>> GetOrderedByDisplayAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND StateCode = @StateCode
                ORDER BY DisplayOrder, Name";

            return await connection.QueryAsync<ProductGroupOption>(sql, new
            {
                ProductGroupId = productGroupId,
                StateCode = (int)ObjectState.Active
            });
        }

        public async Task<bool> OptionExistsInGroupAsync(string productGroupId, string optionName)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM ProductGroupOption 
                WHERE ProductGroupId = @ProductGroupId 
                  AND Name = @OptionName
                  AND StateCode = @StateCode";

            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                ProductGroupId = productGroupId,
                OptionName = optionName,
                StateCode = (int)ObjectState.Active
            });

            return count > 0;
        }
    }
} 