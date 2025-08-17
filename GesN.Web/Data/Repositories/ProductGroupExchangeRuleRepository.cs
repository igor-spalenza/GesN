using System.Data;
using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class ProductGroupExchangeRuleRepository : IProductGroupExchangeRuleRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductGroupExchangeRuleRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupExchangeRule 
                WHERE StateCode = @StateCode
                ORDER BY CreatedAt";

            return await connection.QueryAsync<ProductGroupExchangeRule>(sql, 
                new { StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductGroupExchangeRule?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = "SELECT * FROM ProductGroupExchangeRule WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<ProductGroupExchangeRule>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupIdAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductGroupExchangeRule 
                WHERE ProductGroupId = @ProductGroupId 
                  AND StateCode = @StateCode
                ORDER BY CreatedAt";

            return await connection.QueryAsync<ProductGroupExchangeRule>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetBySourceGroupItemIdAsync(string sourceGroupItemId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       sgi.* as SourceGroupItem,
                       sp.* as SourceProduct,
                       tgi.* as TargetGroupItem,
                       tp.* as TargetProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN ProductGroupItem sgi ON pger.SourceGroupItemId = sgi.Id
                LEFT JOIN Product sp ON sgi.ProductId = sp.Id
                LEFT JOIN ProductGroupItem tgi ON pger.TargetGroupItemId = tgi.Id
                LEFT JOIN Product tp ON tgi.ProductId = tp.Id
                WHERE pger.SourceGroupItemId = @SourceGroupItemId 
                  AND pger.StateCode = @StateCode
                  AND pger.IsActive = 1
                ORDER BY sp.Name, tp.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, ProductGroupItem, Product, ProductGroupItem, Product, ProductGroupExchangeRule>(
                sql,
                (rule, sourceGroupItem, sourceProduct, targetGroupItem, targetProduct) =>
                {
                    if (sourceGroupItem != null)
                    {
                        sourceGroupItem.Product = sourceProduct;
                        rule.SourceGroupItem = sourceGroupItem;
                    }
                    
                    if (targetGroupItem != null)
                    {
                        targetGroupItem.Product = targetProduct;
                        rule.TargetGroupItem = targetGroupItem;
                    }
                    
                    return rule;
                },
                new { SourceGroupItemId = sourceGroupItemId, StateCode = (int)ObjectState.Active },
                splitOn: "Id,Id,Id,Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByTargetGroupItemIdAsync(string targetGroupItemId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       sgi.* as SourceGroupItem,
                       sp.* as SourceProduct,
                       tgi.* as TargetGroupItem,
                       tp.* as TargetProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN ProductGroupItem sgi ON pger.SourceGroupItemId = sgi.Id
                LEFT JOIN Product sp ON sgi.ProductId = sp.Id
                LEFT JOIN ProductGroupItem tgi ON pger.TargetGroupItemId = tgi.Id
                LEFT JOIN Product tp ON tgi.ProductId = tp.Id
                WHERE pger.TargetGroupItemId = @TargetGroupItemId 
                  AND pger.StateCode = @StateCode
                  AND pger.IsActive = 1
                ORDER BY sp.Name, tp.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, ProductGroupItem, Product, ProductGroupItem, Product, ProductGroupExchangeRule>(
                sql,
                (rule, sourceGroupItem, sourceProduct, targetGroupItem, targetProduct) =>
                {
                    if (sourceGroupItem != null)
                    {
                        sourceGroupItem.Product = sourceProduct;
                        rule.SourceGroupItem = sourceGroupItem;
                    }
                    
                    if (targetGroupItem != null)
                    {
                        targetGroupItem.Product = targetProduct;
                        rule.TargetGroupItem = targetGroupItem;
                    }
                    
                    return rule;
                },
                new { TargetGroupItemId = targetGroupItemId, StateCode = (int)ObjectState.Active },
                splitOn: "Id,Id,Id,Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetActiveRulesAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       sgi.* as SourceGroupItem,
                       sp.* as SourceProduct,
                       tgi.* as TargetGroupItem,
                       tp.* as TargetProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN ProductGroupItem sgi ON pger.SourceGroupItemId = sgi.Id
                LEFT JOIN Product sp ON sgi.ProductId = sp.Id
                LEFT JOIN ProductGroupItem tgi ON pger.TargetGroupItemId = tgi.Id
                LEFT JOIN Product tp ON tgi.ProductId = tp.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.IsActive = 1
                  AND pger.StateCode = @StateCode
                ORDER BY sp.Name, tp.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, ProductGroupItem, Product, ProductGroupItem, Product, ProductGroupExchangeRule>(
                sql,
                (rule, sourceGroupItem, sourceProduct, targetGroupItem, targetProduct) =>
                {
                    if (sourceGroupItem != null)
                    {
                        sourceGroupItem.Product = sourceProduct;
                        rule.SourceGroupItem = sourceGroupItem;
                    }
                    
                    if (targetGroupItem != null)
                    {
                        targetGroupItem.Product = targetProduct;
                        rule.TargetGroupItem = targetGroupItem;
                    }
                    
                    return rule;
                },
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active },
                splitOn: "Id,Id,Id,Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetInactiveRulesAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       sgi.* as SourceGroupItem,
                       sp.* as SourceProduct,
                       tgi.* as TargetGroupItem,
                       tp.* as TargetProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN ProductGroupItem sgi ON pger.SourceGroupItemId = sgi.Id
                LEFT JOIN Product sp ON sgi.ProductId = sp.Id
                LEFT JOIN ProductGroupItem tgi ON pger.TargetGroupItemId = tgi.Id
                LEFT JOIN Product tp ON tgi.ProductId = tp.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.IsActive = 0
                  AND pger.StateCode = @StateCode
                ORDER BY sp.Name, tp.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, ProductGroupItem, Product, ProductGroupItem, Product, ProductGroupExchangeRule>(
                sql,
                (rule, sourceGroupItem, sourceProduct, targetGroupItem, targetProduct) =>
                {
                    if (sourceGroupItem != null)
                    {
                        sourceGroupItem.Product = sourceProduct;
                        rule.SourceGroupItem = sourceGroupItem;
                    }
                    
                    if (targetGroupItem != null)
                    {
                        targetGroupItem.Product = targetProduct;
                        rule.TargetGroupItem = targetGroupItem;
                    }
                    
                    return rule;
                },
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active },
                splitOn: "Id,Id,Id,Id");

            return result;
        }

        public async Task<ProductGroupExchangeRule?> GetExchangeRuleAsync(string productGroupId, string sourceGroupItemId, string targetGroupItemId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       sgi.* as SourceGroupItem,
                       sp.* as SourceProduct,
                       tgi.* as TargetGroupItem,
                       tp.* as TargetProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN ProductGroupItem sgi ON pger.SourceGroupItemId = sgi.Id
                LEFT JOIN Product sp ON sgi.ProductId = sp.Id
                LEFT JOIN ProductGroupItem tgi ON pger.TargetGroupItemId = tgi.Id
                LEFT JOIN Product tp ON tgi.ProductId = tp.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.SourceGroupItemId = @SourceGroupItemId
                  AND pger.TargetGroupItemId = @TargetGroupItemId
                  AND pger.StateCode = @StateCode";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, ProductGroupItem, Product, ProductGroupItem, Product, ProductGroupExchangeRule>(
                sql,
                (rule, sourceGroupItem, sourceProduct, targetGroupItem, targetProduct) =>
                {
                    if (sourceGroupItem != null)
                    {
                        sourceGroupItem.Product = sourceProduct;
                        rule.SourceGroupItem = sourceGroupItem;
                    }
                    
                    if (targetGroupItem != null)
                    {
                        targetGroupItem.Product = targetProduct;
                        rule.TargetGroupItem = targetGroupItem;
                    }
                    
                    return rule;
                },
                new { 
                    ProductGroupId = productGroupId, 
                    SourceGroupItemId = sourceGroupItemId, 
                    TargetGroupItemId = targetGroupItemId,
                    StateCode = (int)ObjectState.Active 
                },
                splitOn: "Id,Id,Id,Id");

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByExchangeRatioRangeAsync(string productGroupId, decimal minRatio, decimal maxRatio)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       sgi.* as SourceGroupItem,
                       sp.* as SourceProduct,
                       tgi.* as TargetGroupItem,
                       tp.* as TargetProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN ProductGroupItem sgi ON pger.SourceGroupItemId = sgi.Id
                LEFT JOIN Product sp ON sgi.ProductId = sp.Id
                LEFT JOIN ProductGroupItem tgi ON pger.TargetGroupItemId = tgi.Id
                LEFT JOIN Product tp ON tgi.ProductId = tp.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.ExchangeRatio >= @MinRatio
                  AND pger.ExchangeRatio <= @MaxRatio
                  AND pger.StateCode = @StateCode
                ORDER BY pger.ExchangeRatio";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, ProductGroupItem, Product, ProductGroupItem, Product, ProductGroupExchangeRule>(
                sql,
                (rule, sourceGroupItem, sourceProduct, targetGroupItem, targetProduct) =>
                {
                    if (sourceGroupItem != null)
                    {
                        sourceGroupItem.Product = sourceProduct;
                        rule.SourceGroupItem = sourceGroupItem;
                    }
                    
                    if (targetGroupItem != null)
                    {
                        targetGroupItem.Product = targetProduct;
                        rule.TargetGroupItem = targetGroupItem;
                    }
                    
                    return rule;
                },
                new { 
                    ProductGroupId = productGroupId, 
                    MinRatio = minRatio, 
                    MaxRatio = maxRatio,
                    StateCode = (int)ObjectState.Active 
                },
                splitOn: "Id,Id,Id,Id");

            return result;
        }



        public async Task<string> CreateAsync(ProductGroupExchangeRule rule)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductGroupExchangeRule (
                    Id, ProductGroupId, SourceGroupItemId, SourceGroupItemWeight, 
                    TargetGroupItemId, TargetGroupItemWeight, ExchangeRatio, IsActive, StateCode,
                    CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @ProductGroupId, @SourceGroupItemId, @SourceGroupItemWeight, 
                    @TargetGroupItemId, @TargetGroupItemWeight, @ExchangeRatio, @IsActive, @StateCode,
                    @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
                )";
            
            await connection.ExecuteAsync(sql, rule);
            return rule.Id;
        }

        public async Task<bool> UpdateAsync(ProductGroupExchangeRule rule)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupExchangeRule SET 
                    ProductGroupId = @ProductGroupId,
                    SourceGroupItemId = @SourceGroupItemId,
                    SourceGroupItemWeight = @SourceGroupItemWeight,
                    TargetGroupItemId = @TargetGroupItemId,
                    TargetGroupItemWeight = @TargetGroupItemWeight,
                    ExchangeRatio = @ExchangeRatio,
                    IsActive = @IsActive,
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, rule);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE ProductGroupExchangeRule SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM ProductGroupExchangeRule WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM ProductGroupExchangeRule WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<int> CountByProductGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(*) FROM ProductGroupExchangeRule 
                WHERE ProductGroupId = @ProductGroupId AND StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<int> CountActiveRulesAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(*) FROM ProductGroupExchangeRule 
                WHERE ProductGroupId = @ProductGroupId 
                  AND IsActive = 1 
                  AND StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, 
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active });
        }

        public async Task<bool> SetActiveStatusAsync(string id, bool isActive)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupExchangeRule SET 
                    IsActive = @IsActive,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, IsActive = isActive, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateExchangeRatioAsync(string id, decimal exchangeRatio)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupExchangeRule SET 
                    ExchangeRatio = @ExchangeRatio,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, ExchangeRatio = exchangeRatio, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }



        public async Task<IEnumerable<ProductGroupExchangeRule>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, op.Name as OriginalProductName, ep.Name as ExchangeProductName
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.StateCode = @StateCode
                  AND (op.Name LIKE @SearchTerm OR ep.Name LIKE @SearchTerm)
                ORDER BY op.Name, ep.Name";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<ProductGroupExchangeRule>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, op.Name as OriginalProductName, ep.Name as ExchangeProductName
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.StateCode = @StateCode
                ORDER BY pger.CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductGroupExchangeRule>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }

        public async Task<bool> BulkDeleteByProductGroupAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupExchangeRule SET 
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

        public async Task<bool> ValidateUniqueExchangeAsync(string productGroupId, string sourceGroupItemId, string targetGroupItemId, string? excludeId = null)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            string sql = @"
                SELECT COUNT(1) FROM ProductGroupExchangeRule 
                WHERE ProductGroupId = @ProductGroupId 
                  AND SourceGroupItemId = @SourceGroupItemId 
                  AND TargetGroupItemId = @TargetGroupItemId
                  AND StateCode = @StateCode";

            object parameters = new { 
                ProductGroupId = productGroupId, 
                SourceGroupItemId = sourceGroupItemId, 
                TargetGroupItemId = targetGroupItemId,
                StateCode = (int)ObjectState.Active 
            };

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                sql += " AND Id != @ExcludeId";
                parameters = new { 
                    ProductGroupId = productGroupId, 
                    SourceGroupItemId = sourceGroupItemId, 
                    TargetGroupItemId = targetGroupItemId,
                    StateCode = (int)ObjectState.Active,
                    ExcludeId = excludeId 
                };
            }

            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count == 0;
        }

        public async Task<bool> BulkActivateRulesAsync(List<string> ruleIds)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupExchangeRule SET 
                    IsActive = 1,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";

            foreach (var ruleId in ruleIds)
            {
                await connection.ExecuteAsync(sql, 
                    new { Id = ruleId, LastModifiedAt = DateTime.UtcNow });
            }

            return true;
        }

        public async Task<bool> BulkDeactivateRulesAsync(List<string> ruleIds)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupExchangeRule SET 
                    IsActive = 0,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";

            foreach (var ruleId in ruleIds)
            {
                await connection.ExecuteAsync(sql, 
                    new { Id = ruleId, LastModifiedAt = DateTime.UtcNow });
            }

            return true;
        }

        // Métodos faltantes da interface
        public async Task<IEnumerable<ProductGroupExchangeRule>> GetActiveByGroupAsync(string productGroupId)
        {
            return await GetActiveRulesAsync(productGroupId);
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var offset = (page - 1) * pageSize;
            
            const string sql = @"
                SELECT pger.*, 
                       sgi.* as SourceGroupItem,
                       sp.* as SourceProduct,
                       tgi.* as TargetGroupItem,
                       tp.* as TargetProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN ProductGroupItem sgi ON pger.SourceGroupItemId = sgi.Id
                LEFT JOIN Product sp ON sgi.ProductId = sp.Id
                LEFT JOIN ProductGroupItem tgi ON pger.TargetGroupItemId = tgi.Id
                LEFT JOIN Product tp ON tgi.ProductId = tp.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.StateCode = @StateCode
                ORDER BY pger.CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, ProductGroupItem, Product, ProductGroupItem, Product, ProductGroupExchangeRule>(
                sql,
                (rule, sourceGroupItem, sourceProduct, targetGroupItem, targetProduct) =>
                {
                    if (sourceGroupItem != null)
                    {
                        sourceGroupItem.Product = sourceProduct;
                        rule.SourceGroupItem = sourceGroupItem;
                    }
                    
                    if (targetGroupItem != null)
                    {
                        targetGroupItem.Product = targetProduct;
                        rule.TargetGroupItem = targetGroupItem;
                    }
                    
                    return rule;
                },
                new { 
                    ProductGroupId = productGroupId, 
                    StateCode = (int)ObjectState.Active,
                    PageSize = pageSize,
                    Offset = offset
                },
                splitOn: "Id,Id,Id,Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetAvailableExchangesAsync(string productGroupId, string sourceGroupItemId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       sgi.Id, sgi.ProductCategoryId, sgi.ProductId, sgi.MinQuantity, sgi.MaxQuantity, sgi.RequiredQuantity, sgi.UnitPrice, sgi.IsRequired, sgi.IsActive,
                       tgi.Id, tgi.ProductCategoryId, tgi.ProductId, tgi.MinQuantity, tgi.MaxQuantity, tgi.RequiredQuantity, tgi.UnitPrice, tgi.IsRequired, tgi.IsActive
                FROM ProductGroupExchangeRule pger
                LEFT JOIN ProductGroupItem sgi ON pger.SourceGroupItemId = sgi.Id
                LEFT JOIN ProductGroupItem tgi ON pger.TargetGroupItemId = tgi.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.SourceGroupItemId = @SourceGroupItemId
                  AND pger.IsActive = 1
                  AND pger.StateCode = @StateCode
                ORDER BY pger.CreatedAt";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, ProductGroupItem, ProductGroupItem, ProductGroupExchangeRule>(
                sql,
                (rule, sourceGroupItem, targetGroupItem) =>
                {
                    rule.SourceGroupItem = sourceGroupItem;
                    rule.TargetGroupItem = targetGroupItem;
                    return rule;
                },
                new { 
                    ProductGroupId = productGroupId, 
                    SourceGroupItemId = sourceGroupItemId,
                    StateCode = (int)ObjectState.Active 
                },
                splitOn: "Id,Id");

            return result;
        }

        public async Task<bool> ExchangeRuleExistsAsync(string productGroupId, string sourceGroupItemId, string targetGroupItemId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM ProductGroupExchangeRule 
                WHERE ProductGroupId = @ProductGroupId 
                  AND SourceGroupItemId = @SourceGroupItemId
                  AND TargetGroupItemId = @TargetGroupItemId
                  AND StateCode = @StateCode";

            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                ProductGroupId = productGroupId,
                SourceGroupItemId = sourceGroupItemId,
                TargetGroupItemId = targetGroupItemId,
                StateCode = (int)ObjectState.Active
            });

            return count > 0;
        }

        // New methods to match the updated interface
        public async Task<bool> ValidateGroupItemsCompatibilityAsync(string sourceGroupItemId, string targetGroupItemId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM ProductGroupItem sgi
                JOIN ProductGroupItem tgi ON sgi.ProductGroupId = tgi.ProductGroupId
                WHERE sgi.Id = @SourceGroupItemId 
                  AND tgi.Id = @TargetGroupItemId
                  AND sgi.StateCode = @StateCode 
                  AND tgi.StateCode = @StateCode";

            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                SourceGroupItemId = sourceGroupItemId,
                TargetGroupItemId = targetGroupItemId,
                StateCode = (int)ObjectState.Active
            });

            return count > 0;
        }
    }
} 