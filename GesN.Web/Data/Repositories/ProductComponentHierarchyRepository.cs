using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    /// <summary>
    /// Implementação do repositório para a entidade ProductComponentHierarchy usando Dapper
    /// </summary>
    public class ProductComponentHierarchyRepository : IProductComponentHierarchyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductComponentHierarchyRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Operações CRUD básicas

        public async Task<IEnumerable<ProductComponentHierarchy>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductComponentHierarchy 
                WHERE StateCode = @StateCode
                ORDER BY Name";

            return await connection.QueryAsync<ProductComponentHierarchy>(sql, 
                new { StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductComponentHierarchy?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductComponentHierarchy 
                WHERE Id = @Id AND StateCode = @StateCode";

            return await connection.QueryFirstOrDefaultAsync<ProductComponentHierarchy>(sql, 
                new { Id = id, StateCode = (int)ObjectState.Active });
        }

        public async Task<string> CreateAsync(ProductComponentHierarchy hierarchy)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductComponentHierarchy (
                    Id, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy, StateCode,
                    Name, Description, Notes
                ) VALUES (
                    @Id, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy, @StateCode,
                    @Name, @Description, @Notes
                )";

            await connection.ExecuteAsync(sql, new
            {
                hierarchy.Id,
                hierarchy.CreatedAt,
                hierarchy.CreatedBy,
                hierarchy.LastModifiedAt,
                hierarchy.LastModifiedBy,
                StateCode = (int)hierarchy.StateCode,
                hierarchy.Name,
                hierarchy.Description,
                hierarchy.Notes
            });

            return hierarchy.Id;
        }

        public async Task<bool> UpdateAsync(ProductComponentHierarchy hierarchy)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductComponentHierarchy SET
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy,
                    Name = @Name,
                    Description = @Description,
                    Notes = @Notes
                WHERE Id = @Id AND StateCode = @StateCode";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                hierarchy.LastModifiedAt,
                hierarchy.LastModifiedBy,
                hierarchy.Name,
                hierarchy.Description,
                hierarchy.Notes,
                hierarchy.Id,
                StateCode = (int)ObjectState.Active
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductComponentHierarchy SET 
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                StateCode = (int)ObjectState.Inactive,
                LastModifiedAt = DateTime.UtcNow
            });

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = "SELECT COUNT(1) FROM ProductComponentHierarchy WHERE Id = @Id AND StateCode = @StateCode";
            var count = await connection.QuerySingleAsync<int>(sql, 
                new { Id = id, StateCode = (int)ObjectState.Active });
            
            return count > 0;
        }

        // Consultas específicas (implementação dos métodos mais importantes)

        public async Task<IEnumerable<ProductComponentHierarchy>> GetByNameAsync(string name)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductComponentHierarchy 
                WHERE Name = @Name AND StateCode = @StateCode
                ORDER BY Name";

            return await connection.QueryAsync<ProductComponentHierarchy>(sql, 
                new { Name = name, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductComponentHierarchy>> GetActiveHierarchiesAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductComponentHierarchy 
                WHERE StateCode = @StateCode
                ORDER BY Name";

            return await connection.QueryAsync<ProductComponentHierarchy>(sql, 
                new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductComponentHierarchy>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductComponentHierarchy 
                WHERE (Name LIKE @SearchTerm OR Description LIKE @SearchTerm OR Notes LIKE @SearchTerm)
                  AND StateCode = @StateCode
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<ProductComponentHierarchy>(sql, new
            {
                SearchTerm = searchPattern,
                StateCode = (int)ObjectState.Active
            });
        }

        public async Task<IEnumerable<ProductComponentHierarchy>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductComponentHierarchy 
                WHERE StateCode = @StateCode
                ORDER BY Name
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductComponentHierarchy>(sql, new
            {
                StateCode = (int)ObjectState.Active,
                PageSize = pageSize,
                Offset = offset
            });
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM ProductComponentHierarchy WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<bool> IsNameUniqueAsync(string name, string? excludeId = null)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) FROM ProductComponentHierarchy 
                WHERE Name = @Name AND StateCode = @StateCode";
            
            object parameters;

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                var sqlWithExclude = sql + " AND Id != @ExcludeId";
                parameters = new { Name = name, StateCode = (int)ObjectState.Active, ExcludeId = excludeId };
                var count = await connection.QuerySingleAsync<int>(sqlWithExclude, parameters);
                return count == 0;
            }
            else
            {
                parameters = new { Name = name, StateCode = (int)ObjectState.Active };
                var count = await connection.QuerySingleAsync<int>(sql, parameters);
                return count == 0;
            }
        }

        // Implementação simplificada dos métodos restantes (por brevidade)
        // Em uma implementação completa, todos os métodos da interface seriam implementados

        #region Métodos não implementados completamente (por brevidade)

        public Task<IEnumerable<ProductComponentHierarchy>> GetByCategoryAsync(string category)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetByVersionAsync(string version)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetInactiveHierarchiesAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetDefaultHierarchiesAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetWithComponentsAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetWithCompositeProductsAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<ProductComponentHierarchy?> GetWithAllRelationshipsAsync(string id)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetHierarchiesForProductAsync(string productId)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetAvailableHierarchiesAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetHierarchiesWithMinimumComponentsAsync(int minComponents)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> SearchByNameOrDescriptionAsync(string searchTerm)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetByMultipleCategoriesAsync(IEnumerable<string> categories)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetPagedByStatusAsync(bool isActive, int page, int pageSize)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetPagedByCategoryAsync(string category, int page, int pageSize)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<int> CountActiveAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<int> CountInactiveAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<int> CountByCategoryAsync(string category)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<int> CountWithComponentsAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> CanDeleteAsync(string id)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> HasActiveComponentsAsync(string hierarchyId)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> IsBeingUsedByProductsAsync(string hierarchyId)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> ActivateAsync(string id)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> DeactivateAsync(string id)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> SetAsDefaultAsync(string id)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> RemoveFromDefaultAsync(string id)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetVersionsAsync(string name)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<ProductComponentHierarchy?> GetLatestVersionAsync(string name)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<ProductComponentHierarchy?> GetDefaultVersionAsync(string name)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<string> CreateNewVersionAsync(string baseHierarchyId, string newVersion)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetMostUsedHierarchiesAsync(int top = 10)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetUnusedHierarchiesAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<Dictionary<string, int>> GetCategoryDistributionAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<Dictionary<string, int>> GetUsageStatisticsAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> UpdateStatusBatchAsync(IEnumerable<string> hierarchyIds, bool isActive)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> UpdateCategoryBatchAsync(IEnumerable<string> hierarchyIds, string newCategory)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<bool> DeleteBatchAsync(IEnumerable<string> hierarchyIds)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetHierarchiesWithEstimatedCostAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetHierarchiesByProcessingTimeRangeAsync(int minTime, int maxTime)
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetCompleteHierarchiesAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        public Task<IEnumerable<ProductComponentHierarchy>> GetIncompleteHierarchiesAsync()
        {
            throw new NotImplementedException("Implementar conforme necessário");
        }

        #endregion
    }
} 