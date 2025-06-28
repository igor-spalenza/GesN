using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Entities.ValueObjects;
using GesN.Web.Models.Enumerators;
using Microsoft.Data.Sqlite;

namespace GesN.Web.Data.Repositories
{
    /// <summary>
    /// Implementação do repositório de pedidos
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(
            IDbConnectionFactory connectionFactory,
            ILogger<OrderRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderEntry>> GetAllAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, OrderEntry>();

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new { StateCode = (int)ObjectState.Active },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os pedidos");
                throw;
            }
        }

        public async Task<OrderEntry?> GetByIdAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.Id = @Id";

                var orderDict = new Dictionary<string, OrderEntry>();

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new { Id = id },
                    splitOn: "Id,Id,Id");

                return orderDict.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido por ID: {Id}", id);
                throw;
            }
        }

        public async Task<OrderEntry?> GetByNumberAsync(string numberSequence)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.NumberSequence = @NumberSequence";

                var orderDict = new Dictionary<string, OrderEntry>();

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new { NumberSequence = numberSequence },
                    splitOn: "Id,Id,Id");

                return orderDict.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido por número: {Number}", numberSequence);
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetByCustomerIdAsync(string customerId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.CustomerId = @CustomerId
                    AND o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, OrderEntry>();

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new { CustomerId = customerId, StateCode = (int)ObjectState.Active },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos do cliente: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetByStatusAsync(OrderStatus status)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.Status = @Status
                    AND o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, OrderEntry>();

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new { Status = status.ToString(), StateCode = (int)ObjectState.Active },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetActiveAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    AND o.Status NOT IN (@Cancelled, @Completed)
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, OrderEntry>();

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new 
                    { 
                        StateCode = (int)ObjectState.Active,
                        Cancelled = OrderStatus.Cancelled.ToString(),
                        Completed = OrderStatus.Completed.ToString()
                    },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos ativos");
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetPendingDeliveryAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    AND o.Status IN (@ReadyForDelivery, @InDelivery)
                    ORDER BY o.DeliveryDate ASC";

                var orderDict = new Dictionary<string, OrderEntry>();

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new 
                    { 
                        StateCode = (int)ObjectState.Active,
                        ReadyForDelivery = OrderStatus.ReadyForDelivery.ToString(),
                        InDelivery = OrderStatus.InDelivery.ToString()
                    },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos pendentes de entrega");
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetPendingPrintAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    AND o.PrintStatus = @PrintStatus
                    AND o.Status NOT IN (@Draft, @Cancelled)
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, OrderEntry>();

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new 
                    { 
                        StateCode = (int)ObjectState.Active,
                        PrintStatus = PrintStatus.NotPrinted.ToString(),
                        Draft = OrderStatus.Draft.ToString(),
                        Cancelled = OrderStatus.Cancelled.ToString()
                    },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos pendentes de impressão");
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> SearchAsync(string searchTerm)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    AND (o.NumberSequence LIKE @SearchTerm 
                         OR c.FirstName LIKE @SearchTerm 
                         OR c.LastName LIKE @SearchTerm
                         OR o.Notes LIKE @SearchTerm)
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, OrderEntry>();
                var searchPattern = $"%{searchTerm}%";

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por termo de pesquisa: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<string> CreateAsync(OrderEntry order)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    INSERT INTO OrderEntry (
                        Id, CreatedAt, CreatedBy, StateCode,
                        NumberSequence, OrderDate, DeliveryDate, CustomerId,
                        Status, Type, TotalAmount, Subtotal, TaxAmount, DiscountAmount,
                        Notes, DeliveryAddressId, RequiresFiscalReceipt, FiscalDataId,
                        PrintStatus, PrintBatchNumber
                    ) VALUES (
                        @Id, @CreatedAt, @CreatedBy, @StateCode,
                        @NumberSequence, @OrderDate, @DeliveryDate, @CustomerId,
                        @Status, @Type, @TotalAmount, @Subtotal, @TaxAmount, @DiscountAmount,
                        @Notes, @DeliveryAddressId, @RequiresFiscalReceipt, @FiscalDataId,
                        @PrintStatus, @PrintBatchNumber
                    )";

                var parameters = new
                {
                    order.Id,
                    order.CreatedAt,
                    order.CreatedBy,
                    order.StateCode,
                    order.NumberSequence,
                    order.OrderDate,
                    order.DeliveryDate,
                    order.CustomerId,
                    Status = order.Status.ToString(),
                    Type = order.Type.ToString(),
                    order.TotalAmount,
                    order.Subtotal,
                    order.TaxAmount,
                    order.DiscountAmount,
                    order.Notes,
                    order.DeliveryAddressId,
                    order.RequiresFiscalReceipt,
                    order.FiscalDataId,
                    PrintStatus = order.PrintStatus.ToString(),
                    order.PrintBatchNumber
                };

                await connection.ExecuteAsync(sql, parameters);
                return order.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido: {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(OrderEntry order)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    UPDATE OrderEntry SET
                        LastModifiedAt = @LastModifiedAt,
                        LastModifiedBy = @LastModifiedBy,
                        NumberSequence = @NumberSequence,
                        OrderDate = @OrderDate,
                        DeliveryDate = @DeliveryDate,
                        CustomerId = @CustomerId,
                        Status = @Status,
                        Type = @Type,
                        TotalAmount = @TotalAmount,
                        Subtotal = @Subtotal,
                        TaxAmount = @TaxAmount,
                        DiscountAmount = @DiscountAmount,
                        Notes = @Notes,
                        DeliveryAddressId = @DeliveryAddressId,
                        RequiresFiscalReceipt = @RequiresFiscalReceipt,
                        FiscalDataId = @FiscalDataId,
                        PrintStatus = @PrintStatus,
                        PrintBatchNumber = @PrintBatchNumber
                    WHERE Id = @Id";

                var parameters = new
                {
                    order.Id,
                    order.LastModifiedAt,
                    order.LastModifiedBy,
                    order.NumberSequence,
                    order.OrderDate,
                    order.DeliveryDate,
                    order.CustomerId,
                    Status = order.Status.ToString(),
                    Type = order.Type.ToString(),
                    order.TotalAmount,
                    order.Subtotal,
                    order.TaxAmount,
                    order.DiscountAmount,
                    order.Notes,
                    order.DeliveryAddressId,
                    order.RequiresFiscalReceipt,
                    order.FiscalDataId,
                    PrintStatus = order.PrintStatus.ToString(),
                    order.PrintBatchNumber
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido: {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = "UPDATE OrderEntry SET StateCode = @StateCode WHERE Id = @Id";
                
                var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido: {OrderId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = "SELECT COUNT(1) FROM OrderEntry WHERE Id = @Id";
                
                var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do pedido: {OrderId}", id);
                throw;
            }
        }

        public async Task<int> CountAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = "SELECT COUNT(*) FROM OrderEntry WHERE StateCode = @StateCode";
                
                return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar pedidos");
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM OrderEntry o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC
                    LIMIT @PageSize OFFSET @Offset";

                var orderDict = new Dictionary<string, OrderEntry>();
                var offset = (page - 1) * pageSize;

                await connection.QueryAsync<OrderEntry, Customer?, Address?, FiscalData?, OrderEntry>(
                    sql,
                    (order, customer, address, fiscalData) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                        {
                            existingOrder = order;
                            orderDict.Add(order.Id, existingOrder);
                        }

                        if (customer != null)
                            existingOrder.Customer = customer;

                        if (address != null)
                            existingOrder.DeliveryAddress = address;

                        if (fiscalData != null)
                            existingOrder.FiscalData = fiscalData;

                        return existingOrder;
                    },
                    new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos paginados");
                throw;
            }
        }

        public async Task<string> GetNextNumberSequenceAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT COUNT(*) + 1
                    FROM OrderEntry
                    WHERE strftime('%Y', OrderDate) = strftime('%Y', 'now')";

                var nextNumber = await connection.QuerySingleAsync<int>(sql);
                var year = DateTime.Now.Year;
                
                return $"PED-{year}-{nextNumber:D4}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar próximo número sequencial");
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(string id, OrderStatus status, string modifiedBy)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    UPDATE OrderEntry
                    SET Status = @Status, LastModifiedAt = @LastModifiedAt, LastModifiedBy = @LastModifiedBy
                    WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(sql, new 
                { 
                    Id = id, 
                    Status = status.ToString(), 
                    LastModifiedAt = DateTime.UtcNow, 
                    LastModifiedBy = modifiedBy 
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido: {OrderId}", id);
                throw;
            }
        }

        public async Task<bool> UpdatePrintStatusAsync(string id, PrintStatus printStatus, string modifiedBy)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    UPDATE OrderEntry
                    SET PrintStatus = @PrintStatus, LastModifiedAt = @LastModifiedAt, LastModifiedBy = @LastModifiedBy
                    WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    PrintStatus = printStatus.ToString(),
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = modifiedBy
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status de impressão do pedido: {OrderId}", id);
                throw;
            }
        }
    }
} 
