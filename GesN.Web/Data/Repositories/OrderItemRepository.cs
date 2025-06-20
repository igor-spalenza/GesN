using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using Microsoft.Data.Sqlite;

namespace GesN.Web.Data.Repositories
{
    /// <summary>
    /// Implementação do repositório de itens de pedido
    /// </summary>
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<OrderItemRepository> _logger;

        public OrderItemRepository(
            IDbConnectionFactory connectionFactory,
            ILogger<OrderItemRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT oi.*, p.*
                    FROM OrderItem oi
                    LEFT JOIN Product p ON oi.ProductId = p.Id
                    WHERE oi.OrderId = @OrderId
                    AND oi.StateCode = @StateCode
                    ORDER BY oi.CreatedAt";

                var itemDict = new Dictionary<string, OrderItem>();

                await connection.QueryAsync<OrderItem, Product?, OrderItem>(
                    sql,
                    (item, product) =>
                    {
                        if (!itemDict.TryGetValue(item.Id, out var existingItem))
                        {
                            existingItem = item;
                            itemDict.Add(item.Id, existingItem);
                        }

                        if (product != null)
                            existingItem.Product = product;

                        return existingItem;
                    },
                    new { OrderId = orderId, StateCode = (int)ObjectState.Active },
                    splitOn: "Id");

                return itemDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<OrderItem?> GetByIdAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT oi.*, p.*
                    FROM OrderItem oi
                    LEFT JOIN Product p ON oi.ProductId = p.Id
                    WHERE oi.Id = @Id";

                var itemDict = new Dictionary<string, OrderItem>();

                await connection.QueryAsync<OrderItem, Product?, OrderItem>(
                    sql,
                    (item, product) =>
                    {
                        if (!itemDict.TryGetValue(item.Id, out var existingItem))
                        {
                            existingItem = item;
                            itemDict.Add(item.Id, existingItem);
                        }

                        if (product != null)
                            existingItem.Product = product;

                        return existingItem;
                    },
                    new { Id = id },
                    splitOn: "Id");

                return itemDict.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar item por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderItem>> GetByProductIdAsync(string productId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT oi.*, p.*
                    FROM OrderItem oi
                    LEFT JOIN Product p ON oi.ProductId = p.Id
                    WHERE oi.ProductId = @ProductId
                    AND oi.StateCode = @StateCode
                    ORDER BY oi.CreatedAt DESC";

                var itemDict = new Dictionary<string, OrderItem>();

                await connection.QueryAsync<OrderItem, Product?, OrderItem>(
                    sql,
                    (item, product) =>
                    {
                        if (!itemDict.TryGetValue(item.Id, out var existingItem))
                        {
                            existingItem = item;
                            itemDict.Add(item.Id, existingItem);
                        }

                        if (product != null)
                            existingItem.Product = product;

                        return existingItem;
                    },
                    new { ProductId = productId, StateCode = (int)ObjectState.Active },
                    splitOn: "Id");

                return itemDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do produto: {ProductId}", productId);
                throw;
            }
        }

        public async Task<string> CreateAsync(OrderItem item)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    INSERT INTO OrderItem (
                        Id, OrderId, ProductId, Quantity, UnitPrice,
                        DiscountAmount, TaxAmount, Notes, StateCode,
                        CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                    ) VALUES (
                        @Id, @OrderId, @ProductId, @Quantity, @UnitPrice,
                        @DiscountAmount, @TaxAmount, @Notes, @StateCode,
                        @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
                    )";

                await connection.ExecuteAsync(sql, item);
                return item.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar item de pedido");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(OrderItem item)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    UPDATE OrderItem SET
                        Quantity = @Quantity,
                        UnitPrice = @UnitPrice,
                        DiscountAmount = @DiscountAmount,
                        TaxAmount = @TaxAmount,
                        Notes = @Notes,
                        StateCode = @StateCode,
                        LastModifiedAt = @LastModifiedAt,
                        LastModifiedBy = @LastModifiedBy
                    WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(sql, item);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item de pedido: {Id}", item.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "UPDATE OrderItem SET StateCode = @StateCode WHERE Id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir item de pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "SELECT COUNT(1) FROM OrderItem WHERE Id = @Id";
                var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do item: {Id}", id);
                throw;
            }
        }

        public async Task<int> CountByOrderIdAsync(string orderId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "SELECT COUNT(*) FROM OrderItem WHERE OrderId = @OrderId AND StateCode = @StateCode";
                return await connection.QuerySingleAsync<int>(sql, new { OrderId = orderId, StateCode = (int)ObjectState.Active });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar itens do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<int> CountByProductIdAsync(string productId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "SELECT COUNT(*) FROM OrderItem WHERE ProductId = @ProductId AND StateCode = @StateCode";
                return await connection.QuerySingleAsync<int>(sql, new { ProductId = productId, StateCode = (int)ObjectState.Active });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar itens do produto: {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> DeleteByOrderIdAsync(string orderId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "UPDATE OrderItem SET StateCode = @StateCode WHERE OrderId = @OrderId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, OrderId = orderId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir itens do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> ExistsByOrderIdAsync(string orderId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "SELECT COUNT(1) FROM OrderItem WHERE OrderId = @OrderId AND StateCode = @StateCode";
                var count = await connection.QuerySingleAsync<int>(sql, new { OrderId = orderId, StateCode = (int)ObjectState.Active });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência de itens para o pedido: {OrderId}", orderId);
                throw;
            }
        }
    }
} 