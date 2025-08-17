using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.DTOs;

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
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE StateCode = @StateCode 
                ORDER BY Name";

            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { StateCode = (int)ObjectState.Active });



            return ProductMapper.ToEntities(productDtos);
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = "SELECT *, ProductType as ProductTypeString FROM Product WHERE Id = @Id";
            var productDto = await connection.QuerySingleOrDefaultAsync<ProductDto>(sql, new { Id = id });
            

            
            return ProductMapper.ToEntity(productDto);
        }

        public async Task<IEnumerable<Product>> GetByTypeAsync(ProductType productType)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE ProductType = @ProductType AND StateCode = @StateCode
                ORDER BY Name";

            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { ProductType = productType.ToString(), StateCode = (int)ObjectState.Active });

            return ProductMapper.ToEntities(productDtos);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE StateCode = @StateCode
                ORDER BY Name";

            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { StateCode = (int)ObjectState.Active });

            return ProductMapper.ToEntities(productDtos);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE CategoryId = @CategoryId AND StateCode = @StateCode
                ORDER BY Name";

            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { CategoryId = categoryId, StateCode = (int)ObjectState.Active });

            return ProductMapper.ToEntities(productDtos);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE StateCode = @StateCode
                  AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm OR SKU LIKE @SearchTerm)
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";
            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });

            return ProductMapper.ToEntities(productDtos);
        }

        public async Task<IEnumerable<Product>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE StateCode = @StateCode
                ORDER BY Name
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });

            return ProductMapper.ToEntities(productDtos);
        }

        public async Task<string> CreateAsync(Product product)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO Product (
                    Id, Name, Description, Price, QuantityPrice, UnitPrice, CategoryId, Category, SKU,
                    ImageUrl, Note, Cost, AssemblyTime, AssemblyInstructions, ProductType, 
                    StateCode, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @Name, @Description, @Price, @QuantityPrice, @UnitPrice, @CategoryId, @Category, @SKU,
                    @ImageUrl, @Note, @Cost, @AssemblyTime, @AssemblyInstructions, @ProductType, 
                    @StateCode, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
                )";
            
            var parameters = new
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                QuantityPrice = product.QuantityPrice,
                UnitPrice = product.UnitPrice,
                CategoryId = product.CategoryId,
                Category = product.Category,
                SKU = product.SKU,
                ImageUrl = product.ImageUrl,
                Note = product.Note,
                Cost = product.Cost,
                AssemblyTime = product.AssemblyTime,
                AssemblyInstructions = product.AssemblyInstructions,
                ProductType = product.ProductType.ToString(), // Forçar conversão para string
                StateCode = (int)product.StateCode,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                LastModifiedAt = product.LastModifiedAt,
                LastModifiedBy = product.LastModifiedBy
            };
            
            await connection.ExecuteAsync(sql, parameters);
            return product.Id;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Product SET 
                    Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    QuantityPrice = @QuantityPrice,
                    UnitPrice = @UnitPrice,
                    CategoryId = @CategoryId,
                    Category = @Category,
                    SKU = @SKU,
                    ImageUrl = @ImageUrl,
                    Note = @Note,
                    Cost = @Cost,
                    AssemblyTime = @AssemblyTime,
                    AssemblyInstructions = @AssemblyInstructions,
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var parameters = new
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                QuantityPrice = product.QuantityPrice,
                UnitPrice = product.UnitPrice,
                CategoryId = product.CategoryId,
                Category = product.Category,
                SKU = product.SKU,
                ImageUrl = product.ImageUrl,
                Note = product.Note,
                Cost = product.Cost,
                AssemblyTime = product.AssemblyTime,
                AssemblyInstructions = product.AssemblyInstructions,
                StateCode = (int)product.StateCode,
                LastModifiedAt = product.LastModifiedAt,
                LastModifiedBy = product.LastModifiedBy
            };
            
            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
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


    }
} 