using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class ProductionOrderRepository : IProductionOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductionOrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductionOrder>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductionOrder 
                WHERE StateCode = @StateCode
                ORDER BY CreatedAt DESC";

            return await connection.QueryAsync<ProductionOrder>(sql, 
                new { StateCode = (int)ObjectState.Active });
        }

        public async Task<ProductionOrder?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = "SELECT * FROM ProductionOrder WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<ProductionOrder>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ProductionOrder>> GetByOrderIdAsync(string orderId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductionOrder 
                WHERE OrderId = @OrderId AND StateCode = @StateCode
                ORDER BY CreatedAt";

            return await connection.QueryAsync<ProductionOrder>(sql, 
                new { OrderId = orderId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductionOrder>> GetByOrderItemIdAsync(string orderItemId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT po.*, p.* as Product
                FROM ProductionOrder po
                LEFT JOIN Product p ON po.ProductId = p.Id
                WHERE po.OrderItemId = @OrderItemId AND po.StateCode = @StateCode
                ORDER BY po.CreatedAt";

            var result = await connection.QueryAsync<ProductionOrder, Product, ProductionOrder>(
                sql,
                (productionOrder, product) =>
                {
                    productionOrder.Product = product;
                    return productionOrder;
                },
                new { OrderItemId = orderItemId, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductionOrder>> GetByProductIdAsync(string productId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductionOrder 
                WHERE ProductId = @ProductId AND StateCode = @StateCode
                ORDER BY CreatedAt DESC";

            return await connection.QueryAsync<ProductionOrder>(sql, 
                new { ProductId = productId, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductionOrder>> GetByStatusAsync(ProductionOrderStatus status)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM ProductionOrder 
                WHERE Status = @Status AND StateCode = @StateCode
                ORDER BY Priority DESC, CreatedAt";

            return await connection.QueryAsync<ProductionOrder>(sql, 
                new { Status = status.ToString(), StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductionOrder>> GetByPriorityAsync(ProductionOrderPriority priority)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT po.*, p.* as Product
                FROM ProductionOrder po
                LEFT JOIN Product p ON po.ProductId = p.Id
                WHERE po.Priority = @Priority AND po.StateCode = @StateCode
                ORDER BY po.CreatedAt DESC";

            var result = await connection.QueryAsync<ProductionOrder, Product, ProductionOrder>(
                sql,
                (productionOrder, product) =>
                {
                    productionOrder.Product = product;
                    return productionOrder;
                },
                new { Priority = priority.ToString(), StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductionOrder>> GetByAssignedToAsync(string assignedTo)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT po.*, p.* as Product
                FROM ProductionOrder po
                LEFT JOIN Product p ON po.ProductId = p.Id
                WHERE po.AssignedTo = @AssignedTo AND po.StateCode = @StateCode
                ORDER BY po.Priority DESC, po.CreatedAt";

            var result = await connection.QueryAsync<ProductionOrder, Product, ProductionOrder>(
                sql,
                (productionOrder, product) =>
                {
                    productionOrder.Product = product;
                    return productionOrder;
                },
                new { AssignedTo = assignedTo, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductionOrder>> GetPendingOrdersAsync()
        {
            return await GetByStatusAsync(ProductionOrderStatus.Pending);
        }

        public async Task<IEnumerable<ProductionOrder>> GetInProgressOrdersAsync()
        {
            return await GetByStatusAsync(ProductionOrderStatus.InProgress);
        }

        public async Task<IEnumerable<ProductionOrder>> GetCompletedOrdersAsync()
        {
            return await GetByStatusAsync(ProductionOrderStatus.Completed);
        }

        public async Task<IEnumerable<ProductionOrder>> GetScheduledOrdersAsync()
        {
            return await GetByStatusAsync(ProductionOrderStatus.Scheduled);
        }

        public async Task<IEnumerable<ProductionOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT po.*, p.* as Product
                FROM ProductionOrder po
                LEFT JOIN Product p ON po.ProductId = p.Id
                WHERE po.CreatedAt >= @StartDate 
                  AND po.CreatedAt <= @EndDate 
                  AND po.StateCode = @StateCode
                ORDER BY po.CreatedAt DESC";

            var result = await connection.QueryAsync<ProductionOrder, Product, ProductionOrder>(
                sql,
                (productionOrder, product) =>
                {
                    productionOrder.Product = product;
                    return productionOrder;
                },
                new { StartDate = startDate, EndDate = endDate, StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return result;
        }

        public async Task<IEnumerable<ProductionOrder>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT po.*, p.Name as ProductName
                FROM ProductionOrder po
                LEFT JOIN Product p ON po.ProductId = p.Id
                WHERE po.StateCode = @StateCode
                  AND (p.Name LIKE @SearchTerm
                       OR po.AssignedTo LIKE @SearchTerm
                       OR po.Notes LIKE @SearchTerm)
                ORDER BY po.Priority DESC, po.CreatedAt DESC";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<ProductionOrder>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<string> CreateAsync(ProductionOrder productionOrder)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO ProductionOrder (
                    Id, OrderId, OrderItemId, ProductId, Quantity, Status, Priority,
                    ScheduledStartDate, ScheduledEndDate, ActualStartDate, ActualEndDate,
                    AssignedTo, Notes, EstimatedTime, ActualTime, StateCode,
                    CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @OrderId, @OrderItemId, @ProductId, @Quantity, @Status, @Priority,
                    @ScheduledStartDate, @ScheduledEndDate, @ActualStartDate, @ActualEndDate,
                    @AssignedTo, @Notes, @EstimatedTime, @ActualTime, @StateCode,
                    @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
                )";
            
            await connection.ExecuteAsync(sql, productionOrder);
            return productionOrder.Id;
        }

        public async Task<bool> UpdateAsync(ProductionOrder productionOrder)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductionOrder SET 
                    OrderId = @OrderId,
                    OrderItemId = @OrderItemId,
                    ProductId = @ProductId,
                    Quantity = @Quantity,
                    Status = @Status,
                    Priority = @Priority,
                    ScheduledStartDate = @ScheduledStartDate,
                    ScheduledEndDate = @ScheduledEndDate,
                    ActualStartDate = @ActualStartDate,
                    ActualEndDate = @ActualEndDate,
                    AssignedTo = @AssignedTo,
                    Notes = @Notes,
                    EstimatedTime = @EstimatedTime,
                    ActualTime = @ActualTime,
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, productionOrder);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE ProductionOrder SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM ProductionOrder WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM ProductionOrder WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<int> CountByStatusAsync(ProductionOrderStatus status)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(*) FROM ProductionOrder 
                WHERE Status = @Status AND StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, 
                new { Status = status.ToString(), StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<ProductionOrder>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT po.*, p.Name as ProductName
                FROM ProductionOrder po
                LEFT JOIN Product p ON po.ProductId = p.Id
                WHERE po.StateCode = @StateCode
                ORDER BY po.Priority DESC, po.CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductionOrder>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }

        public async Task<IEnumerable<ProductionOrder>> GetByStatusPagedAsync(ProductionOrderStatus status, int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT po.*, p.Name as ProductName
                FROM ProductionOrder po
                LEFT JOIN Product p ON po.ProductId = p.Id
                WHERE po.Status = @Status AND po.StateCode = @StateCode
                ORDER BY po.Priority DESC, po.CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<ProductionOrder>(sql, 
                new { Status = status.ToString(), StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }

        public async Task<bool> UpdateStatusAsync(string id, ProductionOrderStatus status)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductionOrder SET 
                    Status = @Status,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, Status = status.ToString(), LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<bool> AssignToUserAsync(string id, string assignedTo)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE ProductionOrder SET 
                    AssignedTo = @AssignedTo,
                    LastModifiedAt = @LastModifiedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, 
                new { Id = id, AssignedTo = assignedTo, LastModifiedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<decimal> GetAverageCompletionTimeAsync(string? productId = null)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            string sql = @"
                SELECT AVG(ActualTime) FROM ProductionOrder 
                WHERE Status = @Status 
                  AND ActualTime IS NOT NULL 
                  AND StateCode = @StateCode";

            object parameters = new { Status = ProductionOrderStatus.Completed.ToString(), StateCode = (int)ObjectState.Active };

            if (!string.IsNullOrWhiteSpace(productId))
            {
                sql += " AND ProductId = @ProductId";
                parameters = new { Status = ProductionOrderStatus.Completed.ToString(), StateCode = (int)ObjectState.Active, ProductId = productId };
            }

            var result = await connection.QuerySingleOrDefaultAsync<decimal?>(sql, parameters);
            return result ?? 0;
        }

        public async Task<IEnumerable<ProductionOrder>> GetOverdueOrdersAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT po.*, p.* as Product
                FROM ProductionOrder po
                LEFT JOIN Product p ON po.ProductId = p.Id
                WHERE po.ScheduledEndDate < @CurrentDate 
                  AND po.Status NOT IN (@Completed, @Cancelled, @Failed)
                  AND po.StateCode = @StateCode
                ORDER BY po.ScheduledEndDate";

            var result = await connection.QueryAsync<ProductionOrder, Product, ProductionOrder>(
                sql,
                (productionOrder, product) =>
                {
                    productionOrder.Product = product;
                    return productionOrder;
                },
                new { 
                    CurrentDate = DateTime.UtcNow,
                    Completed = ProductionOrderStatus.Completed.ToString(),
                    Cancelled = ProductionOrderStatus.Cancelled.ToString(),
                    Failed = ProductionOrderStatus.Failed.ToString(),
                    StateCode = (int)ObjectState.Active 
                },
                splitOn: "Id");

            return result;
        }
    }
} 