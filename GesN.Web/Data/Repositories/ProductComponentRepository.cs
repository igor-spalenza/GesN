using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class ProductComponentRepository : IProductComponentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductComponentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductComponent>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT 
                    pc.Id, pc.Name, pc.Description, pc.ProductComponentHierarchyId, 
                    pc.AdditionalCost, pc.StateCode, pc.CreatedAt, pc.CreatedBy, 
                    pc.LastModifiedAt, pc.LastModifiedBy,
                    pch.Id as HierarchyId, pch.Name as HierarchyName, pch.Description as HierarchyDescription
                FROM ProductComponent pc
                LEFT JOIN ProductComponentHierarchy pch ON pc.ProductComponentHierarchyId = pch.Id 
                WHERE pc.StateCode = @StateCode
                ORDER BY pc.Name";

            var components = await connection.QueryAsync<ProductComponent, ProductComponentHierarchy, ProductComponent>(
                sql,
                (component, hierarchy) =>
                {
                    if (hierarchy != null)
                    {
                        component.ProductComponentHierarchy = hierarchy;
                    }
                    return component;
                },
                new { StateCode = (int)ObjectState.Active },
                splitOn: "HierarchyId"
            );

            return components;
        }

        public async Task<ProductComponent?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT 
                    pc.Id, pc.Name, pc.Description, pc.ProductComponentHierarchyId, 
                    pc.AdditionalCost, pc.StateCode, pc.CreatedAt, pc.CreatedBy, 
                    pc.LastModifiedAt, pc.LastModifiedBy,
                    pch.Id as HierarchyId, pch.Name as HierarchyName, pch.Description as HierarchyDescription
                FROM ProductComponent pc
                LEFT JOIN ProductComponentHierarchy pch ON pc.ProductComponentHierarchyId = pch.Id
                WHERE pc.Id = @Id";

            var result = await connection.QueryAsync<ProductComponent, ProductComponentHierarchy, ProductComponent>(
                sql,
                (component, hierarchy) =>
                {
                    if (hierarchy != null)
                    {
                        component.ProductComponentHierarchy = hierarchy;
                    }
                    return component;
                },
                new { Id = id },
                splitOn: "HierarchyId"
            );

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<ProductComponent>> GetByHierarchyIdAsync(string hierarchyId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT 
                    pc.Id, pc.Name, pc.Description, pc.ProductComponentHierarchyId, 
                    pc.AdditionalCost, pc.StateCode, pc.CreatedAt, pc.CreatedBy, 
                    pc.LastModifiedAt, pc.LastModifiedBy,
                    pch.Id as HierarchyId, pch.Name as HierarchyName, pch.Description as HierarchyDescription
                FROM ProductComponent pc
                LEFT JOIN ProductComponentHierarchy pch ON pc.ProductComponentHierarchyId = pch.Id
                WHERE pc.ProductComponentHierarchyId = @HierarchyId 
                AND pc.StateCode = @StateCode
                ORDER BY pc.Name";

            var components = await connection.QueryAsync<ProductComponent, ProductComponentHierarchy, ProductComponent>(
                sql,
                (component, hierarchy) =>
                {
                    if (hierarchy != null)
                    {
                        component.ProductComponentHierarchy = hierarchy;
                    }
                    return component;
                },
                new { HierarchyId = hierarchyId, StateCode = (int)ObjectState.Active },
                splitOn: "HierarchyId"
            );

            return components;
        }

        public async Task<bool> CreateAsync(ProductComponent component)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductComponent (
                    Id, Name, Description, ProductComponentHierarchyId, 
                    AdditionalCost, StateCode, CreatedAt, CreatedBy, 
                    LastModifiedAt, LastModifiedBy
                ) VALUES (
                    @Id, @Name, @Description, @ProductComponentHierarchyId,
                    @AdditionalCost, @StateCode, @CreatedAt, @CreatedBy,
                    @LastModifiedAt, @LastModifiedBy
                )";

            var rowsAffected = await connection.ExecuteAsync(sql, component);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAsync(ProductComponent component)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductComponent SET
                    Name = @Name,
                    Description = @Description,
                    ProductComponentHierarchyId = @ProductComponentHierarchyId,
                    AdditionalCost = @AdditionalCost,
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, component);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductComponent 
                SET StateCode = @StateCode,
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

        public async Task<IEnumerable<ProductComponent>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT 
                    pc.Id, pc.Name, pc.Description, pc.ProductComponentHierarchyId, 
                    pc.AdditionalCost, pc.StateCode, pc.CreatedAt, pc.CreatedBy, 
                    pc.LastModifiedAt, pc.LastModifiedBy,
                    pch.Id as HierarchyId, pch.Name as HierarchyName, pch.Description as HierarchyDescription
                FROM ProductComponent pc
                LEFT JOIN ProductComponentHierarchy pch ON pc.ProductComponentHierarchyId = pch.Id
                WHERE pc.StateCode = @StateCode
                AND (pc.Name LIKE @SearchTerm 
                     OR pc.Description LIKE @SearchTerm
                     OR pch.Name LIKE @SearchTerm)
                ORDER BY pc.Name
                LIMIT 50";

            var components = await connection.QueryAsync<ProductComponent, ProductComponentHierarchy, ProductComponent>(
                sql,
                (component, hierarchy) =>
                {
                    if (hierarchy != null)
                    {
                        component.ProductComponentHierarchy = hierarchy;
                    }
                    return component;
                },
                new { 
                    StateCode = (int)ObjectState.Active,
                    SearchTerm = $"%{searchTerm}%"
                },
                splitOn: "HierarchyId"
            );

            return components;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = "SELECT COUNT(*) FROM ProductComponent WHERE StateCode = @StateCode";
            
            return await connection.ExecuteScalarAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = "SELECT COUNT(*) FROM ProductComponent WHERE Id = @Id AND StateCode = @StateCode";
            
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id, StateCode = (int)ObjectState.Active });
            return count > 0;
        }

        // Métodos obsoletos mas mantidos para compatibilidade (retornarão listas vazias)
        [Obsolete("ProductComponent não possui mais CompositeProductId. Use GetByHierarchyIdAsync()")]
        public async Task<IEnumerable<ProductComponent>> GetByCompositeProductIdAsync(string compositeProductId)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais ComponentProductId")]
        public async Task<IEnumerable<ProductComponent>> GetByComponentProductIdAsync(string componentProductId)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais relacionamento direto com produtos")]
        public async Task<ProductComponent?> GetByCompositeAndComponentProductAsync(string compositeProductId, string componentProductId)
        {
            return null;
        }

        [Obsolete("ProductComponent não possui mais CompositeProductId")]
        public async Task<int> CountByCompositeProductAsync(string compositeProductId)
        {
            return 0;
        }

        [Obsolete("ProductComponent não possui mais CompositeProductId")]
        public async Task<IEnumerable<ProductComponent>> GetByCompositeProductPagedAsync(string compositeProductId, int page, int pageSize)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais relacionamento direto com produtos")]
        public async Task<bool> ComponentExistsInCompositeAsync(string compositeProductId, string componentProductId)
        {
            return false;
        }

        [Obsolete("ProductComponent não possui mais AssemblyOrder")]
        public async Task<IEnumerable<ProductComponent>> GetOrderedByAssemblyAsync(string compositeProductId)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais IsOptional")]
        public async Task<IEnumerable<ProductComponent>> GetOptionalComponentsAsync(string compositeProductId)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais IsOptional")]
        public async Task<IEnumerable<ProductComponent>> GetRequiredComponentsAsync(string compositeProductId)
        {
            return new List<ProductComponent>();
        }
    }
} 