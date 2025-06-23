using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Product 
                WHERE StateCode = @StateCode
                ORDER BY Name";

            var products = await connection.QueryAsync<Product>(sql, 
                new { StateCode = (int)ObjectState.Active });

            return ConvertToSpecificTypes(products);
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = "SELECT * FROM Product WHERE Id = @Id";
            var product = await connection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
            
            return ConvertToSpecificType(product);
        }

        public async Task<IEnumerable<Product>> GetByTypeAsync(ProductType productType)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Product 
                WHERE ProductType = @ProductType AND StateCode = @StateCode
                ORDER BY Name";

            var products = await connection.QueryAsync<Product>(sql, 
                new { ProductType = productType.ToString(), StateCode = (int)ObjectState.Active });

            return ConvertToSpecificTypes(products);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Product 
                WHERE StateCode = @StateCode
                ORDER BY Name";

            var products = await connection.QueryAsync<Product>(sql, 
                new { StateCode = (int)ObjectState.Active });

            return ConvertToSpecificTypes(products);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Product 
                WHERE CategoryId = @CategoryId AND StateCode = @StateCode
                ORDER BY Name";

            var products = await connection.QueryAsync<Product>(sql, 
                new { CategoryId = categoryId, StateCode = (int)ObjectState.Active });

            return ConvertToSpecificTypes(products);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Product 
                WHERE StateCode = @StateCode
                  AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm)
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";
            var products = await connection.QueryAsync<Product>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });

            return ConvertToSpecificTypes(products);
        }

        public async Task<IEnumerable<Product>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Product 
                WHERE StateCode = @StateCode
                ORDER BY Name
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            var products = await connection.QueryAsync<Product>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });

            return ConvertToSpecificTypes(products);
        }

        public async Task<string> CreateAsync(Product product)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO Product (
                    Id, Code, Name, Description, UnitPrice, Cost, CategoryId, ProductType, Unit, SupplierId,
                    StateCode, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @Code, @Name, @Description, @UnitPrice, @Cost, @CategoryId, @ProductType, @Unit, @SupplierId,
                    @StateCode, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
                )";
            
            await connection.ExecuteAsync(sql, product);
            return product.Id;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Product SET 
                    Code = @Code,
                    Name = @Name,
                    Description = @Description,
                    UnitPrice = @UnitPrice,
                    Cost = @Cost,
                    CategoryId = @CategoryId,
                    Unit = @Unit,
                    SupplierId = @SupplierId,
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, product);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE Product SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM Product WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM Product WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        private Product? ConvertToSpecificType(Product? product)
        {
            if (product == null) return null;

            return product.ProductType switch
            {
                ProductType.Simple => new SimpleProduct
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    Cost = product.Cost,
                    CategoryId = product.CategoryId,
                    Unit = product.Unit,
                    SupplierId = product.SupplierId,
                    StateCode = product.StateCode,
                    CreatedAt = product.CreatedAt,
                    CreatedBy = product.CreatedBy,
                    LastModifiedAt = product.LastModifiedAt,
                    LastModifiedBy = product.LastModifiedBy
                },
                ProductType.Composite => new CompositeProduct
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    Cost = product.Cost,
                    CategoryId = product.CategoryId,
                    Unit = product.Unit,
                    SupplierId = product.SupplierId,
                    StateCode = product.StateCode,
                    CreatedAt = product.CreatedAt,
                    CreatedBy = product.CreatedBy,
                    LastModifiedAt = product.LastModifiedAt,
                    LastModifiedBy = product.LastModifiedBy
                },
                ProductType.Group => new ProductGroup
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    Cost = product.Cost,
                    CategoryId = product.CategoryId,
                    Unit = product.Unit,
                    SupplierId = product.SupplierId,
                    StateCode = product.StateCode,
                    CreatedAt = product.CreatedAt,
                    CreatedBy = product.CreatedBy,
                    LastModifiedAt = product.LastModifiedAt,
                    LastModifiedBy = product.LastModifiedBy
                },
                _ => product
            };
        }

        private IEnumerable<Product> ConvertToSpecificTypes(IEnumerable<Product> products)
        {
            return products.Select(p => ConvertToSpecificType(p)!);
        }
    }
} 