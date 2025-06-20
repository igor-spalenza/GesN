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

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, Order>();

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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

        public async Task<Order?> GetByIdAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.Id = @Id";

                var orderDict = new Dictionary<string, Order>();

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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

        public async Task<Order?> GetByNumberAsync(string numberSequence)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.NumberSequence = @NumberSequence";

                var orderDict = new Dictionary<string, Order>();

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.CustomerId = @CustomerId
                    AND o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, Order>();

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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

        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.Status = @Status
                    AND o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, Order>();

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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
                    new { Status = (int)status, StateCode = (int)ObjectState.Active },
                    splitOn: "Id,Id,Id");

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetActiveAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, Order>();

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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
                _logger.LogError(ex, "Erro ao buscar pedidos ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetPendingDeliveryAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    AND o.Status IN (@ReadyForDelivery, @InDelivery)
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, Order>();

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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
                    new { 
                        StateCode = (int)ObjectState.Active,
                        ReadyForDelivery = (int)OrderStatus.ReadyForDelivery,
                        InDelivery = (int)OrderStatus.InDelivery
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

        public async Task<IEnumerable<Order>> GetPendingPrintAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    AND o.PrintStatus = @PrintStatus
                    AND o.Status NOT IN (@Draft, @Cancelled)
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, Order>();

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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
                    new { 
                        StateCode = (int)ObjectState.Active,
                        PrintStatus = (int)PrintStatus.NotPrinted,
                        Draft = (int)OrderStatus.Draft,
                        Cancelled = (int)OrderStatus.Cancelled
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

        public async Task<IEnumerable<Order>> SearchAsync(string searchTerm)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    AND (
                        o.NumberSequence LIKE @SearchTerm
                        OR c.FirstName LIKE @SearchTerm
                        OR c.LastName LIKE @SearchTerm
                        OR c.Email LIKE @SearchTerm
                        OR c.DocumentNumber LIKE @SearchTerm
                        OR c.Phone LIKE @SearchTerm
                    )
                    ORDER BY o.OrderDate DESC";

                var orderDict = new Dictionary<string, Order>();
                var searchPattern = $"%{searchTerm}%";

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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
                _logger.LogError(ex, "Erro ao buscar pedidos por termo: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<string> CreateAsync(Order order)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    INSERT INTO [Order] (
                        Id, NumberSequence, OrderDate, DeliveryDate, CustomerId,
                        Status, Type, TotalAmount, Subtotal, TaxAmount, DiscountAmount,
                        Notes, DeliveryAddressId, RequiresFiscalReceipt, FiscalDataId,
                        PrintStatus, PrintBatchNumber, StateCode, CreatedAt, CreatedBy,
                        LastModifiedAt, LastModifiedBy
                    ) VALUES (
                        @Id, @NumberSequence, @OrderDate, @DeliveryDate, @CustomerId,
                        @Status, @Type, @TotalAmount, @Subtotal, @TaxAmount, @DiscountAmount,
                        @Notes, @DeliveryAddressId, @RequiresFiscalReceipt, @FiscalDataId,
                        @PrintStatus, @PrintBatchNumber, @StateCode, @CreatedAt, @CreatedBy,
                        @LastModifiedAt, @LastModifiedBy
                    )";

                await connection.ExecuteAsync(sql, order);
                return order.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    UPDATE [Order] SET
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
                        PrintBatchNumber = @PrintBatchNumber,
                        StateCode = @StateCode,
                        LastModifiedAt = @LastModifiedAt,
                        LastModifiedBy = @LastModifiedBy
                    WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(sql, order);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido: {Id}", order.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "UPDATE [Order] SET StateCode = @StateCode WHERE Id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "SELECT COUNT(1) FROM [Order] WHERE Id = @Id";
                var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do pedido: {Id}", id);
                throw;
            }
        }

        public async Task<int> CountAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                const string sql = "SELECT COUNT(*) FROM [Order] WHERE StateCode = @StateCode";
                return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar pedidos");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    SELECT o.*, c.*, a.*, f.*
                    FROM [Order] o
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    LEFT JOIN Address a ON o.DeliveryAddressId = a.Id
                    LEFT JOIN FiscalData f ON o.FiscalDataId = f.Id
                    WHERE o.StateCode = @StateCode
                    ORDER BY o.OrderDate DESC
                    LIMIT @PageSize OFFSET @Offset";

                var orderDict = new Dictionary<string, Order>();
                var offset = (page - 1) * pageSize;

                await connection.QueryAsync<Order, Customer?, Address?, FiscalData?, Order>(
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
                _logger.LogError(ex, "Erro ao buscar pedidos paginados: Page={Page}, PageSize={PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<string> GetNextNumberSequenceAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Busca o último número sequencial
                const string sql = @"
                    SELECT NumberSequence
                    FROM [Order]
                    WHERE NumberSequence LIKE 'PED-%'
                    ORDER BY CAST(SUBSTR(NumberSequence, 5) AS INTEGER) DESC
                    LIMIT 1";

                var lastNumber = await connection.QuerySingleOrDefaultAsync<string>(sql);

                if (string.IsNullOrEmpty(lastNumber))
                {
                    // Se não houver nenhum pedido, começa do 1
                    return "PED-1";
                }

                // Extrai o número e incrementa
                var number = int.Parse(lastNumber.Substring(4)) + 1;
                return $"PED-{number}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar próximo número sequencial");
                throw;
            }
        }

        public async Task<bool> UpdatePrintStatusAsync(string id, PrintStatus status, int? batchNumber = null)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    UPDATE [Order] 
                    SET PrintStatus = @PrintStatus,
                        PrintBatchNumber = @PrintBatchNumber,
                        LastModifiedAt = @LastModifiedAt
                    WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(sql, new 
                { 
                    Id = id,
                    PrintStatus = (int)status,
                    PrintBatchNumber = batchNumber,
                    LastModifiedAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status de impressão do pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(string id, OrderStatus status)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                const string sql = @"
                    UPDATE [Order] 
                    SET Status = @Status,
                        LastModifiedAt = @LastModifiedAt
                    WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(sql, new 
                { 
                    Id = id,
                    Status = (int)status,
                    LastModifiedAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido: {Id}", id);
                throw;
            }
        }
    }
} 