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
                SELECT pc.*, 
                       cp.Name as CompositeProductName,
                       comp.Name as ComponentProductName
                FROM ProductComponent pc
                LEFT JOIN Product cp ON pc.CompositeProductId = cp.Id
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.StateCode = @StateCode
                ORDER BY pc.CompositeProductId, pc.AssemblyOrder";

            var components = await connection.QueryAsync<ProductComponent, Product, Product, ProductComponent>(
                sql, 
                (component, compositeProduct, componentProduct) =>
                {
                    component.CompositeProduct = compositeProduct;
                    component.ComponentProduct = componentProduct;
                    return component;
                },
                new { StateCode = (int)ObjectState.Active },
                splitOn: "CompositeProductName,ComponentProductName");

            return components;
        }

        public async Task<ProductComponent?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       cp.* as CompositeProduct,
                       comp.* as ComponentProduct
                FROM ProductComponent pc
                LEFT JOIN Product cp ON pc.CompositeProductId = cp.Id
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.Id = @Id";

            var result = await connection.QueryAsync<ProductComponent, Product, Product, ProductComponent>(
                sql,
                (component, compositeProduct, componentProduct) =>
                {
                    component.CompositeProduct = compositeProduct;
                    component.ComponentProduct = componentProduct;
                    return component;
                },
                new { Id = id },
                splitOn: "Id,Id");

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<ProductComponent>> GetByCompositeProductIdAsync(string compositeProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       comp.* as ComponentProduct
                FROM ProductComponent pc
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.CompositeProductId = @CompositeProductId 
                  AND pc.StateCode = @StateCode
                ORDER BY pc.AssemblyOrder";

            var result = await connection.QueryAsync<ProductComponent, Product, ProductComponent>(
                sql,
                (component, componentProduct) =>
                {
                    component.ComponentProduct = componentProduct;
                    return component;
                },
                new { CompositeProductId = compositeProductId, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductComponent>> GetByComponentProductIdAsync(string componentProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       cp.* as CompositeProduct
                FROM ProductComponent pc
                LEFT JOIN Product cp ON pc.CompositeProductId = cp.Id
                WHERE pc.ComponentProductId = @ComponentProductId 
                  AND pc.StateCode = @StateCode
                ORDER BY cp.Name";

            var result = await connection.QueryAsync<ProductComponent, Product, ProductComponent>(
                sql,
                (component, compositeProduct) =>
                {
                    component.CompositeProduct = compositeProduct;
                    return component;
                },
                new { ComponentProductId = componentProductId, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductComponent>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       cp.Name as CompositeProductName,
                       comp.Name as ComponentProductName
                FROM ProductComponent pc
                LEFT JOIN Product cp ON pc.CompositeProductId = cp.Id
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.StateCode = @StateCode
                  AND (cp.Name LIKE @SearchTerm
                       OR comp.Name LIKE @SearchTerm
                       OR pc.Notes LIKE @SearchTerm)
                ORDER BY cp.Name, pc.AssemblyOrder";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<ProductComponent>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<string> CreateAsync(ProductComponent component)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductComponent (
                    Id, CompositeProductId, ComponentProductId, Quantity, Unit, 
                    IsOptional, AssemblyOrder, Notes, StateCode, CreatedAt, 
                    CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @CompositeProductId, @ComponentProductId, @Quantity, @Unit,
                    @IsOptional, @AssemblyOrder, @Notes, @StateCode, @CreatedAt,
                    @CreatedBy, @LastModifiedAt, @LastModifiedBy
                )";
            
            await connection.ExecuteAsync(sql, component);
            return component.Id;
        }

        public async Task<bool> UpdateAsync(ProductComponent component)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductComponent SET 
                    CompositeProductId = @CompositeProductId,
                    ComponentProductId = @ComponentProductId,
                    Quantity = @Quantity,
                    Unit = @Unit,
                    IsOptional = @IsOptional,
                    AssemblyOrder = @AssemblyOrder,
                    Notes = @Notes,
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
            const string sql = "UPDATE ProductComponent SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM ProductComponent WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM ProductComponent WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<int> CountByCompositeProductAsync(string compositeProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(*) FROM ProductComponent 
                WHERE CompositeProductId = @CompositeProductId AND StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, 
                new { CompositeProductId = compositeProductId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductComponent>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       cp.Name as CompositeProductName,
                       comp.Name as ComponentProductName
                FROM ProductComponent pc
                LEFT JOIN Product cp ON pc.CompositeProductId = cp.Id
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.StateCode = @StateCode
                ORDER BY cp.Name, pc.AssemblyOrder
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductComponent>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }

        public async Task<IEnumerable<ProductComponent>> GetByCompositeProductPagedAsync(string compositeProductId, int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       comp.* as ComponentProduct
                FROM ProductComponent pc
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.CompositeProductId = @CompositeProductId 
                  AND pc.StateCode = @StateCode
                ORDER BY pc.AssemblyOrder
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            var result = await connection.QueryAsync<ProductComponent, Product, ProductComponent>(
                sql,
                (component, componentProduct) =>
                {
                    component.ComponentProduct = componentProduct;
                    return component;
                },
                new { CompositeProductId = compositeProductId, StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset },
                splitOn: "Id");

            return result;
        }

        public async Task<bool> ComponentExistsInCompositeAsync(string compositeProductId, string componentProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(1) FROM ProductComponent 
                WHERE CompositeProductId = @CompositeProductId 
                  AND ComponentProductId = @ComponentProductId 
                  AND StateCode = @StateCode";
            var count = await connection.QuerySingleAsync<int>(sql, 
                new { CompositeProductId = compositeProductId, ComponentProductId = componentProductId, StateCode = (int)ObjectState.Active });
            return count > 0;
        }

        public async Task<IEnumerable<ProductComponent>> GetOrderedByAssemblyAsync(string compositeProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       comp.* as ComponentProduct
                FROM ProductComponent pc
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.CompositeProductId = @CompositeProductId 
                  AND pc.StateCode = @StateCode
                ORDER BY pc.AssemblyOrder";

            var result = await connection.QueryAsync<ProductComponent, Product, ProductComponent>(
                sql,
                (component, componentProduct) =>
                {
                    component.ComponentProduct = componentProduct;
                    return component;
                },
                new { CompositeProductId = compositeProductId, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductComponent>> GetOptionalComponentsAsync(string compositeProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       comp.* as ComponentProduct
                FROM ProductComponent pc
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.CompositeProductId = @CompositeProductId 
                  AND pc.IsOptional = 1
                  AND pc.StateCode = @StateCode
                ORDER BY pc.AssemblyOrder";

            var result = await connection.QueryAsync<ProductComponent, Product, ProductComponent>(
                sql,
                (component, componentProduct) =>
                {
                    component.ComponentProduct = componentProduct;
                    return component;
                },
                new { CompositeProductId = compositeProductId, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductComponent>> GetRequiredComponentsAsync(string compositeProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       comp.* as ComponentProduct
                FROM ProductComponent pc
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.CompositeProductId = @CompositeProductId 
                  AND pc.IsOptional = 0
                  AND pc.StateCode = @StateCode
                ORDER BY pc.AssemblyOrder";

            var result = await connection.QueryAsync<ProductComponent, Product, ProductComponent>(
                sql,
                (component, componentProduct) =>
                {
                    component.ComponentProduct = componentProduct;
                    return component;
                },
                new { CompositeProductId = compositeProductId, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<ProductComponent?> GetByCompositeAndComponentProductAsync(string compositeProductId, string componentProductId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pc.*, 
                       cp.* as CompositeProduct,
                       comp.* as ComponentProduct
                FROM ProductComponent pc
                LEFT JOIN Product cp ON pc.CompositeProductId = cp.Id
                LEFT JOIN Product comp ON pc.ComponentProductId = comp.Id
                WHERE pc.CompositeProductId = @CompositeProductId 
                  AND pc.ComponentProductId = @ComponentProductId
                  AND pc.StateCode = @StateCode";

            var result = await connection.QueryAsync<ProductComponent, Product, Product, ProductComponent>(
                sql,
                (component, compositeProduct, componentProduct) =>
                {
                    component.CompositeProduct = compositeProduct;
                    component.ComponentProduct = componentProduct;
                    return component;
                },
                new { CompositeProductId = compositeProductId, ComponentProductId = componentProductId, StateCode = (int)ObjectState.Active },
                splitOn: "Id,Id");

            return result.FirstOrDefault();
        }
    }
} 