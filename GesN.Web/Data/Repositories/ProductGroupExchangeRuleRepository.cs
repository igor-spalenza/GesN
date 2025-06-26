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

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByOriginalProductIdAsync(string originalProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       ep.* as ExchangeProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.OriginalProductId = @OriginalProductId 
                  AND pger.StateCode = @StateCode
                  AND pger.IsActive = 1
                ORDER BY ep.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, ProductGroupExchangeRule>(
                sql,
                (rule, exchangeProduct) =>
                {
                    rule.ExchangeProduct = exchangeProduct;
                    return rule;
                },
                new { OriginalProductId = originalProductId, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByExchangeProductIdAsync(string exchangeProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       op.* as OriginalProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                WHERE pger.ExchangeProductId = @ExchangeProductId 
                  AND pger.StateCode = @StateCode
                  AND pger.IsActive = 1
                ORDER BY op.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, ProductGroupExchangeRule>(
                sql,
                (rule, originalProduct) =>
                {
                    rule.OriginalProduct = originalProduct;
                    return rule;
                },
                new { ExchangeProductId = exchangeProductId, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetActiveRulesAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       op.* as OriginalProduct,
                       ep.* as ExchangeProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.IsActive = 1
                  AND pger.StateCode = @StateCode
                ORDER BY op.Name, ep.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, Product, ProductGroupExchangeRule>(
                sql,
                (rule, originalProduct, exchangeProduct) =>
                {
                    rule.OriginalProduct = originalProduct;
                    rule.ExchangeProduct = exchangeProduct;
                    return rule;
                },
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active },
                splitOn: "Id,Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetInactiveRulesAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       op.* as OriginalProduct,
                       ep.* as ExchangeProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.IsActive = 0
                  AND pger.StateCode = @StateCode
                ORDER BY op.Name, ep.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, Product, ProductGroupExchangeRule>(
                sql,
                (rule, originalProduct, exchangeProduct) =>
                {
                    rule.OriginalProduct = originalProduct;
                    rule.ExchangeProduct = exchangeProduct;
                    return rule;
                },
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active },
                splitOn: "Id,Id");

            return result;
        }

        public async Task<ProductGroupExchangeRule?> GetExchangeRuleAsync(string productGroupId, string originalProductId, string exchangeProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       op.* as OriginalProduct,
                       ep.* as ExchangeProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.OriginalProductId = @OriginalProductId
                  AND pger.ExchangeProductId = @ExchangeProductId
                  AND pger.StateCode = @StateCode";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, Product, ProductGroupExchangeRule>(
                sql,
                (rule, originalProduct, exchangeProduct) =>
                {
                    rule.OriginalProduct = originalProduct;
                    rule.ExchangeProduct = exchangeProduct;
                    return rule;
                },
                new { 
                    ProductGroupId = productGroupId, 
                    OriginalProductId = originalProductId, 
                    ExchangeProductId = exchangeProductId,
                    StateCode = (int)ObjectState.Active 
                },
                splitOn: "Id,Id");

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByExchangeRatioRangeAsync(string productGroupId, decimal minRatio, decimal maxRatio)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       op.* as OriginalProduct,
                       ep.* as ExchangeProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.ExchangeRatio >= @MinRatio
                  AND pger.ExchangeRatio <= @MaxRatio
                  AND pger.StateCode = @StateCode
                ORDER BY pger.ExchangeRatio";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, Product, ProductGroupExchangeRule>(
                sql,
                (rule, originalProduct, exchangeProduct) =>
                {
                    rule.OriginalProduct = originalProduct;
                    rule.ExchangeProduct = exchangeProduct;
                    return rule;
                },
                new { 
                    ProductGroupId = productGroupId, 
                    MinRatio = minRatio, 
                    MaxRatio = maxRatio,
                    StateCode = (int)ObjectState.Active 
                },
                splitOn: "Id,Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetWithAdditionalCostAsync(string productGroupId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       op.* as OriginalProduct,
                       ep.* as ExchangeProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.AdditionalCost > 0
                  AND pger.StateCode = @StateCode
                ORDER BY pger.AdditionalCost DESC";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, Product, ProductGroupExchangeRule>(
                sql,
                (rule, originalProduct, exchangeProduct) =>
                {
                    rule.OriginalProduct = originalProduct;
                    rule.ExchangeProduct = exchangeProduct;
                    return rule;
                },
                new { ProductGroupId = productGroupId, StateCode = (int)ObjectState.Active },
                splitOn: "Id,Id");

            return result;
        }

        public async Task<string> CreateAsync(ProductGroupExchangeRule rule)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductGroupExchangeRule (
                    Id, ProductGroupId, OriginalProductId, ExchangeProductId, 
                    ExchangeRatio, AdditionalCost, IsActive, StateCode,
                    CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @ProductGroupId, @OriginalProductId, @ExchangeProductId, 
                    @ExchangeRatio, @AdditionalCost, @IsActive, @StateCode,
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
                    OriginalProductId = @OriginalProductId,
                    ExchangeProductId = @ExchangeProductId,
                    ExchangeRatio = @ExchangeRatio,
                    AdditionalCost = @AdditionalCost,
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

        public async Task<bool> UpdateAdditionalCostAsync(string id, decimal additionalCost)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductGroupExchangeRule SET 
                    AdditionalCost = @AdditionalCost,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, AdditionalCost = additionalCost, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<decimal> CalculateExchangeCostAsync(string ruleId, decimal quantity)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT (ExchangeRatio * @Quantity) + AdditionalCost 
                FROM ProductGroupExchangeRule 
                WHERE Id = @RuleId";
            
            return await connection.QuerySingleOrDefaultAsync<decimal>(sql, 
                new { RuleId = ruleId, Quantity = quantity });
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

        public async Task<bool> ValidateUniqueExchangeAsync(string productGroupId, string originalProductId, string exchangeProductId, string? excludeId = null)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            string sql = @"
                SELECT COUNT(1) FROM ProductGroupExchangeRule 
                WHERE ProductGroupId = @ProductGroupId 
                  AND OriginalProductId = @OriginalProductId 
                  AND ExchangeProductId = @ExchangeProductId
                  AND StateCode = @StateCode";

            object parameters = new { 
                ProductGroupId = productGroupId, 
                OriginalProductId = originalProductId, 
                ExchangeProductId = exchangeProductId,
                StateCode = (int)ObjectState.Active 
            };

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                sql += " AND Id != @ExcludeId";
                parameters = new { 
                    ProductGroupId = productGroupId, 
                    OriginalProductId = originalProductId, 
                    ExchangeProductId = exchangeProductId,
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
                       op.* as OriginalProduct,
                       ep.* as ExchangeProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.StateCode = @StateCode
                ORDER BY pger.CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, Product, ProductGroupExchangeRule>(
                sql,
                (rule, originalProduct, exchangeProduct) =>
                {
                    rule.OriginalProduct = originalProduct;
                    rule.ExchangeProduct = exchangeProduct;
                    return rule;
                },
                new { 
                    ProductGroupId = productGroupId, 
                    StateCode = (int)ObjectState.Active,
                    PageSize = pageSize,
                    Offset = offset
                },
                splitOn: "Id,Id");

            return result;
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetAvailableExchangesAsync(string productGroupId, string originalProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pger.*, 
                       op.* as OriginalProduct,
                       ep.* as ExchangeProduct
                FROM ProductGroupExchangeRule pger
                LEFT JOIN Product op ON pger.OriginalProductId = op.Id
                LEFT JOIN Product ep ON pger.ExchangeProductId = ep.Id
                WHERE pger.ProductGroupId = @ProductGroupId 
                  AND pger.OriginalProductId = @OriginalProductId
                  AND pger.IsActive = 1
                  AND pger.StateCode = @StateCode
                ORDER BY ep.Name";

            var result = await connection.QueryAsync<ProductGroupExchangeRule, Product, Product, ProductGroupExchangeRule>(
                sql,
                (rule, originalProduct, exchangeProduct) =>
                {
                    rule.OriginalProduct = originalProduct;
                    rule.ExchangeProduct = exchangeProduct;
                    return rule;
                },
                new { 
                    ProductGroupId = productGroupId, 
                    OriginalProductId = originalProductId,
                    StateCode = (int)ObjectState.Active 
                },
                splitOn: "Id,Id");

            return result;
        }

        public async Task<bool> ExchangeRuleExistsAsync(string productGroupId, string originalProductId, string exchangeProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) 
                FROM ProductGroupExchangeRule 
                WHERE ProductGroupId = @ProductGroupId 
                  AND OriginalProductId = @OriginalProductId
                  AND ExchangeProductId = @ExchangeProductId
                  AND StateCode = @StateCode";

            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                ProductGroupId = productGroupId,
                OriginalProductId = originalProductId,
                ExchangeProductId = exchangeProductId,
                StateCode = (int)ObjectState.Active
            });

            return count > 0;
        }
    }
} 