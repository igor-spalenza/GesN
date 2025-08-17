using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class ProductIngredientRepository : IProductIngredientRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductIngredientRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductIngredient>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pi.*, i.Name as IngredientName, p.Name as ProductName
                FROM ProductIngredient pi
                LEFT JOIN Ingredient i ON pi.IngredientId = i.Id
                LEFT JOIN Product p ON pi.ProductId = p.Id
                WHERE pi.StateCode = @StateCode
                ORDER BY p.Name, i.Name";

            return await connection.QueryAsync<ProductIngredient>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductIngredient?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pi.*, i.Name as IngredientName, p.Name as ProductName
                FROM ProductIngredient pi
                LEFT JOIN Ingredient i ON pi.IngredientId = i.Id
                LEFT JOIN Product p ON pi.ProductId = p.Id
                WHERE pi.Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<ProductIngredient>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ProductIngredient>> GetByProductIdAsync(string productId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pi.*, i.Name as IngredientName, i.Unit as IngredientUnit, i.CostPerUnit, i.CurrentStock
                FROM ProductIngredient pi
                INNER JOIN Ingredient i ON pi.IngredientId = i.Id
                WHERE pi.ProductId = @ProductId AND pi.StateCode = @StateCode
                ORDER BY i.Name";

            return await connection.QueryAsync<ProductIngredient>(sql, 
                new { ProductId = productId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductIngredient>> GetByIngredientIdAsync(string ingredientId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pi.*, p.Name as ProductName
                FROM ProductIngredient pi
                INNER JOIN Product p ON pi.ProductId = p.Id
                WHERE pi.IngredientId = @IngredientId AND pi.StateCode = @StateCode
                ORDER BY p.Name";

            return await connection.QueryAsync<ProductIngredient>(sql, 
                new { IngredientId = ingredientId, StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductIngredient?> GetByProductAndIngredientAsync(string productId, string ingredientId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductIngredient 
                WHERE ProductId = @ProductId 
                  AND IngredientId = @IngredientId 
                  AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<ProductIngredient>(sql, 
                new { ProductId = productId, IngredientId = ingredientId, StateCode = (int)ObjectState.Active });
        }

        public async Task<string> CreateAsync(ProductIngredient productIngredient)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductIngredient (Id, ProductId, IngredientId, Quantity, Unit, IsOptional, Notes, StateCode, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
                VALUES (@Id, @ProductId, @IngredientId, @Quantity, @Unit, @IsOptional, @Notes, @StateCode, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy)";
            
            await connection.ExecuteAsync(sql, productIngredient);
            return productIngredient.Id;
        }

        public async Task<bool> UpdateAsync(ProductIngredient productIngredient)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductIngredient SET 
                ProductId = @ProductId, IngredientId = @IngredientId, Quantity = @Quantity, 
                Unit = @Unit, IsOptional = @IsOptional, Notes = @Notes,
                StateCode = @StateCode, LastModifiedAt = @LastModifiedAt, LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, productIngredient);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE ProductIngredient SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByProductIdAsync(string productId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE ProductIngredient SET StateCode = @StateCode WHERE ProductId = @ProductId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, ProductId = productId });
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByIngredientIdAsync(string ingredientId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE ProductIngredient SET StateCode = @StateCode WHERE IngredientId = @IngredientId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, IngredientId = ingredientId });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM ProductIngredient WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM ProductIngredient WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductIngredient>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT pi.*, i.Name as IngredientName, p.Name as ProductName
                FROM ProductIngredient pi
                LEFT JOIN Ingredient i ON pi.IngredientId = i.Id
                LEFT JOIN Product p ON pi.ProductId = p.Id
                WHERE pi.StateCode = @StateCode
                ORDER BY p.Name, i.Name
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductIngredient>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }
    }
} 