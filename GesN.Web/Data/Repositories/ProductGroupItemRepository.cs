using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class ProductGroupItemRepository : IProductGroupItemRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductGroupItemRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductGroupItem>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductGroupItem?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.Id = @Id";

            var result = await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { Id = id });

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<ProductGroupItem>> GetByProductGroupIdAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupItem>> GetByProductIdAsync(string productId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*, pg.Name as ProductGroupName
                FROM ProductGroupItem pgi
                LEFT JOIN Product pg ON pgi.ProductGroupId = pg.Id
                WHERE pgi.ProductId = @ProductId 
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt";

            return await connection.QueryAsync<ProductGroupItem>(sql, 
                new { ProductId = productId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupItem>> GetOptionalItemsAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.IsOptional = 1
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupItem>> GetRequiredItemsAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.IsOptional = 0
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductGroupItem?> GetByProductGroupAndProductAsync(string productGroupId, string productId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.ProductId = @ProductId";

            var result = await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { ProductGroupId = productGroupId, ProductId = productId });

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<ProductGroupItem>> GetByQuantityRangeAsync(string productGroupId, decimal minQuantity, decimal maxQuantity)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.Quantity >= @MinQuantity
                  AND pgi.Quantity <= @MaxQuantity
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.Quantity";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { 
                    ProductGroupId = productGroupId, 
                    MinQuantity = minQuantity, 
                    MaxQuantity = maxQuantity,
                    StateCode = (int)ObjectState.Active 
                });
        }

        public async Task<IEnumerable<ProductGroupItem>> GetWithExtraPriceAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.ExtraPrice > 0
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.ExtraPrice DESC";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<string> CreateAsync(ProductGroupItem groupItem)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductGroupItem (
                    Id, ProductGroupId, ProductId, ProductCategoryId, Quantity, MinQuantity, MaxQuantity,
                    DefaultQuantity, IsOptional, ExtraPrice, StateCode,
                    CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @ProductGroupId, @ProductId, @ProductCategoryId, @Quantity, @MinQuantity, @MaxQuantity,
                    @DefaultQuantity, @IsOptional, @ExtraPrice, @StateCode,
                    @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
                )";
            
            await connection.ExecuteAsync(sql, groupItem);
            return groupItem.Id;
        }

        public async Task<bool> UpdateAsync(ProductGroupItem groupItem)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupItem SET 
                    ProductGroupId = @ProductGroupId,
                    ProductId = @ProductId,
                    ProductCategoryId = @ProductCategoryId,
                    Quantity = @Quantity,
                    MinQuantity = @MinQuantity,
                    MaxQuantity = @MaxQuantity,
                    DefaultQuantity = @DefaultQuantity,
                    IsOptional = @IsOptional,
                    ExtraPrice = @ExtraPrice,
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, groupItem);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE ProductGroupItem SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM ProductGroupItem WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM ProductGroupItem WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<int> CountByProductGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(*) FROM ProductGroupItem 
                WHERE ProductGroupId = @ProductGroupId AND StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<bool> UpdateQuantityAsync(string id, decimal quantity)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupItem SET 
                    Quantity = @Quantity,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, Quantity = quantity, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<bool> SetOptionalAsync(string id, bool isOptional)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupItem SET 
                    IsOptional = @IsOptional,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, IsOptional = isOptional, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<decimal> GetTotalExtraPriceAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COALESCE(SUM(ExtraPrice), 0) FROM ProductGroupItem 
                WHERE ProductGroupId = @ProductGroupId AND StateCode = @StateCode";
            
            return await connection.QuerySingleAsync<decimal>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<bool> ValidateQuantityLimitsAsync(string id, decimal quantity)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) FROM ProductGroupItem 
                WHERE Id = @Id 
                  AND @Quantity >= MinQuantity 
                  AND @Quantity <= MaxQuantity";
            
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id, Quantity = quantity });
            return count > 0;
        }

        public async Task<IEnumerable<ProductGroupItem>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*, p.Name as ProductName
                FROM ProductGroupItem pgi
                LEFT JOIN Product p ON pgi.ProductId = p.Id
                WHERE pgi.StateCode = @StateCode
                  AND p.Name LIKE @SearchTerm
                ORDER BY p.Name";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<ProductGroupItem>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<IEnumerable<ProductGroupItem>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*, p.Name as ProductName
                FROM ProductGroupItem pgi
                LEFT JOIN Product p ON pgi.ProductId = p.Id
                WHERE pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductGroupItem>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }

        public async Task<bool> BulkDeleteByProductGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupItem SET 
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

        // Métodos faltantes da interface
        public async Task<IEnumerable<ProductGroupItem>> GetAvailableByGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                LEFT JOIN Product p ON pgi.ProductId = p.Id
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.StateCode = @StateCode
                  AND p.StateCode = @ActiveState
                ORDER BY p.Name";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { 
                    ProductGroupId = productGroupId, 
                    StateCode = (int)ObjectState.Active,
                    ActiveState = (int)ObjectState.Active
                });
        }

        public async Task<IEnumerable<ProductGroupItem>> GetOptionalByGroupAsync(string productGroupId)
        {
            return await GetOptionalItemsAsync(productGroupId);
        }

        public async Task<IEnumerable<ProductGroupItem>> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var offset = (page - 1) * pageSize;
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { 
                    ProductGroupId = productGroupId, 
                    StateCode = (int)ObjectState.Active,
                    PageSize = pageSize,
                    Offset = offset
                });
        }

        public async Task<bool> ItemExistsInGroupAsync(string productGroupId, string productId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM ProductGroupItem 
                WHERE ProductGroupId = @ProductGroupId 
                  AND ProductId = @ProductId
                  AND StateCode = @StateCode";

            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                ProductGroupId = productGroupId,
                ProductId = productId,
                StateCode = (int)ObjectState.Active
            });

            return count > 0;
        }

        public async Task<decimal> CalculateGroupTotalPriceAsync(string productGroupId, IEnumerable<string> selectedItemIds)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Primeiro buscar os itens
            const string itemsSql = @"
                SELECT pgi.ProductId, pgi.ExtraPrice
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.Id IN @SelectedItemIds
                  AND pgi.StateCode = @StateCode";

            var items = await connection.QueryAsync<(string ProductId, decimal ExtraPrice)>(itemsSql, new
            {
                ProductGroupId = productGroupId,
                SelectedItemIds = selectedItemIds.ToList(),
                StateCode = (int)ObjectState.Active
            });

            decimal totalPrice = 0;
            foreach (var item in items)
            {
                // Buscar preço do produto separadamente
                const string productPriceSql = "SELECT UnitPrice FROM Product WHERE Id = @ProductId";
                var unitPrice = await connection.QuerySingleOrDefaultAsync<decimal?>(productPriceSql, new { ProductId = item.ProductId });
                
                totalPrice += (unitPrice ?? 0) + item.ExtraPrice;
            }

            return totalPrice;
        }

        // Métodos específicos para ProductCategory
        public async Task<IEnumerable<ProductGroupItem>> GetByProductCategoryIdAsync(string productCategoryId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*, pg.Name as ProductGroupName
                FROM ProductGroupItem pgi
                LEFT JOIN Product pg ON pgi.ProductGroupId = pg.Id
                WHERE pgi.ProductCategoryId = @ProductCategoryId 
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt";

            return await connection.QueryAsync<ProductGroupItem>(sql, 
                new { ProductCategoryId = productCategoryId, StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductGroupItem?> GetByProductGroupAndCategoryAsync(string productGroupId, string productCategoryId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.ProductCategoryId = @ProductCategoryId";

            var result = await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { ProductGroupId = productGroupId, ProductCategoryId = productCategoryId });

            return result.FirstOrDefault();
        }

        public async Task<bool> CategoryItemExistsInGroupAsync(string productGroupId, string productCategoryId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM ProductGroupItem 
                WHERE ProductGroupId = @ProductGroupId 
                  AND ProductCategoryId = @ProductCategoryId
                  AND StateCode = @StateCode";

            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                ProductGroupId = productGroupId,
                ProductCategoryId = productCategoryId,
                StateCode = (int)ObjectState.Active
            });

            return count > 0;
        }

        public async Task<IEnumerable<ProductGroupItem>> GetCategoryItemsByGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.ProductCategoryId IS NOT NULL
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupItem>> GetProductItemsByGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pgi.*
                FROM ProductGroupItem pgi
                WHERE pgi.ProductGroupId = @ProductGroupId 
                  AND pgi.ProductId IS NOT NULL
                  AND pgi.StateCode = @StateCode
                ORDER BY pgi.CreatedAt";

            return await connection.QueryAsync<ProductGroupItem>(
                sql,
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }
    }
} 